namespace BankMore.Tariff.Domain.Messages
{
    public class TariffMessage
    {
        public Guid IdContaCorrente { get; set; }
        public decimal Valor { get; set; }

        public TariffMessage(Guid idContaCorrente, decimal valor)
        {
            IdContaCorrente = idContaCorrente;
            Valor = valor;
        }
    }
}
