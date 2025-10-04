using BankMore.Account.Application.Commands;
using BankMore.Account.Domain.Entities;
using BankMore.Account.Domain.Interfaces;
using Moq;
using BCryptHelper = BCrypt.Net.BCrypt;

namespace BankMore.Account.Tests.Application.Commands
{
    [TestClass]
    public class LoginAccountHandlerTests
    {
        private Mock<IAccountRepository> _repositoryMock;
        private Mock<ITokenService> _tokenServiceMock;
        private LoginAccountHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _repositoryMock = new Mock<IAccountRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _handler = new LoginAccountHandler(_repositoryMock.Object, _tokenServiceMock.Object);
        }

        [TestMethod]
        public async Task Handle_Should_Return_Token_When_Login_With_AccountNumber_Succeeds()
        {
            // Arrange
            var senha = "Senha123!";
            var senhaHash = BCryptHelper.HashPassword(senha);
            var account = new ContaCorrente("52998224725", "Felipe", senhaHash, 12345, "salt");

            _repositoryMock.Setup(r => r.GetByAccountNumberAsync(12345))
                           .ReturnsAsync(account);

            _tokenServiceMock.Setup(t => t.GenerateToken(account))
                             .Returns("token-jwt");

            var command = new LoginAccountCommand("12345", senha);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("token-jwt", result.Token);
        }

        [TestMethod]
        public async Task Handle_Should_Return_Token_When_Login_With_Cpf_Succeeds()
        {
            // Arrange
            var senha = "Senha123!";
            var senhaHash = BCryptHelper.HashPassword(senha);
            var account = new ContaCorrente("52998224725", "Felipe", senhaHash, 12345, "salt");

            _repositoryMock.Setup(r => r.GetByCpfAsync("52998224725"))
                           .ReturnsAsync(account);

            _tokenServiceMock.Setup(t => t.GenerateToken(account))
                             .Returns("token-jwt");

            var command = new LoginAccountCommand("529.982.247-25", senha);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("token-jwt", result.Token);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task Handle_Should_ThrowException_When_CpfOrAccountNumber_IsEmpty()
        {
            // Arrange
            var command = new LoginAccountCommand(" ", "Senha123");

            // Act
            await _handler.Handle(command, CancellationToken.None);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task Handle_Should_ThrowException_When_Cpf_IsInvalid()
        {
            // Arrange
            var command = new LoginAccountCommand("12345678900", "Senha123");

            // Act
            await _handler.Handle(command, CancellationToken.None);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task Handle_Should_ThrowException_When_Account_DoesNotExist()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByAccountNumberAsync(12345))
                           .ReturnsAsync((ContaCorrente)null);

            var command = new LoginAccountCommand("12345", "Senha123");

            // Act
            await _handler.Handle(command, CancellationToken.None);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task Handle_Should_ThrowException_When_Password_IsInvalid()
        {
            // Arrange
            var senhaHash = BCryptHelper.HashPassword("SenhaCorreta");
            var account = new ContaCorrente("52998224725", "Felipe", senhaHash, 12345, "salt");

            _repositoryMock.Setup(r => r.GetByAccountNumberAsync(12345))
                           .ReturnsAsync(account);

            var command = new LoginAccountCommand("12345", "SenhaErrada");

            // Act
            await _handler.Handle(command, CancellationToken.None);
        }
    }
}