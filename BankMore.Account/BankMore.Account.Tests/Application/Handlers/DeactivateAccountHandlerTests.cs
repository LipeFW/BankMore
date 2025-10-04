using BankMore.Account.Application.Commands;
using BankMore.Account.Domain.Entities;
using BankMore.Account.Domain.Exceptions;
using BankMore.Account.Domain.Interfaces;
using Moq;
using System.Security.Principal;
using BCryptHelper = BCrypt.Net.BCrypt;

namespace BankMore.Account.Tests.Application.Commands
{
    [TestClass]
    public class DeactivateAccountHandlerTests
    {
        private Mock<IAccountRepository> _repositoryMock;
        private DeactivateAccountHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _repositoryMock = new Mock<IAccountRepository>();
            _handler = new DeactivateAccountHandler(_repositoryMock.Object);
        }

        [TestMethod]
        public async Task Handle_ShouldDeactivateAccount_WhenAccountExistsAndPasswordIsValid()
        {
            // Arrange
            var senha = "Senha123!";
            var senhaHash = BCryptHelper.HashPassword(senha);
            var account = new ContaCorrente("52998224725", "Felipe", senhaHash, 12345, "salt");

            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
                           .ReturnsAsync(account);

            var command = new DeactivateAccountCommand(account.IdContaCorrente.ToString("d"), senha);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(account.Ativo);
            _repositoryMock.Verify(r => r.UpdateAtivoAsync(account), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidAccountException))]
        public async Task Handle_ShouldThrowException_WhenAccountDoesNotExist()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
                           .ReturnsAsync((ContaCorrente)null);

            var command = new DeactivateAccountCommand(Guid.NewGuid().ToString("d"), "Senha123!");

            // Act
            await _handler.Handle(command, CancellationToken.None);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task Handle_ShouldThrowException_WhenPasswordIsInvalid()
        {
            // Arrange
            var senhaHash = BCryptHelper.HashPassword("SenhaCorreta");
            var account = new ContaCorrente("52998224725", "Felipe", senhaHash, 12345, "salt");

            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
                           .ReturnsAsync(account);

            var command = new DeactivateAccountCommand(account.IdContaCorrente.ToString("d"), "SenhaIncorreta");

            // Act
            await _handler.Handle(command, CancellationToken.None);
        }
    }
}