using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace UserService.Infrastructure.Repositories
{
    public abstract class BaseDapperRepository
    {
        protected readonly IConfiguration _configuration;
        
        protected BaseDapperRepository(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected async Task<T> WithConnectionAsync<T>(Func<IDbConnection, Task<T>> operation)
        {
            var connectionString = _configuration.GetConnectionString("UserServiceConnection");
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            return await operation(connection);
        }

        protected async Task WithConnectionAsync(Func<IDbConnection, Task> operation)
        {
            var connectionString = _configuration.GetConnectionString("UserServiceConnection");
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            await operation(connection);
        }
    }
}
