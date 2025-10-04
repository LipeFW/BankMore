using BankMore.Transfer.Api.Controllers;
using BankMore.Transfer.Application.Commands;
using BankMore.Transfer.Domain.DTOs.Requests;
using BankMore.Transfer.Domain.DTOs.Responses;
using BankMore.Transfer.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Moq;
using System.Security.Claims;

namespace BankMore.Transfer.Tests.Api.Controllers
{
    [TestClass]
    public class TransfersControllerTests
    {
        private Mock<IMediator> _mediatorMock;
        private TransfersController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new TransfersController(_mediatorMock.Object);

            // Simula o HttpContext com o JWT Claims
            var accountId = Guid.NewGuid().ToString();
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, accountId),
            new Claim(JwtRegisteredClaimNames.Sub, accountId)
        };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [TestMethod]
        public async Task Post_Should_Return_NoContent_When_TransferSucceess()
        {
            // Arrange
            var request = new TransferRequest
            {
                RequestId = Guid.NewGuid(),
                NumeroContaDestino = 123,
                Valor = 100
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<TransferCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Post(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Post_Should_Return_BadRequest_When_InvalidAccount()
        {
            // Arrange
            var request = new TransferRequest
            {
                RequestId = Guid.NewGuid(),
                NumeroContaDestino = 123,
                Valor = 100
            };

            var expectedResponse = new ErrorResponse("Conta inválida", "INVALID_ACCOUNT");


            _mediatorMock
                .Setup(m => m.Send(It.IsAny<TransferCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidAccountException("Conta inválida"));

            // Act
            var result = await _controller.Post(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

            var objectResult = result as BadRequestObjectResult;
            Assert.IsNotNull(objectResult);

            var errorResponse = objectResult.Value as ErrorResponse;
            Assert.IsNotNull(errorResponse);

            Assert.AreEqual(expectedResponse.ErrorType, errorResponse.ErrorType);
            Assert.AreEqual(expectedResponse.Message, errorResponse.Message);
        }

        [TestMethod]
        public async Task Post_Should_Return_BadRequest_When_InactiveAccount()
        {
            // Arrange
            var request = new TransferRequest
            {
                RequestId = Guid.NewGuid(),
                NumeroContaDestino = 123,
                Valor = 100
            };

            var expectedResponse = new ErrorResponse("Conta inativa", "INACTIVE_ACCOUNT");


            _mediatorMock
                .Setup(m => m.Send(It.IsAny<TransferCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InactiveAccountException("Conta inativa"));

            // Act
            var result = await _controller.Post(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

            var objectResult = result as BadRequestObjectResult;
            Assert.IsNotNull(objectResult);

            var errorResponse = objectResult.Value as ErrorResponse;
            Assert.IsNotNull(errorResponse);

            Assert.AreEqual(expectedResponse.ErrorType, errorResponse.ErrorType);
            Assert.AreEqual(expectedResponse.Message, errorResponse.Message);
        }

        [TestMethod]
        public async Task Post_Should_Return_BadRequest_When_InvalidValue()
        {
            // Arrange
            var request = new TransferRequest
            {
                RequestId = Guid.NewGuid(),
                NumeroContaDestino = 123,
                Valor = 100
            };

            var expectedResponse = new ErrorResponse("Valor inválido", "INVALID_VALUE");

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<TransferCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidValueException("Valor inválido"));

            // Act
            var result = await _controller.Post(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

            var objectResult = result as BadRequestObjectResult;
            Assert.IsNotNull(objectResult);

            var errorResponse = objectResult.Value as ErrorResponse;
            Assert.IsNotNull(errorResponse);

            Assert.AreEqual(expectedResponse.ErrorType, errorResponse.ErrorType);
            Assert.AreEqual(expectedResponse.Message, errorResponse.Message);
        }

        [TestMethod]
        public async Task Post_Should_Return_BadRequest_When_InvalidOperation()
        {
            // Arrange
            var request = new TransferRequest
            {
                RequestId = Guid.NewGuid(),
                NumeroContaDestino = 123,
                Valor = 100
            };

            var expectedResponse = new ErrorResponse("Operação inválida", "INVALID_OPERATION");

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<TransferCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Operação inválida"));

            // Act
            var result = await _controller.Post(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

            var objectResult = result as BadRequestObjectResult;
            Assert.IsNotNull(objectResult);

            var errorResponse = objectResult.Value as ErrorResponse;
            Assert.IsNotNull(errorResponse);

            Assert.AreEqual(expectedResponse.ErrorType, errorResponse.ErrorType);
            Assert.AreEqual(expectedResponse.Message, errorResponse.Message);
        }
    }
}