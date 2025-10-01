using BankMore.Account.Application.Commands;
using BankMore.Account.Domain.Exceptions;
using BankMore.Account.Domain.Interfaces;
using MediatR;
using System.Text;

namespace BankMore.Account.Application.Handlers
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

            var loginHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.Senha));

            if (Comparer<string>.Default.Compare(account.Senha, loginHash) != 0)
                throw new Exception("Senha inválida.");

            account.Deactivate();
        }
    }
}
