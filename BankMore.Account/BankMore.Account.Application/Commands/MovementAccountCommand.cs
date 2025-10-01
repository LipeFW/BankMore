using MediatR;

namespace BankMore.Account.Application.Commands
{
    public class MovementAccountCommand : IRequest
    {
        public Guid RequestId { get; set; }
        public int AccountNumber { get; set; }
        public string AccountId { get; set; }
        public decimal Valor { get; set; }
        public string Tipo { get; set; }

        public MovementAccountCommand(Guid requestId, int accountNumber, string accountId, decimal valor, string tipo)
        {
            RequestId = requestId;
            AccountNumber = accountNumber;
            AccountId = accountId;
            Valor = valor;
            Tipo = tipo;
        }
    }
}
