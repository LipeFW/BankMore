using BankMore.Account.Domain.Entities;
using BankMore.Account.Domain.Interfaces;
using Dapper;
using System.Data;
using System.Security.Principal;

namespace BankMore.Account.Infrastructure.Repositories
{
    public class MovementRepository : IMovementRepository
    {
        private readonly IDbConnection _db;

        public MovementRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task Add(Movimento movement)
        {
            var sql = @"
                INSERT INTO Movimento (""IdMovimento"", ""IdContaCorrente"", ""DataMovimento"", ""TipoMovimento"", ""Valor"")
                VALUES (:IdMovimento, :IdContaCorrente, :DataMovimento, :TipoMovimento, :Valor)";

            var parameters = new DynamicParameters();
            parameters.Add("IdMovimento", movement.IdMovimento.ToString("D"));
            parameters.Add("IdContaCorrente", movement.IdContaCorrente.ToString("D"));
            parameters.Add("DataMovimento", movement.DataMovimento);
            parameters.Add("TipoMovimento", movement.TipoMovimento);
            parameters.Add("Valor", movement.Valor);

            await _db.ExecuteAsync(sql, parameters);
        }
    }
}
