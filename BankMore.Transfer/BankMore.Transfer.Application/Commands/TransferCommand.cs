using MediatR;

namespace BankMore.Transfer.Application.Commands
{
    public class TransferCommand : IRequest
    {
        public Guid IdRequisicao { get; }
        public int NumeroContaDestino { get; }
        public decimal Valor { get; }
        public Guid ContaOrigemId { get; }

        public TransferCommand(Guid idRequisicao, int numeroContaDestino, decimal valor, Guid contaOrigemId)
        {
            IdRequisicao = idRequisicao;
            NumeroContaDestino = numeroContaDestino;
            Valor = valor;
            ContaOrigemId = contaOrigemId;
        }
    }
}
