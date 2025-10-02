namespace BankMore.Transfer.Domain.DTOs.Requests
{
    public class TransferRequest
    {
        public Guid RequestId { get; set; }
        public string NumeroContaDestino { get; set; }
        public decimal Valor { get; set; }
    }
}
