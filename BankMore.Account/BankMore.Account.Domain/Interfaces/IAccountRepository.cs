using BankMore.Account.Domain.Entities;

namespace BankMore.Account.Domain.Interfaces
{
    public interface IAccountRepository
    {
        Task AddAsync(ContaCorrente account);
        Task<ContaCorrente> GetByCpfAsync(string cpf);
        Task<ContaCorrente> GetByIdAsync(Guid idContaCorrente);
        Task<ContaCorrente> GetByIdAsync(string idContaCorrente);
        Task<ContaCorrente> GetByAccountNumberAsync(int accountNumber);
        Task<decimal> GetSaldoAsync(string idContaCorrente);
        Task UpdateAtivoAsync(ContaCorrente account);
    }
}
