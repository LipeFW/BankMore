using BankMore.Account.Domain.Entities;

namespace BankMore.Account.Domain.Interfaces
{
    public interface IMovementRepository
    {
        Task AddAsync(Movimento movimento);
    }
}
