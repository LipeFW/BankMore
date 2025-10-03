using BankMore.Transfer.Domain.Entities;

namespace BankMore.Transfer.Domain.Interfaces
{
    public interface IIdempotencyRepository
    {
        Task AddAsync(Idempotencia idempotency);
        Task<Idempotencia> GetByRequestIdAsync(Guid requestId);
    }
}
