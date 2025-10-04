using BankMore.Account.Domain.Entities;
using BankMore.Account.Infrastructure.Repositories;
using BankMore.Account.Infrastructure.Utils;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BankMore.Account.Tests.Infrastructure.Repositories
{
    [TestClass]
    public class AccountRepositoryTests
    {
        private IDbConnection _connection;
        private AccountRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            SqlMapper.AddTypeHandler(new GuidTypeHandler());

            var tableConta = @"CREATE TABLE ContaCorrente (
                            IdContaCorrente TEXT PRIMARY KEY,
                            Cpf TEXT NOT NULL,
                            Nome TEXT NOT NULL,
                            Senha TEXT NOT NULL,
                            Numero INTEGER NOT NULL,
                            Ativo INTEGER NOT NULL,
                            Salt TEXT NOT NULL
                          );";

            _connection.Execute(tableConta);

            var tableMovimento = @"CREATE TABLE Movimento (
                                IdMovimento TEXT PRIMARY KEY,
                                IdContaCorrente TEXT NOT NULL,
                                TipoMovimento TEXT NOT NULL,
                                Valor REAL NOT NULL,
                                DataMovimento TEXT NOT NULL
                            );";

            _connection.Execute(tableMovimento);

            _repository = new AccountRepository(_connection);
        }

        [TestMethod]
        public async Task Add_And_GetByCpf_Should_Return_InsertedAccount()
        {
            // Arrange
            var conta = new ContaCorrente("52998224725", "Felipe", "hash", 12345, "salt");

            // Act
            await _repository.AddAsync(conta);
            var retrieved = await _repository.GetByCpfAsync(conta.Cpf);

            // Assert
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(conta.Nome, retrieved.Nome);
            Assert.AreEqual(conta.Numero, retrieved.Numero);
        }

        [TestMethod]
        public async Task GetSaldo_Should_Return_Zero_When_NoMovements()
        {
            // Arrange
            var conta = new ContaCorrente("52998224725", "Felipe", "hash", 12345, "salt");
            await _repository.AddAsync(conta);

            // Act
            string sqlLiteQuery = @"
                SELECT 
                    IFNULL(SUM(
                        CASE WHEN m.TipoMovimento = 'C' THEN m.Valor
                             WHEN m.TipoMovimento = 'D' THEN -m.Valor
                             ELSE 0 END
                    ), 0) AS Saldo
                FROM ContaCorrente c
                LEFT JOIN Movimento m ON c.IdContaCorrente = m.IdContaCorrente
                WHERE c.IdContaCorrente = @IdContaCorrente
                GROUP BY c.Numero, c.Nome
            ";

            var saldo = await _connection.QuerySingleOrDefaultAsync<decimal>(
                sqlLiteQuery,
                new { conta.IdContaCorrente }
            );

            // Assert
            Assert.AreEqual(0, saldo);
        }
    }
}