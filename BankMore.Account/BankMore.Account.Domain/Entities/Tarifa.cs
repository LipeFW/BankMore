namespace BankMore.Account.Domain.Entities
{
    public class Tarifa
    {
        public Guid IdTarifa { get; set; }
        public Guid IdContaCorrente { get; set; }
        public DateTime DataMovimento { get; set; }
        public decimal Valor { get; set; }

        // Navegação
        public ContaCorrente ContaCorrente { get; set; }
    }
}
