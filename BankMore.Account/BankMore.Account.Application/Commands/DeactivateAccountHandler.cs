using BankMore.Account.Domain.Exceptions;
using BankMore.Account.Domain.Interfaces;
using MediatR;
using System.Text;
using BCryptHelper = BCrypt.Net.BCrypt;

namespace BankMore.Account.Application.Commands
{
    public class DeactivateAccountHandler : IRequestHandler<DeactivateAccountCommand>
    {
        private readonly IAccountRepository _repository;

        public DeactivateAccountHandler(IAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeactivateAccountCommand request, CancellationToken cancellationToken)
        {
            var account = await _repository.GetByIdAsync(request.AccountId);

            if (account is null)
                throw new InvalidAccountException("A conta informada não existe.");

            if (account == null || !BCryptHelper.Verify(request.Senha, account.Senha))
                throw new Exception("Usuário e/ou senha inválidos.");

            account.Deactivate();

            await _repository.UpdateAtivoAsync(account);
        }
    }
}
