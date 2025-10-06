using BankMore.Tariff.Domain.Messages;

namespace BankMore.Tariff.Domain.Interfaces
{
    public interface ITariffMessageProducer
    {
        Task PublishAsync(TariffMessage tariffMessage);

    }
}
