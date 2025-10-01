namespace BankMore.Account.Domain.DTOs.Requests
{
    public class LoginAccountRequest
    {
        public string CpfOrAccountNumber { get; set; }
        public string Senha { get; set; }
    }
}
