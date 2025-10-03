namespace BankMore.Transfer.Domain.Entities
{
    public class Transferencia
    {
        public Guid IdTransferencia { get; set; }
        public Guid IdContaCorrenteOrigem { get; set; }
        public Guid IdContaCorrenteDestino { get; set; }
        public DateTime DataTransferencia { get; set; }
        public decimal Valor { get; set; }
    }
}
