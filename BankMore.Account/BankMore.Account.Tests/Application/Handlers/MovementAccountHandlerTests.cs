using BankMore.Account.Application.Commands;
using BankMore.Account.Domain.Entities;
using BankMore.Account.Domain.Exceptions;
using BankMore.Account.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;

namespace BankMore.Account.Tests.Application.Handlers
{
    [TestClass]
    public class MovementAccountHandlerTests
    {
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IAccountRepository> _accountRepositoryMock;
        private Mock<IMovementRepository> _movementRepositoryMock;
        private Mock<IIdempotencyRepository> _idempotencyRepositoryMock;
        private MovementAccountHandler _handler;
        private DefaultHttpContext _httpContext;

        [TestInitialize]
        public void Setup()
        {
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _movementRepositoryMock = new Mock<IMovementRepository>();
            _idempotencyRepositoryMock = new Mock<IIdempotencyRepository>();

            _httpContext = new DefaultHttpContext();
            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(_httpContext);

            _handler = new MovementAccountHandler(
                _httpContextAccessorMock.Object,
                _accountRepositoryMock.Object,
                _movementRepositoryMock.Object,
                _idempotencyRepositoryMock.Object);
        }

        [TestMethod]
        public async Task Handle_Should_Process_Movement_Successfully()
        {
            // Arrange
            var account = new ContaCorrente("52998224725", "Felipe", "hash", 12345, "salt");
            var command = new MovementAccountCommand(Guid.NewGuid(), 12345, account.IdContaCorrente.ToString("d"), 100, "C");

            _accountRepositoryMock.Setup(r => r.GetByAccountNumberAsync(12345))
                                      .ReturnsAsync(account);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _movementRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Movimento>()), Times.Once);
            _idempotencyRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Idempotencia>()), Times.Once);
            Assert.IsTrue(_httpContext.Response.Headers.ContainsKey("AccountId"));
            Assert.AreEqual(account.IdContaCorrente.ToString(), _httpContext.Response.Headers["AccountId"]);
        }

        [TestMethod]
        [ExpectedException(typeof(IdempotencyViolationException))]
        public async Task Handle_Should_ThrowException_When_RequestId_AlreadyExists()
        {
            // Arrange
            var command = new MovementAccountCommand(Guid.NewGuid(), 12345, Guid.NewGuid().ToString("d"), 100, "C");

            _idempotencyRepositoryMock.Setup(r => r.GetByRequestIdAsync(command.IdRequisicao))
                                      .ReturnsAsync(new Idempotencia());

            // Act
            await _handler.Handle(command, CancellationToken.None);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidAccountException))]
        public async Task Handle_Should_ThrowException_When_Account_DoesNotExist()
        {
            // Arrange
            var command = new MovementAccountCommand(Guid.NewGuid(), 12345, Guid.NewGuid().ToString("d"), 100, "C");

            // Act
            await _handler.Handle(command, CancellationToken.None);
        }

        [TestMethod]
        [ExpectedException(typeof(InactiveAccountException))]
        public async Task Handle_Should_ThrowException_When_Account_IsInactive()
        {
            // Arrange
            var account = new ContaCorrente("52998224725", "Felipe", "hash", 12345, "salt") { Ativo = false };
            var command = new MovementAccountCommand(Guid.NewGuid(), 12345, Guid.NewGuid().ToString("d"), 100, "C");


            _accountRepositoryMock.Setup(r => r.GetByAccountNumberAsync(12345)).ReturnsAsync(account);

            // Act
            await _handler.Handle(command, CancellationToken.None);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidValueException))]
        public async Task Handle_Should_ThrowException_When_Value_IsNegative()
        {
            // Arrange
            var account = new ContaCorrente("52998224725", "Felipe", "hash", 12345, "salt");
            var command = new MovementAccountCommand(Guid.NewGuid(), 12345, Guid.NewGuid().ToString("d"), -10, "C");

            _accountRepositoryMock.Setup(r => r.GetByAccountNumberAsync(12345)).ReturnsAsync(account);

            // Act
            await _handler.Handle(command, CancellationToken.None);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidTypeException))]
        public async Task Handle_Should_ThrowException_When_Type_IsInvalid()
        {
            // Arrange
            var account = new ContaCorrente("52998224725", "Felipe", "hash", 12345, "salt");
            var command = new MovementAccountCommand(Guid.NewGuid(), 12345, Guid.NewGuid().ToString("d"), 100, "X");

            _accountRepositoryMock.Setup(r => r.GetByAccountNumberAsync(12345)).ReturnsAsync(account);

            // Act
            await _handler.Handle(command, CancellationToken.None);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidTypeException))]
        public async Task Handle_Should_ThrowException_When_Debit_On_DifferentAccount()
        {
            // Arrange
            var account = new ContaCorrente("52998224725", "Felipe", "hash", 12345, "salt");

            var command = new MovementAccountCommand(Guid.NewGuid(), 99999, Guid.NewGuid().ToString("d"), 99999, "D");

            _accountRepositoryMock.Setup(r => r.GetByAccountNumberAsync(command.NumeroConta)).ReturnsAsync(account);

            // Act
            await _handler.Handle(command, CancellationToken.None);
        }
    }
}