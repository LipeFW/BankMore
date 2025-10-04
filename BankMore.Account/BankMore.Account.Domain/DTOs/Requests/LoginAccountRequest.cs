namespace BankMore.Account.Domain.DTOs.Requests
{
    public class LoginAccountRequest
    {
        public string CpfOuNumeroConta { get; set; }
        public string Senha { get; set; }
    }
}
