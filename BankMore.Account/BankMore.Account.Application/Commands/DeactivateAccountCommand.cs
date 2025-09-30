using MediatR;

namespace BankMore.Account.Application.Commands
{
    public class DeactivateAccountCommand : IRequest
    {
        public string AccountId { get; set; }
        public string Senha { get; set; } = string.Empty;

        public DeactivateAccountCommand(string accountNumber, string senha)
        {
            AccountId = accountNumber;
            Senha = senha;
        }
    }
}
