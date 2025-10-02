using BankMore.Account.Domain.Entities;
using BankMore.Account.Domain.Interfaces;
using BankMore.Account.Domain.Utils;
using MediatR;
using System.Security.Principal;
using BCryptHelper = BCrypt.Net.BCrypt;

namespace BankMore.Account.Application.Commands
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
            if (string.IsNullOrWhiteSpace(request.CpfOrAccountNumber))
                throw new Exception("CPF ou número da conta devem ser informados.");

            bool isNumeroConta = int.TryParse(request.CpfOrAccountNumber, out int numeroConta);

            ContaCorrente account;

            if (isNumeroConta)
                account = await _repository.GetByAccountNumberAsync(numeroConta);
            else if(CpfUtils.IsValid(request.CpfOrAccountNumber, out var cpf))
                account = await _repository.GetByCpfAsync(cpf);
            else
                throw new Exception("Usuário e/ou senha inválidos.");

            if (account == null || !BCryptHelper.Verify(request.Senha, account.Senha))
                throw new Exception("Usuário e/ou senha inválidos.");

            return _jwtService.GenerateToken(account.IdContaCorrente.ToString());
        }
    }
}
