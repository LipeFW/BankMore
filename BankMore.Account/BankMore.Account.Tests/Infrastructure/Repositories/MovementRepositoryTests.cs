using BankMore.Account.Domain.Entities;
using BankMore.Account.Domain.Interfaces;
using BankMore.Account.Infrastructure.Repositories;
using BankMore.Account.Infrastructure.Utils;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;

namespace BankMore.Account.Tests.Infrastructure.Repositories
{
    [TestClass]
    public class MovementRepositoryTests
    {
        private IDbConnection _connection;
        private IMovementRepository _repository;
        private IAccountRepository _accountRepository;

        [TestInitialize]
        public void Setup()
        {
            SqlMapper.AddTypeHandler(new GuidTypeHandler());

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var tableConta = @"
                CREATE TABLE ContaCorrente (
                    IdContaCorrente TEXT PRIMARY KEY,
                    Cpf TEXT NOT NULL,
                    Nome TEXT NOT NULL,
                    Senha TEXT NOT NULL,
                    Numero INTEGER NOT NULL,
                    Ativo INTEGER NOT NULL,
                    Salt TEXT NOT NULL
                );";
            _connection.Execute(tableConta);

            var tableMovimento = @"
                CREATE TABLE Movimento (
                    IdMovimento TEXT PRIMARY KEY,
                    IdContaCorrente TEXT NOT NULL,
                    DataMovimento TEXT NOT NULL,
                    TipoMovimento TEXT NOT NULL,
                    Valor REAL NOT NULL,
                    FOREIGN KEY (IdContaCorrente) REFERENCES ContaCorrente(IdContaCorrente)
                );";
            _connection.Execute(tableMovimento);

            _repository = new MovementRepository(_connection);
            _accountRepository = new AccountRepository(_connection);    
        }

        [TestCleanup]
        public void Cleanup()
        {
            _connection?.Dispose();
        }

        [TestMethod]
        public async Task AddAsync_Should_InsertMovement()
        {
            // Arrange

            var conta = new ContaCorrente("52998224725", "Felipe", "hash", 12345, "salt");
            var movimento = new Movimento(conta.IdContaCorrente, "C", 100m);

            // Act
            await _accountRepository.AddAsync(conta);

            await _repository.AddAsync(movimento);

            // Assert
            var movimentoCriado = await _repository.GetByIdAsync(movimento.IdMovimento.ToString("d"));

            Assert.IsNotNull(movimentoCriado);
            Assert.AreEqual(movimento.IdContaCorrente, movimentoCriado.IdContaCorrente);
            Assert.AreEqual(movimento.TipoMovimento, movimentoCriado.TipoMovimento);
            Assert.AreEqual(movimento.Valor, movimentoCriado.Valor);
            Assert.AreEqual(movimento.DataMovimento, movimentoCriado.DataMovimento);
        }
    }
}