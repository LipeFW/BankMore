using BankMore.Account.Domain.Entities;

namespace BankMore.Account.Domain.Models
{
    public class Tarifa
    {
        public Guid IdTarifa { get; set; }
        public Guid IdContaCorrente { get; set; }
        public DateTime DataMovimento { get; set; }
        public decimal Valor { get; set; }
    }
}
