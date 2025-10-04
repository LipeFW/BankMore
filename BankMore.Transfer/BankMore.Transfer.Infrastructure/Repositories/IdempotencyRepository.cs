using BankMore.Transfer.Domain.Entities;
using BankMore.Transfer.Domain.Interfaces;
using Dapper;
using System.Data;

namespace BankMore.Transfer.Infrastructure.Repositories
{
    public class IdempotencyRepository : IIdempotencyRepository
    {
        private readonly IDbConnection _db;

        public IdempotencyRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task AddAsync(Idempotencia idempotency)
        {
            var sql = @"
                INSERT INTO Idempotencia (""Chave_Idempotencia"", ""Requisicao"", ""Resultado"")
                VALUES (:Chave_Idempotencia, :Requisicao, :Resultado)";

            var parameters = new DynamicParameters();
            parameters.Add("Chave_Idempotencia", idempotency.ChaveIdempotencia.ToString("D"));
            parameters.Add("Requisicao", idempotency.Requisicao);
            parameters.Add("Resultado", idempotency.Resultado);

            await _db.ExecuteAsync(sql, parameters);
        }

        public async Task<Idempotencia> GetByRequestIdAsync(Guid requestId)
        {
            var sql = @"SELECT *
                FROM Idempotencia
                WHERE ""Chave_Idempotencia"" = :RequestId";

            return await _db.QueryFirstOrDefaultAsync<Idempotencia>(
                sql,
                new { RequestId = requestId.ToString("d") }
            );
        }
    }
}