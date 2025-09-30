using MediatR;

namespace BankMore.Account.Application.Commands
{
    /// <summary>
    /// Comando para registrar uma nova conta corrente.
    /// </summary>
    public class RegisterAccountCommand : IRequest<string>
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Cpf { get; set; }
        public string Senha { get; set; }

        public RegisterAccountCommand(string cpf, string nome,  string senha)
        {
            Cpf = cpf;
            Nome = nome;
            Senha = senha;
        }
    }
}
