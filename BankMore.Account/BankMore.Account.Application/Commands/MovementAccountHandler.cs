using BankMore.Account.Domain.Entities;
using BankMore.Account.Domain.Exceptions;
using BankMore.Account.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace BankMore.Account.Application.Commands
{
    public class MovementAccountHandler : IRequestHandler<MovementAccountCommand>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountRepository _accountRepository;
        private readonly IMovementRepository _movementRepository;
        private readonly IIdempotencyRepository _idempotencyRepository;

        public MovementAccountHandler(IHttpContextAccessor httpContextAccessor,
            IAccountRepository accountRepository,
            IMovementRepository movementRepository,
            IIdempotencyRepository idempotencyRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _accountRepository = accountRepository;
            _movementRepository = movementRepository;
            _idempotencyRepository = idempotencyRepository;
        }

        public async Task Handle(MovementAccountCommand command, CancellationToken cancellationToken)
        {
            var movementExists = await _idempotencyRepository.GetByRequestIdAsync(command.RequestId);

            if (movementExists != null)
            {
                throw new IdempotencyViolationException(command.RequestId.ToString("d"));
            }

            ContaCorrente account = command.AccountNumber != 0
                ? await _accountRepository.GetByAccountNumberAsync(command.AccountNumber)
                : await _accountRepository.GetByIdAsync(command.AccountId);

            _httpContextAccessor.HttpContext.Response.Headers.Append("AccountId", account.IdContaCorrente.ToString());

            if (account is null)
                throw new InvalidAccountException("Apenas contas correntes cadastradas podem receber movimentação");

            if (!account.Ativo)
                throw new InactiveAccountException("Apenas contas correntes ativas podem receber movimentação.");

            if (command.Valor < 0)
                throw new InvalidValueException("Apenas valores positivos podem ser recebidos.");

            if (command.Tipo != "C" && command.Tipo != "D")
                throw new InvalidTypeException("Apenas os tipos 'débito' e 'crédito' são aceitos.");

            if (command.AccountNumber != 0 && command.AccountId != account.IdContaCorrente.ToString() && command.Tipo == "D")
                throw new InvalidTypeException("Apenas 'crédito' é permitido em conta diferente.");

            var movement = new Movimento(account.IdContaCorrente, command.Tipo, command.Valor);

            await _movementRepository.AddAsync(movement);

            var idempotency = new Idempotencia(command.RequestId, JsonSerializer.Serialize(command), JsonSerializer.Serialize(movement));

            await _idempotencyRepository.AddAsync(idempotency);

            return;
        }
    }
}
