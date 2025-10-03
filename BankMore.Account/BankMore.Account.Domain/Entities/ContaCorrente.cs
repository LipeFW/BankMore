using BankMore.Account.Domain.Models;

namespace BankMore.Account.Domain.Entities
{
    public class ContaCorrente
    {
        public Guid IdContaCorrente { get; set; }
        public int Numero { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public bool Ativo { get; set; }
        public string Senha { get; set; }
        public string Salt { get; set; }

        public ICollection<Movimento> Movimentos { get; set; } = new List<Movimento>();

        public ContaCorrente()
        {
                
        }

        public ContaCorrente(string cpf, string nome, string senha, int numero, string salt )
        {
            IdContaCorrente = Guid.NewGuid();
            Numero = numero;
            Nome = nome;
            Cpf = cpf;
            Ativo = true;
            Senha = senha;
            Salt = salt;
        }

        public void Deactivate() => Ativo = false;
    }
}
