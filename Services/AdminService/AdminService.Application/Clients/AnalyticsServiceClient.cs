using AdminService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System.Text.Json;

namespace AdminService.Application.Clients
{
    public class AnalyticsServiceClient : IAnalyticsServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AnalyticsServiceClient> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public AnalyticsServiceClient(HttpClient httpClient, ILogger<AnalyticsServiceClient> logger)
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

        public async Task<object?> GetUserAnalyticsAsync(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var queryParams = new List<string>();
                if (startDate.HasValue)
                    queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
                if (endDate.HasValue)
                    queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

                var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync($"/api/analytics/users{queryString}"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(content);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling AnalyticsService for user analytics");
                return null;
            }
        }

        public async Task<object?> GetQuizAnalyticsAsync(int? quizId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var queryParams = new List<string>();
                if (quizId.HasValue)
                    queryParams.Add($"quizId={quizId.Value}");
                if (startDate.HasValue)
                    queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
                if (endDate.HasValue)
                    queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

                var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync($"/api/analytics/quizzes{queryString}"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(content);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling AnalyticsService for quiz analytics");
                return null;
            }
        }

        public async Task<object?> GetSubscriptionAnalyticsAsync(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var queryParams = new List<string>();
                if (startDate.HasValue)
                    queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
                if (endDate.HasValue)
                    queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

                var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync($"/api/analytics/subscriptions{queryString}"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(content);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling AnalyticsService for subscription analytics");
                return null;
            }
        }

        public async Task<object?> GetLeaderboardDataAsync(int? quizId, int limit)
        {
            try
            {
                var queryParams = new List<string> { $"limit={limit}" };
                if (quizId.HasValue)
                    queryParams.Add($"quizId={quizId.Value}");

                var queryString = "?" + string.Join("&", queryParams);
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync($"/api/analytics/leaderboard{queryString}"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(content);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling AnalyticsService for leaderboard data");
                return null;
            }
        }
    }
}
