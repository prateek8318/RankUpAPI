using MasterService.Application.Interfaces;
using Microsoft.Extensions.Logging;

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
            Func<Task<T>> operation,
            string operationName)
        {
            try
            {
                _logger.LogInformation("Starting transaction for operation: {OperationName}", operationName);
                
                // For now, we'll rely on the repository's existing transaction handling
                // In a full implementation, we would pass transaction context to repositories
                var result = await operation();
                
                _logger.LogInformation("Successfully completed operation: {OperationName}", operationName);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute operation: {OperationName}", operationName);
                throw;
            }
        }

        protected async Task ExecuteInTransactionAsync(
            object repository,
            Func<Task> operation,
            string operationName)
        {
            try
            {
                _logger.LogInformation("Starting transaction for operation: {OperationName}", operationName);
                
                // For now, we'll rely on the repository's existing transaction handling
                // In a full implementation, we would pass transaction context to repositories
                await operation();
                
                _logger.LogInformation("Successfully completed operation: {OperationName}", operationName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute operation: {OperationName}", operationName);
                throw;
            }
        }
    }
}
