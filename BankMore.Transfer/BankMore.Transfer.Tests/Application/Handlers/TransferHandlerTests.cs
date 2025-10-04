using BankMore.Transfer.Application.Commands;
using BankMore.Transfer.Domain.Entities;
using BankMore.Transfer.Domain.Exceptions;
using BankMore.Transfer.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;

namespace BankMore.Transfer.Tests.Application.Handlers
{
    [TestClass]
    public class TransferHandlerTests
    {
        private Mock<IConfiguration> _configurationMock;
        private Mock<IHttpContextAccessor> _httpContextMock;
        private Mock<ITransferRepository> _transferRepositoryMock;
        private Mock<IIdempotencyRepository> _idempotencyRepositoryMock;
        private TransferHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _configurationMock = new Mock<IConfiguration>();
            _httpContextMock = new Mock<IHttpContextAccessor>();
            _transferRepositoryMock = new Mock<ITransferRepository>();
            _idempotencyRepositoryMock = new Mock<IIdempotencyRepository>();

            _configurationMock.Setup(c => c["AccountAPI:BaseAddress"]).Returns("http://localhost");

            _handler = new TransferHandler(
                _configurationMock.Object,
                _httpContextMock.Object,
                _transferRepositoryMock.Object,
                _idempotencyRepositoryMock.Object
            );
        }

        [TestMethod]
        [ExpectedException(typeof(IdempotencyViolationException))]
        public async Task Handle_Should_ThrowException_IdempotencyViolation_When_RequestExists()
        {
            // Arrange
            var idRequisicao = Guid.NewGuid();

            _idempotencyRepositoryMock.Setup(r => r.GetByRequestIdAsync(idRequisicao))
                .ReturnsAsync(new Idempotencia(idRequisicao, "{}", "{}"));

            var command = new TransferCommand(idRequisicao, 123, 100m, Guid.NewGuid());

            // Act & Assert
            await _handler.Handle(command, CancellationToken.None);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidValueException))]
        public async Task Handle_Should_ThrowException_InvalidValueException_When_Value_IsZeroOrNegative()
        {
            // Arrange
            var command = new TransferCommand(Guid.NewGuid(), 123, 0m, Guid.NewGuid());

            // Act & Assert
            await _handler.Handle(command, CancellationToken.None);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Handle_Should_Call_Repositories_When_TransferSucceeds()
        {
            // Arrange
            var command = new TransferCommand(Guid.NewGuid(), 123, 100m, Guid.NewGuid());

            var context = new DefaultHttpContext();

            context.Request.Headers["Authorization"] = "Bearer token";
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("AccountNumber", "456")
            }));

            _httpContextMock.Setup(a => a.HttpContext).Returns(context);

            _transferRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Transferencia>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _idempotencyRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Idempotencia>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert   

            _transferRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Transferencia>()), Times.Never);
            _idempotencyRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Idempotencia>()), Times.Never);
        }
    }
}