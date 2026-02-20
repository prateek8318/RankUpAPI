using AdminService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace AdminService.Application.Clients
{
    public class MasterServiceClient : IMasterServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MasterServiceClient> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public MasterServiceClient(HttpClient httpClient, ILogger<MasterServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            
            _retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        _logger.LogWarning($"Retry {retryCount} after {timespan.TotalSeconds}s");
                    });
        }

        public async Task<object?> CreateLanguageAsync(System.Text.Json.JsonElement createDto)
        {
            try
            {
                var json = createDto.GetRawText();
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.PostAsync("/api/languages", content));

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<object>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to create language: {StatusCode} - {Error}", response.StatusCode, errorContent);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling MasterService to create language");
                return null;
            }
        }

        public async Task<object?> CreateStateAsync(System.Text.Json.JsonElement createDto)
        {
            try
            {
                var json = createDto.GetRawText();
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.PostAsync("/api/states", content));

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<object>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to create state: {StatusCode} - {Error}", response.StatusCode, errorContent);
                    
                    // Throw exceptions for specific status codes
                    if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    {
                        throw new InvalidOperationException(errorContent);
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        throw new ArgumentException(errorContent);
                    }
                }

                return null;
            }
            catch (InvalidOperationException)
            {
                throw; // Re-throw InvalidOperationException
            }
            catch (ArgumentException)
            {
                throw; // Re-throw ArgumentException
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling MasterService to create state");
                return null;
            }
        }
    }
}
