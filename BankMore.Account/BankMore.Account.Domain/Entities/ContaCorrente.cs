namespace BankMore.Account.Domain.Entities
{
    public class ContaCorrente
    {
        public Guid IdContaCorrente { get; private set; }
        public string Cpf { get; private set; }
        public string Nome { get; private set; }
        public string Senha { get; private set; }
        public string Numero { get; private set; }
        public bool Ativo { get; private set; }

        public ICollection<Movimento> Movimentos { get; set; } = new List<Movimento>();
        public ICollection<Tarifa> Tarifas { get; set; } = new List<Tarifa>();

        public ContaCorrente()
        {

        }

        public ContaCorrente(string cpf, string nome, string senha, string numero )
        {
            IdContaCorrente = Guid.NewGuid();
            Cpf = cpf;
            Nome = nome;
            Senha = senha;
            Numero = numero;
            Ativo = true;
        }

        public void Deactivate() => Ativo = false;
    }
}
