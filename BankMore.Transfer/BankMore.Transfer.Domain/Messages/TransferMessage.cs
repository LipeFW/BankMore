namespace BankMore.Transfer.Domain.DTOs.Events
{
    public class TransferMessage
    {
        public Guid IdRequisicao { get; set; }
        public Guid IdContaCorrente { get; set; }
        public decimal Valor { get; set; }

        public TransferMessage(Guid idRequisicao, Guid idContaCorrente, decimal valor)
        {
            IdRequisicao = idRequisicao;
            IdContaCorrente = idContaCorrente;
            Valor = valor;
        }
    }
}
