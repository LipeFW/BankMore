namespace BankMore.Account.Domain.Entities
{
    public class Idempotencia
    {
        public Guid ChaveIdempotencia { get; set; }
        public string Requisicao { get; set; }
        public string Resultado { get; set; }

        public Idempotencia()
        {
                
        }

        public Idempotencia(Guid chaveIdempotencia, string requisicao, string resultado)
        {
            ChaveIdempotencia = chaveIdempotencia;
            Requisicao = requisicao;
            Resultado = resultado;
        }
    }
}
