namespace BankMore.Account.Api.Models.Requests
{
    public class MovementAccountDto
    {
        public Guid RequestId { get; set; }
        public int AccountNumber { get; set; }
        public decimal Valor { get; set; }
        public char Tipo { get; set; }
    }
}
