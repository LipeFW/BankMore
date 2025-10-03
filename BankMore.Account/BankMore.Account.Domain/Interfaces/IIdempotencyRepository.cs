using BankMore.Account.Domain.Entities;

namespace BankMore.Account.Domain.Interfaces
{
    public interface IIdempotencyRepository
    {
        Task AddAsync(Idempotencia idempotency);
        Task<Idempotencia> GetByRequestIdAsync(Guid requestId);
    }
}
