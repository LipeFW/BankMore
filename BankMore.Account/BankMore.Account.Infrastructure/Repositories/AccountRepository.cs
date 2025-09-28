using BankMore.Account.Domain.Entities;
using BankMore.Account.Domain.Interfaces;
using Dapper;
using System.Data;

namespace BankMore.Account.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDbConnection _db;

        public AccountRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<ContaCorrente> GetByCpf(string cpf)
        {
            var sql = @"
                SELECT Id, Cpf, PasswordHash, AccountNumber, Active
                FROM ContaCorrente
                WHERE Cpf = @cpf";

            return await _db.QuerySingleOrDefaultAsync<ContaCorrente>(sql, new { cpf });
        }

        public async Task Add(ContaCorrente account)
        {
            var sql = @"
                INSERT INTO ContaCorrente (Id, Cpf, PasswordHash, AccountNumber, Active)
                VALUES (@Id, @Cpf, @PasswordHash, @AccountNumber, @Active)";
            await _db.ExecuteAsync(sql, account);
        }
    }
}
