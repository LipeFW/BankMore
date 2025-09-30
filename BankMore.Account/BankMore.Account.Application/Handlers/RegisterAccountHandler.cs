using BankMore.Account.Application.Commands;
using BankMore.Account.Domain.Entities;
using BankMore.Account.Domain.Exceptions;
using BankMore.Account.Domain.Interfaces;
using BankMore.Account.Domain.Utils;
using MediatR;
using System.Text;

namespace BankMore.Account.Application.Handlers
{
    public class RegisterAccountHandler : IRequestHandler<RegisterAccountCommand, string>
    {
        private readonly IAccountRepository _repository;

        public RegisterAccountHandler(IAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> Handle(RegisterAccountCommand request, CancellationToken cancellationToken)
        {
            if (!CpfUtils.IsValid(request.Cpf))
                throw new InvalidDocumentException("CPF inválido.");

            var existingAccount = await _repository.GetByCpf(request.Cpf);

            if (existingAccount != null)
                throw new InvalidOperationException("Conta já cadastrada.");

            var senhaHash = HashPassword(request.Senha);
            var numeroConta = GenerateAccountNumber();
            var newAccount = new ContaCorrente(request.Cpf, request.Nome, senhaHash, numeroConta);

            await _repository.Add(newAccount);
            return numeroConta;
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
