using BankMore.Account.Application.Commands;
using BankMore.Account.Application.Queries;
using BankMore.Account.Domain.DTOs.Requests;
using BankMore.Account.Domain.DTOs.Responses;
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
        /// <param name="request">Informações da conta corrente a ser criada</param>
        /// <returns>O número da conta criada</returns>
        [AllowAnonymous]
        [ProducesResponseType(typeof(CreateAccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            try
            {
                var result = await _mediator.Send(new CreateAccountCommand(request.Cpf, request.Nome, request.Senha));

                return Ok(new { numeroConta = result });
            }
            catch (InvalidDocumentException ex)
            {
                return BadRequest(new { message = ex.Message, errorType = "INVALID_DOCUMENT" });
            }
        }

        /// <summary>
        /// Responsável por efetuar o login na conta corrente
        /// </summary>
        /// <returns>O Token de autenticação (JWT).</returns>
        /// <response code="200">Token de autenticação (JWT).</response>
        /// <response code="401"></response>
        /// <response code="403"></response>
        /// <param name="request">Dados para efetuar o login. Objeto LoginAccountDto</param>
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginAccountRequest request)
        {
            try
            {
                var result = await _mediator.Send(new LoginAccountCommand(request.CpfOrAccountNumber, request.Senha));
                return Ok(new { token = result });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message, type = "USER_UNAUTHORIZED" });
            }
        }

        /// <summary>
        /// Responsável por inativar uma conta corrente
        /// </summary>
        /// <response code="200">Token de autenticação (JWT).</response>
        /// <param name="senha">Senha do usuário.</param>
        [Authorize]
        [ProducesResponseType(typeof(string), 204)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
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
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message, type = "INVALID_ACCOUNT" });
            }
        }

        /// <summary>
        /// Responsável por efetuar movimentações nas contas corrente
        /// </summary>
        /// <param name="request">Informações da movimentação</param>
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPost("movements")]
        public async Task<IActionResult> MovementAccount([FromBody] MovementAccountRequest request)
        {
            try
            {
                var accountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                                HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

                await _mediator.Send(new MovementAccountCommand(request.RequestId, request.AccountNumber, accountId, request.Valor, request.Tipo));
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
            catch (InvalidValueException ex)
            {
                return BadRequest(new { message = ex.Message, type = "INVALID_VALUE" });
            }
            catch (InvalidTypeException ex)
            {
                return BadRequest(new { message = ex.Message, type = "INVALID_TYPE" });
            }
        }

        /// <summary>
        /// Responsável por consultar o saldo da conta.
        /// A consulta de saldo é efetuada pelo ID da conta logada.
        /// </summary>
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [HttpGet("balance")]
        public async Task<IActionResult> GetAccountBalance()
        {
            try
            {
                var accountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                                HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

                if (accountId == null)
                {
                    throw new InvalidAccountException("Houve um problema ao verificar o saldo da conta. Por favor tente novamente.");
                }

                var result = await _mediator.Send(new AccountBalanceQuery(new Guid(accountId)));
                return Ok(result);
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
