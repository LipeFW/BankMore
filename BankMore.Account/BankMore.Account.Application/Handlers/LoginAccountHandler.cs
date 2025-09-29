using BankMore.Account.Application.Commands;
using BankMore.Account.Domain.Interfaces;
using MediatR;
using System.Text;

namespace BankMore.Account.Application.Handlers
{
    public class LoginAccountHandler : IRequestHandler<LoginAccountCommand, string>
    {
        private readonly IAccountRepository _repository;
        private readonly ITokenService _jwtService;

        public LoginAccountHandler(IAccountRepository repository, ITokenService jwtService)
        {
            _repository = repository;
            _jwtService = jwtService;
        }

        public async Task<string> Handle(LoginAccountCommand request, CancellationToken cancellationToken)
        {
            var account = await _repository.GetByOrAccountNumberCpf(request.CpfOrAccountNumber);

            if (account == null)
                throw new InvalidOperationException("Usuário não encontrado");

            var loginHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.Senha));

            if (Comparer<string>.Default.Compare(account.PasswordHash, loginHash) != 0)
                throw new ArgumentException("Senha inválida");

            return _jwtService.GenerateToken(account.Id.ToString());
        }
    }
}
