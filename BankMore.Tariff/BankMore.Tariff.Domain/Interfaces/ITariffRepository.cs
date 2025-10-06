using BankMore.Tariff.Domain.Entities;

namespace BankMore.Tariff.Domain.Interfaces
{
    public interface ITariffRepository
    {
        Task AddAsync(Tarifa tarifa);
    }
}
