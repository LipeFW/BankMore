using BankMore.Account.Application.Commands;
using BankMore.Account.Domain.Interfaces;
using MediatR;
using System.Text;

namespace BankMore.Account.Application.Handlers
{
    public class RegisterAccountHandler : IRequestHandler<RegisterAccountCommand, Guid>
    {
        private readonly IAccountRepository _accountRepository;

        public RegisterAccountHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<Guid> Handle(RegisterAccountCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Cpf) || request.Cpf.Length != 11)
                throw new ArgumentException("CPF inválido");

            var existingAccount = await _accountRepository.GetByCpf(request.Cpf);

            if (existingAccount != null)
                throw new InvalidOperationException("Conta já cadastrada.");

            var passwordHash = HashPassword(request.Senha);
            var accountNumber = GenerateAccountNumber();
            var newAccount = new Domain.Entities.ContaCorrente(request.Cpf, passwordHash, accountNumber);

            await _accountRepository.Add(newAccount);
            return newAccount.Id;
        }
        private string HashPassword(string password)
        {
            // Implement a proper password hashing mechanism here
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
        }
        private string GenerateAccountNumber()
        {
            // Implement a proper account number generation logic here
            return Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }
    }
}
