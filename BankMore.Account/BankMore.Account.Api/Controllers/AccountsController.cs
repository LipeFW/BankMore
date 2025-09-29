using BankMore.Account.Api.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using BankMore.Account.Application.Commands;
using Microsoft.AspNetCore.Authorization;
using BankMore.Account.Domain.Exceptions;

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

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var result = await _mediator.Send(new RegisterAccountCommand(dto.Cpf, dto.Senha));

                return Ok(new { numeroConta = result });
            }
            catch (InvalidDocumentException ex)
            {
                return BadRequest(new { message = ex.Message, errorType = "INVALID_DOCUMENT" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var result = await _mediator.Send(new LoginAccountCommand(dto.Cpf, dto.Senha));
                return Ok(result);
            }
            catch (Exception)
            {
                return Unauthorized(new { message = "Houve um problema na tentativa de login.", type = "USER_UNAUTHORIZED" });
            }
        }
    }
}
