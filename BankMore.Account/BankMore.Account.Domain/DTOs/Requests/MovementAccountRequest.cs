namespace BankMore.Account.Domain.DTOs.Requests
{
    public class MovementAccountRequest
    {
        public Guid RequestId { get; set; }
        public int AccountNumber { get; set; }
        public decimal Valor { get; set; }
        public string Tipo { get; set; }
    }
}
