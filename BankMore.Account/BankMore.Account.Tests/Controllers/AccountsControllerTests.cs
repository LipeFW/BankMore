using BankMore.Account.Api.Controllers;
using BankMore.Account.Application.Commands;
using BankMore.Account.Application.Queries;
using BankMore.Account.Domain.DTOs.Requests;
using BankMore.Account.Domain.DTOs.Responses;
using BankMore.Account.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BankMore.Account.Tests.Controllers
{
    [TestClass]
    public class AccountsControllerTests
    {
        private Mock<IMediator> _mediatorMock;
        private AccountsController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new AccountsController(_mediatorMock.Object);

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

        #region CreateAccount

        [TestMethod]
        public async Task CreateAccount_Should_Return_Ok_When_Account_IsValid()
        {
            // Arrange
            var request = new CreateAccountRequest()
            {
                Cpf = "12345678900",
                Nome = "Felipe Weber",
                Senha = "123456"
            };

            var expectedResponse = new CreateAccountResponse
            {
                NumeroConta = 1001
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateAccountCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.CreateAccount(request);


            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            
            var objectResult = (OkObjectResult)result;

            Assert.AreEqual(expectedResponse, objectResult.Value);
        }

        [TestMethod]
        public async Task CreateAccount_Should_Return_BadRequest_When_Cpf_AlreadyExists()
        {
            // Arrange
            var request = new CreateAccountRequest()
            {
                Cpf = "12345678900",
                Nome = "Felipe Weber",
                Senha = "123456"
            };

            var expectedResponse = new ErrorResponse("O CPF informado já possui uma conta vinculada.", "INVALID_DOCUMENT");

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateAccountCommand>(), It.IsAny<CancellationToken>()))
                .Throws(new InvalidDocumentException("O CPF informado já possui uma conta vinculada."));

            // Act
            var result = await _controller.CreateAccount(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            
            var objectResult = (BadRequestObjectResult)result;
            Assert.IsNotNull(objectResult);

            var errorResponse = objectResult.Value as ErrorResponse;
            Assert.IsNotNull(errorResponse);

            Assert.AreEqual(expectedResponse.Message, errorResponse.Message);
            Assert.AreEqual(expectedResponse.ErrorType, errorResponse.ErrorType);
        }

        [TestMethod]
        public async Task CreateAccount_Should_Return_BadRequest_WhenCpfInvalid()
        {
            // Arrange
            var request = new CreateAccountRequest
            {
                Cpf = "11111111111",
                Nome = "Felipe Weber",
                Senha = "123456"
            };

            var expectedResponse = new ErrorResponse("CPF inválido", "INVALID_DOCUMENT");

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateAccountCommand>(), It.IsAny<CancellationToken>()))
                .Throws(new InvalidDocumentException("CPF inválido"));

            // Act
            var result = await _controller.CreateAccount(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            
            var objectResult = (BadRequestObjectResult)result;
            Assert.IsNotNull(objectResult);

            var errorResponse = objectResult.Value as ErrorResponse;
            Assert.IsNotNull(errorResponse);

            Assert.AreEqual(expectedResponse.Message, errorResponse.Message);
            Assert.AreEqual(expectedResponse.ErrorType, errorResponse.ErrorType);
        }

        #endregion

        #region Login

        [TestMethod]
        public async Task Login_Should_ReturnOk_WhenSuccess()
        {
            // Arrange
            var request = new LoginAccountRequest
            { 
                CpfOuNumeroConta = "12345678909", 
                Senha = "123" 
            };

            var expectedResponse = new LoginResponse { 
                Token = "jwt_token" 
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<LoginAccountCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            
            var objectResult = (OkObjectResult)result;
            var okResponse = objectResult.Value as LoginResponse;

            Assert.AreEqual(expectedResponse.Token, okResponse.Token);
        }

        [TestMethod]
        public async Task Login_Should_Return_Unauthorized_When_InvalidCredentials()
        {
            // Arrange
            var request = new LoginAccountRequest 
            { 
                CpfOuNumeroConta = "99999999999", 
                Senha = "errada"
            };

            var expectedResponse = new ErrorResponse("Usuário e/ou senha inválidos", "USER_UNAUTHORIZED");

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<LoginAccountCommand>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("Usuário e/ou senha inválidos"));

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
            
            var objectResult = result as UnauthorizedObjectResult;
            Assert.IsNotNull(objectResult);

            var errorResponse = objectResult.Value as ErrorResponse;
            Assert.IsNotNull(errorResponse);

            Assert.AreEqual(expectedResponse.Message, errorResponse.Message);
            Assert.AreEqual(expectedResponse.ErrorType, errorResponse.ErrorType);
        }

        #endregion

        #region DeactivateAccount

        [TestMethod]
        public async Task Deactivate_Should_ReturnNoContent_WhenSuccess()
        {
            // Arrange
            var accountId = Guid.NewGuid().ToString();
            var senha = "123";

            _controller.ControllerContext = CreateFakeContext(accountId);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<DeactivateAccountCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Deactivate(senha);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Deactivate_Should_Return_BadRequest_When_InvalidAccount()
        {
            // Arrange
            var accountId = Guid.NewGuid().ToString();
            var senha = "123";

            var expectedResponse = new ErrorResponse("Conta inválida", "INVALID_ACCOUNT");

            _controller.ControllerContext = CreateFakeContext(accountId);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<DeactivateAccountCommand>(), It.IsAny<CancellationToken>()))
                .Throws(new InvalidAccountException("Conta inválida"));

            // Act
            var result = await _controller.Deactivate(senha);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            
            var objectResult = (BadRequestObjectResult)result;
            Assert.IsNotNull(objectResult);

            var errorResponse = objectResult.Value as ErrorResponse;
            Assert.IsNotNull(errorResponse);


            Assert.AreEqual(expectedResponse.Message, errorResponse.Message);
            Assert.AreEqual(expectedResponse.ErrorType, errorResponse.ErrorType);
        }

        #endregion

        #region MovementAccount

        [TestMethod]
        public async Task MovementAccount_Should_Return_NoContent_When_Success()
        {
            // Arrange
            var accountId = Guid.NewGuid().ToString();
            var request = new MovementAccountRequest
            {
                IdRequisicao = Guid.NewGuid(),
                NumeroConta = 123,
                Valor = 100,
                Tipo = "C"
            };

            _controller.ControllerContext = CreateFakeContext(accountId);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<MovementAccountCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.MovementAccount(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task MovementAccount_Should_Return_BadRequest_When_InvalidAccount()
        {
            // Arrange
            var accountId = Guid.NewGuid().ToString();
            var request = new MovementAccountRequest
            {
                IdRequisicao = Guid.NewGuid(),
                NumeroConta = 123,
                Valor = 100,
                Tipo = "X"
            };

            var expectedResponse = new ErrorResponse("Conta inválida", "INVALID_ACCOUNT");

            _controller.ControllerContext = CreateFakeContext(accountId);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<MovementAccountCommand>(), It.IsAny<CancellationToken>()))
                .Throws(new InvalidAccountException("Conta inválida"));

            // Act
            var result = await _controller.MovementAccount(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var objectResult = (BadRequestObjectResult)result;
            var errorResponse = objectResult.Value as ErrorResponse;

            Assert.AreEqual(expectedResponse.ErrorType, errorResponse.ErrorType);
            Assert.AreEqual(expectedResponse.Message, errorResponse.Message);
        }

        [TestMethod]
        public async Task MovementAccount_Should_Return_BadRequest_When_InactiveAccount()
        {
            // Arrange
            var accountId = Guid.NewGuid().ToString();
            var request = new MovementAccountRequest
            {
                IdRequisicao = Guid.NewGuid(),
                NumeroConta = 123,
                Valor = 100,
                Tipo = "X"
            };

            var expectedResponse = new ErrorResponse("Conta inativa", "INACTIVE_ACCOUNT");

            _controller.ControllerContext = CreateFakeContext(accountId);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<MovementAccountCommand>(), It.IsAny<CancellationToken>()))
                .Throws(new InactiveAccountException("Conta inativa"));

            // Act
            var result = await _controller.MovementAccount(request);

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
        public async Task MovementAccount_Should_Return_BadRequest_When_InvalidValue()
        {
            // Arrange
            var accountId = Guid.NewGuid().ToString();
            var request = new MovementAccountRequest
            {
                IdRequisicao = Guid.NewGuid(),
                NumeroConta = 123,
                Valor = 100,
                Tipo = "X"
            };

            var expectedResponse = new ErrorResponse("Valor inválido", "INVALID_VALUE");

            _controller.ControllerContext = CreateFakeContext(accountId);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<MovementAccountCommand>(), It.IsAny<CancellationToken>()))
                .Throws(new InvalidValueException("Valor inválido"));

            // Act
            var result = await _controller.MovementAccount(request);

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
        public async Task MovementAccount_Should_Return_BadRequest_When_InvalidType()
        {
            // Arrange
            var accountId = Guid.NewGuid().ToString();
            var request = new MovementAccountRequest
            {
                IdRequisicao = Guid.NewGuid(),
                NumeroConta = 123,
                Valor = 100,
                Tipo = "X"
            };

            var expectedResponse = new ErrorResponse("Tipo inválido", "INVALID_TYPE");


            _controller.ControllerContext = CreateFakeContext(accountId);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<MovementAccountCommand>(), It.IsAny<CancellationToken>()))
                .Throws(new InvalidTypeException("Tipo inválido"));

            // Act
            var result = await _controller.MovementAccount(request);

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
        public async Task MovementAccount_Should_Return_BadRequest_When_IdempotencyViolated()
        {
            // Arrange
            var accountId = Guid.NewGuid().ToString();
            var request = new MovementAccountRequest
            {
                IdRequisicao = Guid.NewGuid(),
                NumeroConta = 123,
                Valor = 100,
                Tipo = "X"
            };

            var expectedResponse = new ErrorResponse($"Já existe um movimento processado para o RequestId {request.IdRequisicao.ToString("d")}.", "IDEMPOTENCY_VIOLATION");

            _controller.ControllerContext = CreateFakeContext(accountId);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<MovementAccountCommand>(), It.IsAny<CancellationToken>()))
                .Throws(new IdempotencyViolationException(request.IdRequisicao.ToString("d")));

            // Act
            var result = await _controller.MovementAccount(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

            var objectResult = result as BadRequestObjectResult;
            Assert.IsNotNull(objectResult);

            var errorResponse = objectResult.Value as ErrorResponse;
            Assert.IsNotNull(errorResponse);

            Assert.AreEqual(expectedResponse.ErrorType, errorResponse.ErrorType);
            Assert.AreEqual(expectedResponse.Message, errorResponse.Message);
        }

        #endregion

        #region GetBalance

        [TestMethod]
        public async Task GetAccountBalance_Should_Return_Ok_When_Account_IsValid()
        {
            // Arrange
            int numeroConta = 1001;
            decimal saldo = 500.75m;

            var expectedResponse = new AccountBalanceResponse()
            {
                DataConsulta = DateTime.UtcNow,
                Nome = "Felipe Weber",
                NumeroConta = numeroConta,
                Saldo = saldo.ToString()
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<AccountBalanceQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetAccountBalance();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            var objectResult = result as OkObjectResult;
            Assert.IsNotNull(objectResult);

            var accountBalanceResponse = objectResult.Value as AccountBalanceResponse;
            Assert.IsNotNull(accountBalanceResponse);

            Assert.AreEqual(expectedResponse.Saldo, accountBalanceResponse.Saldo);
            Assert.AreEqual(expectedResponse.Nome, accountBalanceResponse.Nome);
            Assert.AreEqual(expectedResponse.NumeroConta, accountBalanceResponse.NumeroConta);
            Assert.AreEqual(expectedResponse.DataConsulta, accountBalanceResponse.DataConsulta);
        }

        [TestMethod]
        public async Task GetAccountBalance_Should_Return_BadRequest_When_InactiveAccount()
        {
            // Arrange
            _controller.ControllerContext = CreateFakeContext(Guid.NewGuid().ToString());

            var expectedResponse = new ErrorResponse("Conta inativa", "INACTIVE_ACCOUNT");


            _mediatorMock
                .Setup(m => m.Send(It.IsAny<AccountBalanceQuery>(), It.IsAny<CancellationToken>()))
                .Throws(new InactiveAccountException("Conta inativa"));

            // Act
            var result = await _controller.GetAccountBalance();

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

            var objectResult = result as BadRequestObjectResult;
            Assert.IsNotNull(objectResult);

            var errorResponse = objectResult.Value as ErrorResponse;
            Assert.IsNotNull(errorResponse);

            Assert.AreEqual(expectedResponse.ErrorType, errorResponse.ErrorType);
            Assert.AreEqual(expectedResponse.Message, errorResponse.Message);
        }

        #endregion

        #region Helpers

        private ControllerContext CreateFakeContext(string accountId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, accountId),
                new Claim(JwtRegisteredClaimNames.Sub, accountId)
            };

            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);

            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        #endregion
    }
}