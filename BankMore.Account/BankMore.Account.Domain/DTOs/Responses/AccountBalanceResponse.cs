namespace BankMore.Account.Domain.DTOs.Responses
{
    public class AccountBalanceResponse
    {
        public string NumeroConta { get; set; }
        public string Nome { get; set; } = null!;
        public string DataConsulta { get; set; }
        public string Saldo { get; set; }
    }
}
