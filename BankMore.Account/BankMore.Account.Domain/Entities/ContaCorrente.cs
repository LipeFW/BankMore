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

        public ContaCorrente(string cpf, string passwordHash, string accountNumber)
        {
            IdContaCorrente = Guid.NewGuid();
            Cpf = cpf;
            Senha = passwordHash;
            Numero = accountNumber;
            Ativo = true;
        }

        public void Deactivate() => Ativo = false;
    }
}
