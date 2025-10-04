namespace BankMore.Transfer.Domain.Entities
{
    public class Transferencia
    {
        public Transferencia(Guid idContaCorrenteOrigem, Guid idContaCorrenteDestino, DateTime dataTransferencia, decimal valor)
        {
            IdTransferencia = Guid.NewGuid();
            IdContaCorrenteOrigem = idContaCorrenteOrigem;
            IdContaCorrenteDestino = idContaCorrenteDestino;
            DataTransferencia = dataTransferencia;
            Valor = valor;
        }

        public Transferencia()
        {
                
        }

        public Guid IdTransferencia { get; set; }
        public Guid IdContaCorrenteOrigem { get; set; }
        public Guid IdContaCorrenteDestino { get; set; }
        public DateTime DataTransferencia { get; set; }
        public decimal Valor { get; set; }
    }
}
