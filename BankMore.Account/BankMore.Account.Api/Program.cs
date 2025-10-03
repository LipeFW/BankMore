
using BankMore.Account.Api.Configuration;
using BankMore.Account.Application.Commands;
using BankMore.Account.Application.Services;
using BankMore.Account.Domain.Interfaces;
using BankMore.Account.Infrastructure.Context;
using BankMore.Account.Infrastructure.Repositories;
using BankMore.Account.Infrastructure.Utils;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Net;
using System.Reflection;
using System.Text;

namespace BankMore.Account.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BankMore.Account.Api", Version = "v1" });

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insira o token JWT desta forma: Bearer {seu token}"
                };

                c.AddSecurityDefinition("Bearer", securityScheme);

                var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            };

                c.AddSecurityRequirement(securityRequirement);

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            SqlMapper.AddTypeHandler<Guid>(new GuidTypeHandler());

            builder.Services.AddDbContext<MainContext>(options =>
                options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection"))
            );

            builder.Services.AddScoped<IDbConnection>(sp =>
            {
                var connectionString = builder.Configuration.GetConnectionString("OracleConnection");
                var connection = new OracleConnection(connectionString);
                connection.Open(); // Abre ao criar
                return connection;
            });

            builder.Services.AddScoped<IIdempotencyRepository, IdempotencyRepository>();
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<IMovementRepository, MovementRepository>();
            builder.Services.AddScoped<ITokenService, TokenService>();

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CreateAccountCommand).Assembly);
            });

            // Configurações de JWT
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            builder.Services.Configure<JwtConfiguration>(jwtSettings);

            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            builder.Services.AddAuthentication("Bearer")
             .AddJwtBearer("Bearer", options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ClockSkew = TimeSpan.Zero,
                     ValidIssuer = jwtSettings["Issuer"],
                     ValidAudience = jwtSettings["Audience"],
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
                 };

                 options.Events = new JwtBearerEvents
                 {
                     OnAuthenticationFailed = context =>
                     {
                         // Se a rota permitir [AllowAnonymous], não retorna 403
                         var endpoint = context.HttpContext.GetEndpoint();
                         var allowAnonymous = endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null;

                         if (allowAnonymous)
                         {
                             return Task.CompletedTask;
                         }

                         if (context.Exception is SecurityTokenExpiredException)
                         {
                             context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                             context.Response.ContentType = "application/json";
                             return context.Response.WriteAsync("{\"message\": \"Token expirado\"}");
                         }

                         return Task.CompletedTask;
                     }
                 };
             });

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddAuthorization();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<MainContext>();
                db.Database.Migrate();
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
