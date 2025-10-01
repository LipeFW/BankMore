using BankMore.Account.Domain.DTOs.Responses;
using BankMore.Account.Domain.Exceptions;
using BankMore.Account.Domain.Interfaces;
using MediatR;

namespace BankMore.Account.Application.Queries
{
    public class AccountBalanceHandler : IRequestHandler<AccountBalanceQuery, AccountBalanceResponse>
    {
        private readonly IAccountRepository _repository;

        public AccountBalanceHandler(IAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<AccountBalanceResponse> Handle(AccountBalanceQuery request, CancellationToken cancellationToken)
        {
            var conta = await _repository.GetByIdAsync(request.AccountIdFromToken.ToString("d"));

            if (conta == null)
                throw new InvalidAccountException("Apenas contas correntes cadastradas podem consultar o saldo.");

            if (!conta.Ativo)
                throw new InactiveAccountException("Apenas contas correntes ativas podem consultar o saldo.");

            var saldo = await _repository.GetSaldoAsync(conta.IdContaCorrente.ToString("d"));

            return new AccountBalanceResponse
            {
                NumeroConta = conta.Numero,
                Nome = conta.Nome,
                DataConsulta = DateTime.UtcNow,
                Saldo = saldo
            };
        }
    }
}
