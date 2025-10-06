using BankMore.Tariff.Application.Commands;
using BankMore.Tariff.Domain.Interfaces;
using BankMore.Tariff.Domain.Messages;
using BankMore.Tariff.Infrastructure.Context;
using BankMore.Tariff.Infrastructure.Kafka;
using BankMore.Tariff.Infrastructure.Repositories;
using BankMore.Tariff.Infrastructure.Utils;
using BankMore.Tariff.Worker.Handlers;
using Dapper;
using KafkaFlow;
using KafkaFlow.Serializer;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace BankMore.Tariff.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

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

            builder.Services.AddScoped<ITariffRepository, TariffRepository>();

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CreateTariffCommand).Assembly);
            });

            builder.Services.AddHostedService<KafkaFlowHostedService>();

            builder.Services.AddKafka(kafka => kafka
                .UseConsoleLog()
                .AddCluster(cluster => cluster
                    .WithBrokers(new[] { builder.Configuration["Kafka:Host"] ?? "172.18.16.1:9092" })
                    // CONSUMER - lê as transferências realizadas
                    .AddConsumer(consumer => consumer
                        .Topic("transfers")
                        .WithGroupId("transfers-worker")
                        .WithBufferSize(100)
                        .WithWorkersCount(1)
                        .AddMiddlewares(m => m
                            .AddSingleTypeDeserializer<JsonCoreDeserializer>(typeof(TransferMessage))
                            .AddTypedHandlers(h => h
                                .WithHandlerLifetime(InstanceLifetime.Scoped)
                                .AddHandler<TransferMessageHandler>())
                        )
                    )
                    // PRODUCER - publica as tarifas criadas
                    .AddProducer("tariff-producer", producer => producer
                        .DefaultTopic("tariffs")
                    .AddMiddlewares(m => m.AddSerializer<JsonCoreSerializer>())
                    )
                )
            );

            builder.Services.AddSingleton<ITariffMessageProducer, TariffMessageProducer>();


            var host = builder.Build();
            host.Run();

            using (var scope = host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<MainContext>();
                db.Database.Migrate();
            }
        }
    }
}
