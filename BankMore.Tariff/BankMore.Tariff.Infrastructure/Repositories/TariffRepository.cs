using BankMore.Tariff.Domain.Entities;
using BankMore.Tariff.Domain.Interfaces;
using Dapper;
using System.Data;

namespace BankMore.Tariff.Infrastructure.Repositories
{
    public class TariffRepository : ITariffRepository
    {
        private readonly IDbConnection _db;

        public TariffRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task AddAsync(Tarifa tarifa)
        {
            var sql = @"INSERT INTO Tarifa (""IdTarifa"", ""IdContaCorrente"", ""Valor"", ""DataTarifacao"")
                    VALUES (:IdTarifa, :IdContaCorrente, :Valor, :DataTarifacao)";

            var parameters = new DynamicParameters();
            parameters.Add("IdTarifa", tarifa.IdTarifa.ToString("D"));
            parameters.Add("IdContaCorrente", tarifa.IdContaCorrente.ToString("D"));
            parameters.Add("Valor", tarifa.Valor);
            parameters.Add("DataTarifacao", tarifa.DataTarifacao);

            await _db.ExecuteAsync(sql, parameters);
        }
    }
}
