using BankMore.Account.Application.Commands;
using BankMore.Account.Domain.Exceptions;
using BankMore.Account.Domain.Interfaces;
using BankMore.Account.Domain.Utils;
using MediatR;
using System.Text;

namespace BankMore.Account.Application.Handlers
{
    public class RegisterAccountHandler : IRequestHandler<RegisterAccountCommand, string>
    {
        private readonly IAccountRepository _accountRepository;

        public RegisterAccountHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<string> Handle(RegisterAccountCommand request, CancellationToken cancellationToken)
        {
            if (!CpfUtils.IsValid(request.Cpf))
                throw new InvalidDocumentException("CPF inválido.");

            var existingAccount = await _accountRepository.GetByCpf(request.Cpf);

            if (existingAccount != null)
                throw new InvalidOperationException("Conta já cadastrada.");

            var passwordHash = HashPassword(request.Senha);
            var accountNumber = GenerateAccountNumber();
            var newAccount = new Domain.Entities.ContaCorrente(request.Cpf, passwordHash, accountNumber);

            await _accountRepository.Add(newAccount);
            return accountNumber;
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
