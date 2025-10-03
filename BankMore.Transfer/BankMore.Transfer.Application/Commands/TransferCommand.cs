using MediatR;

namespace BankMore.Transfer.Application.Commands
{
    public class TransferCommand : IRequest
    {
        public Guid IdRequisicao { get; set; }
        public int NumeroContaDestino { get; }
        public decimal Valor { get; }
        public Guid IdContaOrigem { get; }

        public TransferCommand(Guid idRequisicao, int numeroContaDestino, decimal valor, Guid idContaOrigem)
        {
            IdRequisicao = idRequisicao;
            NumeroContaDestino = numeroContaDestino;
            Valor = valor;
            IdContaOrigem = idContaOrigem;
        }
    }
}
