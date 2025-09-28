using Dapper;
using System.Data;

namespace BankMore.Account.Infrastructure.Utils
{
    public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
    {
        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            // Salva Guid como string no SQLite
            parameter.Value = value.ToString();
        }

        public override Guid Parse(object value)
        {
            // Lê string e converte para Guid
            return Guid.Parse(value.ToString());
        }
    }
}
