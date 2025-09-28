namespace BankMore.Account.Api.Models
{
    /// <summary>
    /// DTO usado para cadastrar uma nova conta corrente.
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// CPF do titular da conta
        /// </summary>
        public string Cpf { get; set; } = string.Empty;

        /// <summary>
        /// Senha da conta
        /// </summary>
        public string Senha { get; set; } = string.Empty;
    }
}
