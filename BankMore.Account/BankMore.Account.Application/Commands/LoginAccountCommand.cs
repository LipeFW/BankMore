using MediatR;

namespace BankMore.Account.Application.Commands
{
    public class LoginAccountCommand : IRequest<string>
    {
        public string CpfOrAccountNumber { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;

        public LoginAccountCommand(string cpfOrAccountNumber, string senha)
        {
            CpfOrAccountNumber = cpfOrAccountNumber;
            Senha = senha;
        }
    }
}
