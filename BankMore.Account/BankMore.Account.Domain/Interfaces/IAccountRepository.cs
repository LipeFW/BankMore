namespace BankMore.Account.Domain.Interfaces
{
    public interface IAccountRepository
    {
        Task<Entities.ContaCorrente> GetByCpf(string cpf);
        Task Add(Entities.ContaCorrente account);
    }
}
