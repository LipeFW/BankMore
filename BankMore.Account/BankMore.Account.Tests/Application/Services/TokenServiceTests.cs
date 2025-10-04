using BankMore.Account.Application.Services;
using BankMore.Account.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;

namespace BankMore.Account.Tests.Application.Services
{
    [TestClass]
    public class TokenServiceTests
    {
        private TokenService _tokenService;

        [TestInitialize]
        public void Setup()
        {
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["Jwt:Key"]).Returns("MinhaChaveSecretaMuitoSegura12345678901");
            configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("https://meusistema.com");
            configurationMock.Setup(c => c["Jwt:Audience"]).Returns("https://meusistema.com");

            _tokenService = new TokenService(configurationMock.Object);
        }

        [TestMethod]
        public void GenerateToken_ShouldReturnValidToken()
        {
            // Arrange
            var account = new ContaCorrente
            {
                IdContaCorrente = Guid.NewGuid(),
                Numero = 12345
            };

            // Act
            var token = _tokenService.GenerateToken(account);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(token));

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Assert.IsNotNull(jwtToken);
        }

        [TestMethod]
        public void GenerateToken_ShouldContainCorrectClaims()
        {
            // Arrange
            var account = new ContaCorrente
            {
                IdContaCorrente = Guid.NewGuid(),
                Numero = 54321
            };

            // Act
            var token = _tokenService.GenerateToken(account);
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Assert claims
            Assert.AreEqual(account.IdContaCorrente.ToString(), jwtToken.Subject);
            Assert.AreEqual(account.Numero.ToString(), jwtToken.Claims.First(c => c.Type == "AccountNumber").Value);

            var jtiClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            Assert.IsTrue(Guid.TryParse(jtiClaim, out _));
        }

        [TestMethod]
        public void GenerateToken_ShouldExpireIn60Minutes()
        {
            // Arrange
            var account = new ContaCorrente
            {
                IdContaCorrente = Guid.NewGuid(),
                Numero = 11111
            };

            // Act
            var token = _tokenService.GenerateToken(account);
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Assert expiration ~60 minutos
            var diff = jwtToken.ValidTo - DateTime.UtcNow;
            Assert.IsTrue(diff.TotalMinutes <= 60 && diff.TotalMinutes > 59);
        }
    }
}