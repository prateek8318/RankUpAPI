using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace MasterService.Infrastructure.Repositories
{
    public abstract class BaseDapperRepository
    {
        protected readonly string _connectionString;
        private IDbConnection? _connectionField;

        protected BaseDapperRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        protected IDbConnection _connection => _connectionField ??= GetConnection();

        protected IDbConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        protected async Task EnsureOpenAsync(IDbConnection connection)
        {
            if (connection.State != ConnectionState.Open)
            {
                if (connection is DbConnection dbConnection)
                {
                    await dbConnection.OpenAsync();
                }
                else
                {
                    connection.Open();
                }
            }
        }

        protected async Task<T> WithConnectionAsync<T>(Func<IDbConnection, Task<T>> operation)
        {
            using var connection = GetConnection();
            await EnsureOpenAsync(connection);
            return await operation(connection);
        }

        protected async Task WithConnectionAsync(Func<IDbConnection, Task> operation)
        {
            using var connection = GetConnection();
            await EnsureOpenAsync(connection);
            await operation(connection);
        }
    }
}
