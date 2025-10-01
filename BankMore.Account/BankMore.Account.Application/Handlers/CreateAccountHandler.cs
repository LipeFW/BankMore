using BankMore.Account.Application.Commands;
using BankMore.Account.Domain.Entities;
using BankMore.Account.Domain.Exceptions;
using BankMore.Account.Domain.Interfaces;
using BankMore.Account.Domain.Utils;
using MediatR;
using BCryptHelper = BCrypt.Net.BCrypt;

namespace BankMore.Account.Application.Handlers
{
    public class CreateAccountHandler : IRequestHandler<CreateAccountCommand, int>
    {
        private readonly IAccountRepository _repository;

        public CreateAccountHandler(IAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            if (!CpfUtils.IsValid(request.Cpf, out var formattedCpf))
                throw new InvalidDocumentException("CPF inválido.");

            var existingAccount = await _repository.GetByCpfAsync(request.Cpf);

            if (existingAccount != null)
                throw new InvalidOperationException("Conta já cadastrada.");

            (var senhaHash, var salt) = HashPassword(request.Senha);
            var numeroConta = GenerateAccountNumber();
            var newAccount = new ContaCorrente(formattedCpf, request.Nome, senhaHash, numeroConta, salt);

            await _repository.AddAsync(newAccount);
            return numeroConta;
        }
        private (string senhaHash, string salt) HashPassword(string senha)
        {
            string salt = BCryptHelper.GenerateSalt();
            string senhaHash = BCryptHelper.HashPassword(senha, salt);

            return (senhaHash, salt);
        }

        private int GenerateAccountNumber()
        {
            return new Random().Next(1, 999999999);
        }
    }
}
