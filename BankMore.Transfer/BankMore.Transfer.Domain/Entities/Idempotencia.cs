namespace BankMore.Transfer.Domain.Entities
{
    public class Idempotencia
    {
        public string Chave_Idempotencia { get; set; } = string.Empty;
        public string Requisicao { get; set; } = string.Empty;
        public string Resultado { get; set; } = string.Empty;
    }
}
