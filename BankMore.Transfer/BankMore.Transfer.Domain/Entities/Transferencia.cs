namespace BankMore.Transfer.Domain.Entities
{
    public class Transferencia
    {
        public Guid IdTransferencia { get; set; }
        public Guid IdContaCorrenteOrigem { get; set; }
        public Guid IdContaCorrenteDestino { get; set; }
        public DateTime DataMovimento { get; set; }
        public decimal Valor { get; set; }
    }
}
