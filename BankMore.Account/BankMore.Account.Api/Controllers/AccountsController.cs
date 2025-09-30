using BankMore.Account.Api.Models.Requests;
using BankMore.Account.Application.Commands;
using BankMore.Account.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BankMore.Account.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Responsável pela criação de uma nova conta corrente
        /// </summary>
        /// <returns>O número da conta que foi criada</returns>
        /// <response code="200">O número da conta que foi criadas</response>
        /// <param name="dto">Infos da conta corrente à ser criada. Objeto CreateAccountDto</param>
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), 200)]
        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto dto)
        {
            try
            {
                var result = await _mediator.Send(new RegisterAccountCommand(dto.Cpf, dto.Nome, dto.Senha));

                return Ok(new { numeroConta = result });
            }
            catch (InvalidDocumentException ex)
            {
                return BadRequest(new { message = ex.Message, errorType = "INVALID_DOCUMENT" });
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginAccountDto dto)
        {
            try
            {
                var result = await _mediator.Send(new LoginAccountCommand(dto.CpfOrAccountNumber, dto.Senha));
                return Ok(new { token = result });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Usuário e/ou senha inválidos", type = "USER_UNAUTHORIZED" });
            }
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Deactivate([FromBody] string senha)
        {
            var accountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            try
            {
                await _mediator.Send(new DeactivateAccountCommand(accountId, senha));
                return NoContent();
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Houve um problema ao inativar a conta.", type = "INVALID_ACCOUNT" });
            }
        }

        [Authorize]
        [HttpPost("movements")]
        public async Task<IActionResult> GetAccountMovements([FromBody] MovementAccountDto dto)
        {
            try
            {
                var accountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                                HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

                await _mediator.Send(new MovementAccountCommand(dto.RequestId, dto.AccountNumber, accountId, dto.Valor, dto.Tipo));
                return NoContent();
            }
            catch (InvalidAccountException ex)
            {
                return BadRequest(new { message = ex.Message, type = "INVALID_ACCOUNT" });
            }
            catch (InactiveAccountException ex)
            {
                return BadRequest(new { message = ex.Message, type = "INACTIVE_ACCOUNT" });
            }
            catch (InvalidTypeException ex)
            {
                return BadRequest(new { message = ex.Message, type = "INVALID_TYPE" });
            }
        }
    }
}
