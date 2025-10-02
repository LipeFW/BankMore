using BankMore.Transfer.Domain.Entities;

namespace BankMore.Transfer.Domain.Interfaces
{
    public interface ITransferRepository
    {
        Task AddAsync(Transferencia transferencia);
    }
}
