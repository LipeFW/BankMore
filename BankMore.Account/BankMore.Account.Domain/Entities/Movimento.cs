using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankMore.Account.Domain.Entities
{
    public class Movimento
    {
        public int IdMovimento { get; set; }
        public int IdContaCorrente { get; set; }
        public DateTime DataMovimento { get; set; }
        public string TipoMovimento { get; set; }

        public ContaCorrente ContaCorrente { get; set; }
    }
}
