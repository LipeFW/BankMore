using BankMore.Transfer.Domain.Entities;
using BankMore.Transfer.Infrastructure.Repositories;
using BankMore.Transfer.Infrastructure.Utils;
using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankMore.Transfer.Tests.Infrastructure.Repositories
{
    [TestClass]
    public class TransferRepositoryTests
    {
        private IDbConnection _connection;
        private TransferRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            // Registrar TypeHandler para Guid
            SqlMapper.AddTypeHandler(new GuidTypeHandler());

            // Conexão SQLite in-memory
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            // Criar tabela Transferencia
            var tableTransfer = @"
                CREATE TABLE Transferencia (
                    IdTransferencia TEXT PRIMARY KEY,
                    IdContaCorrenteOrigem TEXT NOT NULL,
                    IdContaCorrenteDestino TEXT NOT NULL,
                    Valor REAL NOT NULL,
                    DataTransferencia TEXT NOT NULL
                );";
            _connection.Execute(tableTransfer);

            _repository = new TransferRepository(_connection);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _connection?.Dispose();
        }

        [TestMethod]
        public async Task AddAsync_ShouldInsertTransfer()
        {
            // Arrange
            var transfer = new Transferencia
            {
                IdTransferencia = Guid.NewGuid(),
                IdContaCorrenteOrigem = Guid.NewGuid(),
                IdContaCorrenteDestino = Guid.NewGuid(),
                Valor = 500m,
                DataTransferencia = DateTime.UtcNow
            };

            // Act
            await _repository.AddAsync(transfer);

            // Assert
            var inserted = await _connection.QuerySingleOrDefaultAsync<Transferencia>(
                "SELECT IdTransferencia IdTransferencia, IdContaCorrenteOrigem IdContaCorrenteOrigem, IdContaCorrenteDestino IdContaCorrenteDestino, Valor, DataTransferencia FROM Transferencia WHERE IdTransferencia = @Id",
                new { Id = transfer.IdTransferencia.ToString("D") }
            );

            Assert.IsNotNull(inserted);
            Assert.AreEqual(transfer.IdTransferencia, inserted.IdTransferencia);
            Assert.AreEqual(transfer.IdContaCorrenteOrigem, inserted.IdContaCorrenteOrigem);
            Assert.AreEqual(transfer.IdContaCorrenteDestino, inserted.IdContaCorrenteDestino);
            Assert.AreEqual(transfer.Valor, inserted.Valor);
            Assert.AreEqual(transfer.DataTransferencia, inserted.DataTransferencia);
        }
    }
}