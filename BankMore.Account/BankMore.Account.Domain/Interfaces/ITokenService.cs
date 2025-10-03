using BankMore.Account.Domain.Entities;

namespace BankMore.Account.Domain.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(ContaCorrente accountId);
    }
}
