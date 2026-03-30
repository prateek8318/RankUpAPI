using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace MasterService.Infrastructure.Repositories
{
    public abstract class BaseDapperRepository
    {
        protected readonly string _connectionString;
        protected readonly ILogger? _logger;

        protected BaseDapperRepository(string connectionString, ILogger? logger = null)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _logger = logger;
        }

        protected IDbConnection GetConnection()
        {
            var connection = new SqlConnection(_connectionString);
            // Set QUOTED_IDENTIFIER ON for this connection
            connection.StatisticsEnabled = false;
            return connection;
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

        protected async Task<T> WithConnectionAsync<T>(Func<IDbConnection, Task<T>> operation, [CallerMemberName] string operationName = "")
        {
            using var connection = GetConnection();
            try
            {
                await EnsureOpenAsync(connection);
                _logger?.LogDebug("Executing database operation {Repository}.{Operation}", GetType().Name, operationName);
                return await operation(connection);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Database operation failed in {Repository}.{Operation}", GetType().Name, operationName);
                throw;
            }
        }

        protected async Task<T> WithConnectionTransactionAsync<T>(Func<IDbConnection, IDbTransaction, Task<T>> operation, [CallerMemberName] string operationName = "")
        {
            return await WithTransactionAsync(operation, operationName);
        }

        protected async Task WithConnectionAsync(Func<IDbConnection, Task> operation, [CallerMemberName] string operationName = "")
        {
            using var connection = GetConnection();
            try
            {
                await EnsureOpenAsync(connection);
                _logger?.LogDebug("Executing database operation {Repository}.{Operation}", GetType().Name, operationName);
                await operation(connection);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Database operation failed in {Repository}.{Operation}", GetType().Name, operationName);
                throw;
            }
        }

        protected async Task<T> WithTransactionAsync<T>(Func<IDbConnection, IDbTransaction, Task<T>> operation, [CallerMemberName] string operationName = "")
        {
            using var connection = GetConnection();
            try
            {
                await EnsureOpenAsync(connection);
                using var transaction = connection.BeginTransaction();
                try
                {
                    _logger?.LogDebug("Executing transaction {Repository}.{Operation}", GetType().Name, operationName);
                    var result = await operation(connection, transaction);
                    transaction.Commit();
                    _logger?.LogDebug("Transaction committed {Repository}.{Operation}", GetType().Name, operationName);
                    return result;
                }
                catch
                {
                    transaction.Rollback();
                    _logger?.LogError("Transaction rolled back {Repository}.{Operation}", GetType().Name, operationName);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Transaction failed in {Repository}.{Operation}", GetType().Name, operationName);
                throw;
            }
        }

        protected async Task WithTransactionAsync(Func<IDbConnection, IDbTransaction, Task> operation, [CallerMemberName] string operationName = "")
        {
            using var connection = GetConnection();
            try
            {
                await EnsureOpenAsync(connection);
                using var transaction = connection.BeginTransaction();
                try
                {
                    _logger?.LogDebug("Executing transaction {Repository}.{Operation}", GetType().Name, operationName);
                    await operation(connection, transaction);
                    transaction.Commit();
                    _logger?.LogDebug("Transaction committed {Repository}.{Operation}", GetType().Name, operationName);
                }
                catch
                {
                    transaction.Rollback();
                    _logger?.LogError("Transaction rolled back {Repository}.{Operation}", GetType().Name, operationName);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Transaction failed in {Repository}.{Operation}", GetType().Name, operationName);
                throw;
            }
        }
    }
}
