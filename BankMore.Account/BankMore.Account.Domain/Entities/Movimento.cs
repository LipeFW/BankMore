namespace BankMore.Account.Domain.Entities
{
    public class Movimento
    {
        public Guid IdMovimento { get; set; }
        public Guid IdContaCorrente { get; set; }
        public DateTime DataMovimento { get; set; }
        public char TipoMovimento { get; set; }
        public decimal Valor { get; set; }

        public ContaCorrente ContaCorrente { get; set; }
    }
}
