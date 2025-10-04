namespace BankMore.Transfer.Domain.DTOs.Requests
{
    public class MovementAccountRequest
    {
        public Guid IdRequisicao { get; set; }
        public int NumeroConta { get; set; }
        public decimal Valor { get; set; }

        private string _tipo;

        public string Tipo
        {
            get => _tipo;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    _tipo = value.Trim().Substring(0, 1).ToUpper();
                else
                    _tipo = string.Empty;
            }
        }

        public MovementAccountRequest(Guid idRequisicao, int numeroConta, decimal valor, string tipo)
        {
            IdRequisicao = idRequisicao;
            NumeroConta = numeroConta;
            Valor = valor;
            Tipo = tipo;
        }
    }
}
