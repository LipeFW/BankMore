namespace BankMore.Account.Domain.DTOs.Requests
{
    public class MovementAccountRequest
    {
        public Guid RequestId { get; set; }
        public int AccountNumber { get; set; }
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
    }
}
