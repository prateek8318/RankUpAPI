using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace SubscriptionService.Infrastructure.Repositories
{
    public abstract class BaseDapperRepository
    {
        protected readonly string _connectionString;
        
        protected BaseDapperRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        protected async Task<T> WithConnectionAsync<T>(Func<IDbConnection, Task<T>> operation)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return await operation(connection);
        }

        protected async Task WithConnectionAsync(Func<IDbConnection, Task> operation)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await operation(connection);
        }

        protected async Task<T> WithTransactionAsync<T>(Func<IDbConnection, IDbTransaction, Task<T>> operation)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                var result = await operation(connection, transaction);
                transaction.Commit();
                return result;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        protected async Task WithTransactionAsync(Func<IDbConnection, IDbTransaction, Task> operation)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                await operation(connection, transaction);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
