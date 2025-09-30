using BankMore.Account.Domain.Entities;

namespace BankMore.Account.Domain.Interfaces
{
    public interface IAccountRepository
    {
        Task<ContaCorrente> GetByCpf(string cpf);
        Task Add(ContaCorrente account);
        Task<ContaCorrente> GetByOrAccountNumberCpf(string cpfOrAccontNumber);
        Task<ContaCorrente> GetById(string id);
        Task<ContaCorrente> GetByAccountNumber(string accountNumber);
    }
}
