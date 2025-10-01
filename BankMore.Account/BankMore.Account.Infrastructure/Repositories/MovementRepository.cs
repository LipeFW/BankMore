using BankMore.Account.Domain.Entities;
using BankMore.Account.Domain.Interfaces;
using Dapper;
using System.Data;

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
                INSERT INTO Movimento (IdMovimento, IdContaCorrente, DataMovimento, TipoMovimento, Valor)
                VALUES (:IdMovimento, :IdContaCorrente, :DataMovimento, :TipoMovimento, :Valor)";
            await _db.ExecuteAsync(sql, movement);
        }
    }
}
