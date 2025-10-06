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

        public async Task<ContaCorrente> GetByCpfAsync(string cpf)
        {
            var sql = @"
                SELECT ""IdContaCorrente"", ""Cpf"", ""Nome"", ""Senha"", ""Numero"", ""Ativo""
                FROM ContaCorrente
                WHERE ""Cpf"" = :cpf";

            return await _db.QuerySingleOrDefaultAsync<ContaCorrente>(sql, new { cpf });
        }
        
        public async Task AddAsync(ContaCorrente account)
        {
            var sql = @"INSERT INTO ContaCorrente (""IdContaCorrente"", ""Cpf"", ""Nome"", ""Senha"", ""Numero"", ""Ativo"", ""Salt"")
                        VALUES (:IdContaCorrente, :Cpf, :Nome, :Senha, :Numero, :Ativo, :Salt)";

            var parameters = new DynamicParameters();
            parameters.Add("IdContaCorrente", account.IdContaCorrente.ToString("D"));
            parameters.Add("Cpf", account.Cpf);
            parameters.Add("Nome", account.Nome);
            parameters.Add("Senha", account.Senha);
            parameters.Add("Numero", account.Numero);
            parameters.Add("Ativo", account.Ativo ? 1 : 0);
            parameters.Add("Salt", account.Salt);

            await _db.ExecuteAsync(sql, parameters);
        }

        public async Task<ContaCorrente> GetByIdAsync(Guid idContaCorrente)
        {
            var sql = @"SELECT ""IdContaCorrente"", ""Cpf"", ""Nome"", ""Senha"", ""Numero"", ""Ativo""
                        FROM ContaCorrente
                        WHERE ""IdContaCorrente"" = :idContaCorrente";

            return await _db.QuerySingleOrDefaultAsync<ContaCorrente>(sql, new { idContaCorrente = idContaCorrente.ToString("d") });
        }

        public async Task<ContaCorrente> GetByIdAsync(string idContaCorrente)
        {
            var sql = @"SELECT ""IdContaCorrente"", ""Cpf"", ""Nome"", ""Senha"", ""Numero"", ""Ativo""
                        FROM ContaCorrente
                        WHERE ""IdContaCorrente"" = :idContaCorrente";

            return await _db.QuerySingleOrDefaultAsync<ContaCorrente>(sql, new { idContaCorrente });
        }

        public async Task<ContaCorrente> GetByAccountNumberAsync(int accountNumber)
        {
            var sql = @"SELECT ""IdContaCorrente"", ""Cpf"", ""Nome"", ""Senha"", ""Numero"", ""Ativo""
                        FROM ContaCorrente
                        WHERE ""Numero"" = :accountNumber";

            return await _db.QuerySingleOrDefaultAsync<ContaCorrente>(sql, new { accountNumber });
        }

        public async Task<decimal> GetSaldoAsync(string idContaCorrente)
        {
            string sql = @"SELECT 
                                NVL(SUM(CASE WHEN m.""TipoMovimento"" = 'C' THEN m.""Valor""
                                             WHEN m.""TipoMovimento"" = 'D' THEN -m.""Valor""
                                             ELSE 0 END), 0) AS ""Saldo""
                            FROM ContaCorrente c
                            LEFT JOIN Movimento m ON c.""IdContaCorrente"" = m.""IdContaCorrente""
                            WHERE c.""IdContaCorrente"" = :IdContaCorrente
                            GROUP BY c.""Numero"", c.""Nome""";

            return await _db.QuerySingleOrDefaultAsync<decimal>(
                sql,
                new { IdContaCorrente = idContaCorrente });
        }

        public async Task UpdateAtivoAsync(ContaCorrente account)
        {
            var sql = @"UPDATE ContaCorrente 
                SET ""Ativo"" = :Ativo
                WHERE ""IdContaCorrente"" = :IdContaCorrente";

            var parameters = new DynamicParameters();
            parameters.Add("IdContaCorrente", account.IdContaCorrente.ToString("D"));
            parameters.Add("Ativo", account.Ativo? 1 : 0);

            await _db.ExecuteAsync(sql, parameters);
        }
    }
}
