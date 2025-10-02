namespace BankMore.Account.Domain.DTOs.Responses
{
    public class AccountBalanceResponse
    {
        public int NumeroConta { get; set; }
        public string Nome { get; set; } = null!;
        public DateTime DataConsulta { get; set; }
        public string Saldo { get; set; }
    }
}
