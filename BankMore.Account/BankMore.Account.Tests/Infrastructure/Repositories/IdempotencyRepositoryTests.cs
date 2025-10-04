using BankMore.Account.Domain.Entities;
using BankMore.Account.Infrastructure.Repositories;
using BankMore.Account.Infrastructure.Utils;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;

namespace BankMore.Account.Tests.Infrastructure.Repositories
{
    [TestClass]
    public class IdempotencyRepositoryTests
    {
        private IDbConnection _connection;
        private IdempotencyRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            // Configurar TypeHandler para Guid
            SqlMapper.AddTypeHandler(new GuidTypeHandler());

            // Conexão SQLite in-memory
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            // Criar tabela Idempotencia
            var tableIdempotencia = @"
                CREATE TABLE Idempotencia (
                    Chave_Idempotencia TEXT PRIMARY KEY,
                    Requisicao TEXT,
                    Resultado TEXT
                );";
            _connection.Execute(tableIdempotencia);

            _repository = new IdempotencyRepository(_connection);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _connection?.Dispose();
        }

        [TestMethod]
        public async Task AddAsync_Should_InsertRecord()
        {
            // Arrange
            var idempotencia = new Idempotencia(Guid.NewGuid(), "{}", "{}");

            // Act
            await _repository.AddAsync(idempotencia);

            // Assert
            var inserted = await _connection.QuerySingleOrDefaultAsync<Idempotencia>(
                "SELECT Chave_Idempotencia ChaveIdempotencia, Requisicao, Resultado FROM Idempotencia WHERE Chave_Idempotencia = @Id",
                new { Id = idempotencia.ChaveIdempotencia.ToString("d") }
            );

            Assert.IsNotNull(inserted);
            Assert.AreEqual(idempotencia.Requisicao, inserted.Requisicao);
            Assert.AreEqual(idempotencia.Resultado, inserted.Resultado);
        }

        [TestMethod]
        public async Task GetByRequestIdAsync_Should_Return_Record_WhenExists()
        {
            // Arrange
            var idempotencia = new Idempotencia(Guid.NewGuid(), "{}", "{}");
            await _repository.AddAsync(idempotencia);

            // Act
            var idempotenciaGet = await _repository.GetByRequestIdAsync(idempotencia.ChaveIdempotencia);

            // Assert
            Assert.IsNotNull(idempotenciaGet);
            Assert.AreEqual(idempotencia.ChaveIdempotencia, idempotenciaGet.ChaveIdempotencia);
            Assert.AreEqual(idempotencia.Requisicao, idempotenciaGet.Requisicao);
            Assert.AreEqual(idempotencia.Resultado, idempotenciaGet.Resultado);
        }

        [TestMethod]
        public async Task GetByRequestIdAsync_Should_Return_Null_When_NotExists()
        {
            // Act
            var idempotencia = await _repository.GetByRequestIdAsync(Guid.NewGuid());

            // Assert
            Assert.IsNull(idempotencia);
        }
    }
}