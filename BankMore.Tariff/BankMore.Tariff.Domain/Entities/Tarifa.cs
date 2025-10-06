namespace BankMore.Tariff.Domain.Entities
{
    public class Tarifa
    {
        public Guid IdTarifa { get; set; } = Guid.NewGuid();
        public Guid IdContaCorrente { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataTarifacao { get; set; } = DateTime.UtcNow;

        public Tarifa(Guid idContaCorrente, decimal valor, DateTime dataTarifacao)
        {
            IdContaCorrente = idContaCorrente;
            Valor = valor;
            DataTarifacao = dataTarifacao;
        }
    }
}
