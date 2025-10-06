using BankMore.Transfer.Domain.DTOs.Events;

namespace BankMore.Transfer.Domain.Interfaces
{
    public interface ITransferMessageProducer
    {
        Task PublishAsync(TransferMessage transferEvent);
    }
}
