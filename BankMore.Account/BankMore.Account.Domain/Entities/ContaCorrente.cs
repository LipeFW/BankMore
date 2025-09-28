namespace BankMore.Account.Domain.Entities
{
    public class ContaCorrente
    {
        public Guid Id { get; private set; }
        public string Cpf { get; private set; }
        public string PasswordHash { get; private set; }
        public string AccountNumber { get; private set; }
        public bool Active { get; private set; }

        private ContaCorrente() { }

        public ContaCorrente(string cpf, string passwordHash, string accountNumber)
        {
            Id = Guid.NewGuid();
            Cpf = cpf;
            PasswordHash = passwordHash;
            AccountNumber = accountNumber;
            Active = true;
        }

        public void Deactivate() => Active = false;
    }
}
