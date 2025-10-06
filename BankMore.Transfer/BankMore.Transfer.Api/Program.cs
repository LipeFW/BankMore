using BankMore.Transfer.Api.Configuration;
using BankMore.Transfer.Application.Commands;
using BankMore.Transfer.Domain.Interfaces;
using BankMore.Transfer.Infrastructure.Context;
using BankMore.Transfer.Infrastructure.Kafka;
using BankMore.Transfer.Infrastructure.Repositories;
using BankMore.Transfer.Infrastructure.Utils;
using Dapper;
using KafkaFlow;
using KafkaFlow.Serializer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Oracle.ManagedDataAccess.Client;
using RestSharp;
using System.Data;
using System.Net;
using System.Reflection;
using System.Text;

namespace BankMore.Transfer.Api
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BankMore.Transfer.Api", Version = "v1" });

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

            // Configurações de JWT
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            builder.Services.Configure<JwtConfiguration>(jwtSettings);

            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
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

            SqlMapper.AddTypeHandler<Guid>(new GuidTypeHandler());

            builder.Services.AddDbContext<MainContext>(options =>
                options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection"))
            );

            builder.Services.AddScoped<IDbConnection>(sp =>
            {
                var connection = new OracleConnection(builder.Configuration.GetConnectionString("OracleConnection"));
                connection.Open(); // Abre ao criar
                return connection;
            });

            builder.Services.AddSingleton<RestClient>(sp =>
            {
                return new RestClient(builder.Configuration["AccountAPI:BaseAddress"] ?? "http://localhost:5001/");
            });

            builder.Services.AddScoped<IIdempotencyRepository, IdempotencyRepository>();
            builder.Services.AddScoped<ITransferRepository, TransferRepository>();

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(TransferCommand).Assembly);
            });

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddAuthorization();

            builder.Services.AddKafka(kafka => kafka
            .UseConsoleLog()
                    .AddCluster(cluster => cluster
                        .WithBrokers(new[] { builder.Configuration["Kafka:Host"] ?? "172.18.16.1:9092" })
                        .AddProducer("transfer-producer", producer => producer
                            .DefaultTopic("transfers")
                        .AddMiddlewares(m => m.AddSerializer<JsonCoreSerializer>())
                        )
                    )
                );

            builder.Services.AddSingleton<ITransferMessageProducer, TransferMessageProducer>();


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
