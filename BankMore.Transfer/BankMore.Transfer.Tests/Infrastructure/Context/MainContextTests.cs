using BankMore.Transfer.Domain.Entities;
using BankMore.Transfer.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Transfer.Tests.Infrastructure.Context
{
    [TestClass]
    public class MainContextTests
    {
        private MainContext _context;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MainContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new MainContext(options);
        }

        [TestMethod]
        public async Task Can_AddAndRetrieve_ContaCorrente()
        {
            // Arrange
            var transferencia = new Transferencia(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, 100m);

            // Act
            await _context.Transferencias.AddAsync(transferencia);
            await _context.SaveChangesAsync();

            var retrieved = await _context.Transferencias.FindAsync(transferencia.IdTransferencia);

            // Assert
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(transferencia.IdTransferencia, retrieved.IdTransferencia);
            Assert.AreEqual(transferencia.Valor, retrieved.Valor);
        }
    }
}
