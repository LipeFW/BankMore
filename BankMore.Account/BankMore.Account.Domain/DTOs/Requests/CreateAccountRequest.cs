namespace BankMore.Account.Domain.DTOs.Requests
{
    /// <summary>
    /// DTO usado para cadastrar uma nova conta corrente.
    /// </summary>
    public class CreateAccountRequest
    {
        /// <summary>
        /// CPF do titular da conta corrente
        /// </summary>
        public string Cpf { get; set; } = string.Empty;

        /// <summary>
        /// Senha da conta
        /// </summary>
        public string Senha { get; set; } = string.Empty;

        /// <summary>
        /// Nome do titular da conta corrente
        /// </summary>
        public string Nome { get; set; } = string.Empty;
    }
}
