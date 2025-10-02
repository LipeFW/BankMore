using BankMore.Transfer.Application.Commands;
using BankMore.Transfer.Domain.DTOs.Requests;
using BankMore.Transfer.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BankMore.Transfer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransfersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransfersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Responsável por efetuar transferências entre contas
        /// </summary>
        /// <param name="request">Informações da transferência</param>
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)] 
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]TransferRequest request)
        {
            try
            {
                var accountId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                                HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

                await _mediator.Send(new TransferCommand(request.RequestId, request.NumeroContaDestino, request.Valor, Guid.Parse(accountId)));

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
        }
    }
}
