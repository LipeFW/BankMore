namespace BankMore.Account.Domain.Entities
{
    public class Movimento
    {
        public Guid IdMovimento { get; set; }
        public Guid IdContaCorrente { get; set; }
        public DateTime DataMovimento { get; set; }
        public string TipoMovimento { get; set; }
        public decimal Valor { get; set; }

        public ContaCorrente ContaCorrente { get; set; }

        public Movimento()
        {

        }

        public Movimento(Guid idContaCorrente, string tipoMovimento, decimal valor)
        {
            IdMovimento = Guid.NewGuid();
            IdContaCorrente = idContaCorrente;
            DataMovimento = DateTime.UtcNow;
            TipoMovimento = tipoMovimento;
            Valor = valor;
        }
    }
}
