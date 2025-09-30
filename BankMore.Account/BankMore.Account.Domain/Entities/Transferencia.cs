namespace BankMore.Account.Domain.Entities
{
    public class Transferencia
    {
        public int IdTransferencia { get; set; }
        public int IdContaCorrenteOrigem { get; set; }
        public int IdContaCorrenteDestino { get; set; }
        public DateTime DataMovimento { get; set; }
        public decimal Valor { get; set; }
    }
}
