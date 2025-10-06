using BankMore.Transfer.Application.Commands;
using BankMore.Transfer.Domain.DTOs.Requests;
using BankMore.Transfer.Domain.DTOs.Responses;
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

        #region POST

        /// <summary>
        /// Responsável por efetuar transferências entre contas
        /// </summary>
        /// <param name="request">Informações da transferência</param>
        [Authorize]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)] 
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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message, "INVALID_OPERATION"));

            }
            catch (IdempotencyViolationException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message, "IDEMPOTENCY_VIOLATION"));

            }
        }
        #endregion POST
    }
}
