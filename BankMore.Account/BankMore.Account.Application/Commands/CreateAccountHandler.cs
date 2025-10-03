using BankMore.Account.Domain.DTOs.Responses;
using BankMore.Account.Domain.Entities;
using BankMore.Account.Domain.Exceptions;
using BankMore.Account.Domain.Interfaces;
using BankMore.Account.Domain.Utils;
using MediatR;
using BCryptHelper = BCrypt.Net.BCrypt;

namespace BankMore.Account.Application.Commands
{
    public class CreateAccountHandler : IRequestHandler<CreateAccountCommand, CreateAccountResponse>
    {
        private readonly IAccountRepository _repository;

        public CreateAccountHandler(IAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<CreateAccountResponse> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
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

            return new CreateAccountResponse
            {
                NumeroConta = newAccount.Numero
            };
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
