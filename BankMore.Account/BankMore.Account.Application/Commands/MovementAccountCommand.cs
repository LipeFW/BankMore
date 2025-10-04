using MediatR;

namespace BankMore.Account.Application.Commands
{
    public class MovementAccountCommand : IRequest
    {
        public Guid IdRequisicao { get; set; }
        public int NumeroConta { get; set; }
        public string IdContaCorrente { get; set; }
        public decimal Valor { get; set; }
        public string Tipo { get; set; }

        public MovementAccountCommand(Guid requestId, int accountNumber, string accountId, decimal valor, string tipo)
        {
            IdRequisicao = requestId;
            NumeroConta = accountNumber;
            IdContaCorrente = accountId;
            Valor = valor;
            Tipo = tipo;
        }
    }
}
