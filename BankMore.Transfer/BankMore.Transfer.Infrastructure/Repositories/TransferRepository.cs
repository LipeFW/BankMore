using BankMore.Transfer.Domain.Entities;
using BankMore.Transfer.Domain.Interfaces;
using Dapper;
using System.Data;
using System.Security.Principal;

namespace BankMore.Transfer.Infrastructure.Repositories
{
    public class TransferRepository : ITransferRepository
    {
        private readonly IDbConnection _db;

        public TransferRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task AddAsync(Transferencia transfer)
        {
            var sql = @"INSERT INTO Transferencia (""IdTransferencia"", ""IdContaCorrenteOrigem"", ""IdContaCorrenteDestino"", ""Valor"", ""DataTransferencia"")
                    VALUES (:IdTransferencia, :IdContaCorrenteOrigem, :IdContaCorrenteDestino, :Valor, :DataMovimento)";

            var parameters = new DynamicParameters();
            parameters.Add("IdTransferencia", transfer.IdTransferencia.ToString("D"));
            parameters.Add("IdContaCorrenteOrigem", transfer.IdContaCorrenteOrigem.ToString("D"));
            parameters.Add("IdContaCorrenteDestino", transfer.IdContaCorrenteDestino.ToString("D"));
            parameters.Add("Valor", transfer.Valor);
            parameters.Add("DataTransferencia", transfer.DataTransferencia);

            await _db.ExecuteAsync(sql, parameters);
        }
    }
}
