using BankMore.Account.Application.Commands;
using BankMore.Account.Domain.Entities;
using BankMore.Account.Domain.Exceptions;
using BankMore.Account.Domain.Interfaces;
using Moq;

namespace BankMore.Account.Tests.Application.Commands
{
    [TestClass]
    public class CreateAccountHandlerTests
    {
        private Mock<IAccountRepository> _repositoryMock;
        private CreateAccountHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _repositoryMock = new Mock<IAccountRepository>();
            _handler = new CreateAccountHandler(_repositoryMock.Object);
        }

        [TestMethod]
        public async Task Handle_Should_CreateAccount_When_CpfIsValidAndNotExists()
        {
            // Arrange
            var request = new CreateAccountCommand("529.982.247-25", "Felipe Weber", "123456");

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.NumeroConta > 0);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<ContaCorrente>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDocumentException))]
        public async Task Handle_Should_ThrowException_When_Cpf_IsInvalid()
        {
            // Arrange
            var request = new CreateAccountCommand("12345678900", "Felipe Weber", "123456");

            // Act
            await _handler.Handle(request, CancellationToken.None);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDocumentException))]
        public async Task Handle_Should_ThrowException_When_Cpf_AlreadyExists()
        {
            // Arrange
            var request = new CreateAccountCommand("12345678900", "Felipe Weber", "123456");

            _repositoryMock.Setup(r => r.GetByCpfAsync(It.IsAny<string>()))
                           .ReturnsAsync(new ContaCorrente("52998224725", "Outro", "hash", 123456, "salt"));

            // Act
            await _handler.Handle(request, CancellationToken.None);
        }
    }
}