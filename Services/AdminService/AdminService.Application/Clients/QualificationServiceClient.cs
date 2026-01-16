using AdminService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace AdminService.Application.Clients
{
    public class QualificationServiceClient : IQualificationServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<QualificationServiceClient> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public QualificationServiceClient(HttpClient httpClient, ILogger<QualificationServiceClient> logger)
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

        public async Task<object?> CreateQualificationAsync(System.Text.Json.JsonElement createDto)
        {
            try
            {
                var json = createDto.GetRawText();
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.PostAsync("/api/qualifications", content));

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<object>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to create qualification: {StatusCode} - {Error}", response.StatusCode, errorContent);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling QualificationService to create qualification");
                return null;
            }
        }
    }
}
