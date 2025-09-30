using BankMore.Account.Application.Commands;
using BankMore.Account.Domain.Entities;
using BankMore.Account.Domain.Exceptions;
using BankMore.Account.Domain.Interfaces;
using MediatR;
using static System.Net.Mime.MediaTypeNames;

namespace BankMore.Account.Application.Handlers
{
    public class MovementAccountHandler : IRequestHandler<MovementAccountCommand>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMovementRepository _movementRepository;

        public MovementAccountHandler(IAccountRepository accountRepository,
            IMovementRepository movementRepository)
        {
            _accountRepository = accountRepository;
            _movementRepository = movementRepository;
        }

        public async Task Handle(MovementAccountCommand request, CancellationToken cancellationToken)
        {
            ContaCorrente account = !string.IsNullOrWhiteSpace(request.AccountNumber)
                ? await _accountRepository.GetByAccountNumber(request.AccountNumber)
                : account = await _accountRepository.GetById(request.AccountId);

            if (account is null)
                throw new InvalidAccountException("Apenas contas correntes cadastradas podem receber movimentação");

            if (!account.Ativo)
                throw new InactiveAccountException("Apenas contas correntes ativas podem receber movimentação.");

            if (request.Valor < 0)
                throw new InvalidValueException("Apenas valores positivos podem ser recebidos.");

            if (request.Tipo != 'C' && request.Tipo != 'D')
                throw new InvalidTypeException("Apenas os tipos 'débito' e 'crédito' são aceitos.");

            if (request.AccountNumber != null && request.AccountId != account.IdContaCorrente.ToString() && request.Tipo == 'D')
                throw new InvalidTypeException("Apenas 'crédito' é permitido em conta diferente.");

            var movimento = new Movimento
            {
                IdContaCorrente = account.IdContaCorrente,
                DataMovimento = DateTime.UtcNow,
                TipoMovimento = request.Tipo,
                Valor = request.Valor
            };

            await _movementRepository.Add(movimento);

            return;
        }
    }
}
