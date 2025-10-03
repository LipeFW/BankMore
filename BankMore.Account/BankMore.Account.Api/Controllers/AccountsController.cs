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
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            try
            {
                var result = await _mediator.Send(new CreateAccountCommand(request.Cpf, request.Nome, request.Senha));

                return Ok(result);
            }
            catch (InvalidDocumentException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message, "INVALID_DOCUMENT"));
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
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
                return Unauthorized(new ErrorResponse(ex.Message, "USER_UNAUTHORIZED"));

            }
        }

        /// <summary>
        /// Responsável por inativar uma conta corrente
        /// </summary>
        /// <response code="200">Token de autenticação (JWT).</response>
        /// <param name="senha">Senha do usuário.</param>
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
                return BadRequest(new ErrorResponse(ex.Message, "INVALID_ACCOUNT"));
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
                return BadRequest(new ErrorResponse(ex.Message, "INVALID_ACCOUNT"));

            }
            catch (InactiveAccountException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message, "INACTIVE_ACCOUNT"));

            }
            catch (InvalidValueException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message, "INVALID_VALUE"));

            }
            catch (InvalidTypeException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message, "INVALID_TYPE"));

            }
        }

        /// <summary>
        /// Responsável por consultar o saldo da conta.
        /// A consulta de saldo é efetuada pelo ID da conta logada.
        /// </summary>
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("balance")]
        public async Task<IActionResult> GetAccountBalance()
        {
            try
            {
                var accountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                                HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

                var result = await _mediator.Send(new AccountBalanceQuery(new Guid(accountId)));

                return Ok(result);
            }
            catch (InvalidAccountException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message, "INVALID_ACCOUNT"));

            }
            catch (InactiveAccountException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message, "INACTIVE_ACCOUNT"));

            }
            catch (InvalidTypeException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message, "INVALID_TYPE"));

            }
        }
    }
}
