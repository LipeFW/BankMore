using BankMore.Account.Domain.Entities;
using BankMore.Account.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Account.Tests.Infrastructure.Context
{
    [TestClass]
    public class MainContextTests
    {
        private MainContext _context;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MainContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // banco isolado por teste
                .Options;

            _context = new MainContext(options);
        }

        [TestMethod]
        public async Task Can_Add_And_Retrieve_ContaCorrente()
        {
            // Arrange
            var conta = new ContaCorrente("52998224725", "Felipe Weber", "hash", 12345, "salt");

            // Act
            await _context.ContasCorrentes.AddAsync(conta);
            await _context.SaveChangesAsync();

            var retrieved = await _context.ContasCorrentes.FindAsync(conta.IdContaCorrente);

            // Assert
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(conta.Nome, retrieved.Nome);
            Assert.AreEqual(conta.Numero, retrieved.Numero);
        }

        [TestMethod]
        public async Task Can_Add_Movimento_And_LinkTo_ContaCorrente()
        {
            // Arrange
            var conta = new ContaCorrente("52998224725", "Felipe Weber", "hash", 12345, "salt");
            await _context.ContasCorrentes.AddAsync(conta);
            await _context.SaveChangesAsync();

            var movimento = new Movimento(conta.IdContaCorrente, "C", 100);
            await _context.Movimentos.AddAsync(movimento);
            await _context.SaveChangesAsync();

            // Act
            var retrievedMovimento = await _context.Movimentos
                .Include(m => m.ContaCorrente)
                .FirstOrDefaultAsync(m => m.IdMovimento == movimento.IdMovimento);

            // Assert
            Assert.IsNotNull(retrievedMovimento);
            Assert.AreEqual(conta.IdContaCorrente, retrievedMovimento.ContaCorrente.IdContaCorrente);
        }
    }
}
