
using BankMore.Account.Api.Configuration;
using BankMore.Account.Application.Commands;
using BankMore.Account.Domain.Interfaces;
using BankMore.Account.Infrastructure.Context;
using BankMore.Account.Infrastructure.Data;
using BankMore.Account.Infrastructure.Repositories;
using BankMore.Account.Infrastructure.Utils;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
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
            builder.Services.AddSwaggerGen();

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(RegisterAccountCommand).Assembly);
            });

            SqlMapper.AddTypeHandler(new GuidTypeHandler());

            builder.Services.AddScoped<IAccountRepository, AccountRepository>();

            var dbPath = Environment.GetEnvironmentVariable("SQLITE_DB_PATH")
                         ?? Path.Combine(AppContext.BaseDirectory, "data", "app.db");

            // Garante que o diretório existe
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

            builder.Services.AddDbContext<MainContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            builder.Services.AddScoped<IDbConnection>(sp =>
            {
                var context = new SQLiteContext("Data Source=/app/data/app.db");
                return context.CreateConnection();
            });

            // Configurações de JWT
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            builder.Services.Configure<JwtConfiguration>(jwtSettings);

            var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<MainContext>();
                db.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger(); 
                app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Urls.Add("http://0.0.0.0:80");
            app.MapGet("/", () => "API rodando!");

            app.Run();
        }
    }
}
