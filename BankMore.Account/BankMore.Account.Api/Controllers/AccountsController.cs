using BankMore.Account.Api.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using BankMore.Account.Application.Commands;

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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (string.IsNullOrEmpty(dto.Cpf)) 
                return BadRequest(new { message = "CPF inválido", errorType = "INVALID_DOCUMENT" });

            var numero = await _mediator.Send(new RegisterAccountCommand(dto.Cpf, dto.Senha));

            return Created(string.Empty, new { numeroConta = numero });
        }
    }
}
