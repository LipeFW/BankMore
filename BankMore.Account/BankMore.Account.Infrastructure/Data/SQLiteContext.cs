using Microsoft.Data.Sqlite;

namespace BankMore.Account.Infrastructure.Data
{
    public class SQLiteContext
    {
        private readonly string _connectionString;

        public SQLiteContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public System.Data.IDbConnection CreateConnection()
        {
            return new SqliteConnection(_connectionString);
        }
    }
}
