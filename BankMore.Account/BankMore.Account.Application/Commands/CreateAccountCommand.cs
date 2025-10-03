using BankMore.Account.Domain.DTOs.Responses;
using MediatR;

namespace BankMore.Account.Application.Commands
{
    /// <summary>
    /// Comando para registrar uma nova conta corrente.
    /// </summary>
    public class CreateAccountCommand : IRequest<CreateAccountResponse>
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Cpf { get; set; }
        public string Senha { get; set; }

        public CreateAccountCommand(string cpf, string nome,  string senha)
        {
            Cpf = cpf;
            Nome = nome;
            Senha = senha;
        }
    }
}
