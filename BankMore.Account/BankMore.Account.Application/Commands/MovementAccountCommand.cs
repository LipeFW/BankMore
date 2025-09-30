using MediatR;

namespace BankMore.Account.Application.Commands
{
    public class MovementAccountCommand : IRequest
    {
        public Guid RequestId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountId { get; set; }
        public decimal Valor { get; set; }
        public char Tipo { get; set; }

        public MovementAccountCommand(Guid requestId, string accountNumber, string accountId, decimal valor, char tipo)
        {
            RequestId = requestId;
            AccountNumber = accountNumber;
            AccountId = accountId;
            Valor = valor;
            Tipo = tipo;
        }
    }
}
