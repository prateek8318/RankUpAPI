using Microsoft.Extensions.Logging;

namespace QuestionService.Application.Services
{
    public abstract class BaseService
    {
        protected readonly ILogger _logger;

        protected BaseService(ILogger logger)
        {
            _logger = logger;
        }

        protected async Task<T> ExecuteInTransactionAsync<T>(
            Func<Task<T>> operation,
            string operationName)
        {
            try
            {
                _logger.LogInformation("Starting transaction for operation: {OperationName}", operationName);
                
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
            Func<Task> operation,
            string operationName)
        {
            try
            {
                _logger.LogInformation("Starting transaction for operation: {OperationName}", operationName);
                
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
