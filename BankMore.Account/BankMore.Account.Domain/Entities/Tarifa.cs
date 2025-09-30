namespace BankMore.Account.Domain.Entities
{
    public class Tarifa
    {
        public int IdTarifa { get; set; }
        public int IdContaCorrente { get; set; }
        public DateTime DataMovimento { get; set; }
        public decimal Valor { get; set; }

        // Navegação
        public ContaCorrente ContaCorrente { get; set; }
    }
}
