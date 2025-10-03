using Dapper;
using System.Data;

namespace BankMore.Transfer.Infrastructure.Utils
{
    public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
    {
        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            parameter.Value = value.ToString("D");
        }

        public override Guid Parse(object value)
        {
            return Guid.Parse((string)value);
        }
    }
}
