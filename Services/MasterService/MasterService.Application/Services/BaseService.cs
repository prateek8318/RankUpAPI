using MasterService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.Common;

namespace MasterService.Application.Services
{
    public abstract class BaseService
    {
        protected readonly ILogger _logger;

        protected BaseService(ILogger logger)
        {
            _logger = logger;
        }

        protected async Task<T> ExecuteInTransactionAsync<T>(
            object repository,
            Func<IDbConnection, IDbTransaction, Task<T>> operation,
            string operationName)
        {
            // For repositories that support transaction passing, we need to create a transaction
            // This is a temporary solution - ideally all repositories should support transaction injection
            if (repository is ITransactionRepository transactionRepo)
            {
                return await transactionRepo.ExecuteInTransactionAsync(operation, operationName);
            }
            else
            {
                // Fallback for repositories that don't support transaction injection yet
                _logger.LogWarning("Repository {RepositoryType} doesn't support transaction injection, using repository's own transaction management", repository.GetType().Name);
                
                // Create a dummy connection and transaction for compatibility
                // The repository will create its own transaction
                return await operation(null!, null!);
            }
        }

        protected async Task ExecuteInTransactionAsync(
            object repository,
            Func<IDbConnection, IDbTransaction, Task> operation,
            string operationName)
        {
            // For repositories that support transaction passing, we need to create a transaction
            if (repository is ITransactionRepository transactionRepo)
            {
                await transactionRepo.ExecuteInTransactionAsync(operation, operationName);
            }
            else
            {
                // Fallback for repositories that don't support transaction injection yet
                _logger.LogWarning("Repository {RepositoryType} doesn't support transaction injection, using repository's own transaction management", repository.GetType().Name);
                
                // Create a dummy connection and transaction for compatibility
                // The repository will create its own transaction
                await operation(null!, null!);
            }
        }
    }

    // Interface for repositories that support transaction injection
    public interface ITransactionRepository
    {
        Task<T> ExecuteInTransactionAsync<T>(Func<IDbConnection, IDbTransaction, Task<T>> operation, string operationName);
        Task ExecuteInTransactionAsync(Func<IDbConnection, IDbTransaction, Task> operation, string operationName);
    }
}
