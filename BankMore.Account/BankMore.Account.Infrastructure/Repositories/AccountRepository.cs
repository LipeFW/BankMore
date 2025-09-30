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
                SELECT IdContaCorrente, Cpf, Nome, Senha, Numero, Ativo
                FROM ContaCorrente
                WHERE Cpf = @cpf";

            return await _db.QuerySingleOrDefaultAsync<ContaCorrente>(sql, new { cpf });
        }

        public async Task Add(ContaCorrente account)
        {
            var sql = @"
                INSERT INTO ContaCorrente (IdContaCorrente, Cpf, Nome, Senha, Numero, Ativo)
                VALUES (@IdContaCorrente, @Cpf, @Nome, @Senha, @Numero, @Ativo)";
            await _db.ExecuteAsync(sql, account);
        }

        public async Task<ContaCorrente> GetByOrAccountNumberCpf(string cpfOrAccontNumber)
        {
            var sql = @"
                SELECT IdContaCorrente, Cpf, Nome, Senha, Numero, Ativo
                FROM ContaCorrente
                WHERE Cpf = @cpfOrAccontNumber
                OR Numero = @cpfOrAccontNumber";

            return await _db.QuerySingleOrDefaultAsync<ContaCorrente>(sql, new { cpfOrAccontNumber });
        }

        public async Task<ContaCorrente> GetById(string id)
        {
            var sql = @"
                SELECT IdContaCorrente, Cpf, Nome, Senha, Numero, Ativo
                FROM ContaCorrente
                WHERE IdContaCorrente = @id";

            return await _db.QuerySingleOrDefaultAsync<ContaCorrente>(sql, new { id });
        }

        public async Task<ContaCorrente> GetByAccountNumber(string accountNumber)
        {
            var sql = @"
                SELECT IdContaCorrente, Cpf, Nome, Senha, Numero, Ativo
                FROM ContaCorrente
                WHERE Numero = @accountNumber";

            return await _db.QuerySingleOrDefaultAsync<ContaCorrente>(sql, new { accountNumber });
        }
    }
}
