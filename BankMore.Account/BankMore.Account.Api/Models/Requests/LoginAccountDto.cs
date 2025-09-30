namespace BankMore.Account.Api.Models.Requests
{
    public class LoginAccountDto
    {
        public string CpfOrAccountNumber { get; set; }
        public string Senha { get; set; }
    }
}
