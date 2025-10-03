using BankMore.Account.Domain.DTOs.Responses;
using MediatR;

namespace BankMore.Account.Application.Queries
{
    public class AccountBalanceQuery : IRequest<AccountBalanceResponse>
    {
        public Guid AccountIdFromToken { get; }
        public int? NumeroConta { get; } 

        public AccountBalanceQuery(Guid accountIdFromToken)
        {
            AccountIdFromToken = accountIdFromToken;
        }
    }
}
