namespace BankMore.Account.Domain.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(string accountId);
    }
}
