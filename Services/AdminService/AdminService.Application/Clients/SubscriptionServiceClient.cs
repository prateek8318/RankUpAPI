using AdminService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace AdminService.Application.Clients
{
    public class SubscriptionServiceClient : ISubscriptionServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SubscriptionServiceClient> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public SubscriptionServiceClient(HttpClient httpClient, ILogger<SubscriptionServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            
            _logger.LogInformation("SubscriptionServiceClient initialized with BaseAddress: {BaseAddress}, Timeout: {Timeout}s", 
                httpClient.BaseAddress, httpClient.Timeout.TotalSeconds);
            
            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        if (outcome.Exception != null)
                        {
                            _logger.LogWarning("Retry {RetryCount} after {Delay}s due to {ExceptionType}: {ExceptionMessage}", 
                                retryCount, timespan.TotalSeconds, outcome.Exception.GetType().Name, outcome.Exception.Message);
                        }
                        else
                        {
                            _logger.LogWarning("Retry {RetryCount} after {Delay}s due to HTTP status {StatusCode}", 
                                retryCount, timespan.TotalSeconds, outcome.Result?.StatusCode);
                        }
                    });
        }

        public async Task<object?> GetAllSubscriptionsAsync()
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync("/api/admin/subscription-plans"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(content);
                }

                _logger.LogWarning($"Failed to get all subscriptions: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling SubscriptionService for all subscriptions");
                return null;
            }
        }

        public async Task<object?> GetSubscriptionByIdAsync(int id)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync($"/api/admin/subscription-plans/{id}"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(content);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling SubscriptionService for subscription {id}");
                return null;
            }
        }

        public async Task<object?> GetActiveSubscriptionsAsync()
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync("/api/admin/subscription-plans/active"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(content);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling SubscriptionService for active subscriptions");
                return null;
            }
        }

        public async Task<object?> CreateSubscriptionPlanAsync(object createDto)
        {
            try
            {
                _logger.LogInformation("Creating subscription plan at {Url}", _httpClient.BaseAddress + "/api/admin/subscription-plans");
                
                var json = JsonSerializer.Serialize(createDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.PostAsync("/api/admin/subscription-plans", content));

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Successfully created subscription plan");
                    return JsonSerializer.Deserialize<object>(responseContent);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to create subscription plan. Status: {StatusCode}, Content: {Content}", 
                        response.StatusCode, errorContent);
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed when creating subscription plan. BaseAddress: {BaseAddress}", 
                    _httpClient.BaseAddress);
                return null;
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                _logger.LogError(ex, "Request timed out when creating subscription plan. Timeout: {Timeout}s", 
                    _httpClient.Timeout.TotalSeconds);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when creating subscription plan");
                return null;
            }
        }

        public async Task<object?> UpdateSubscriptionPlanAsync(int id, object updateDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(updateDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.PutAsync($"/api/admin/subscription-plans/{id}", content));

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(responseContent);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling SubscriptionService to update subscription plan {id}");
                return null;
            }
        }

        public async Task<bool> DeleteSubscriptionPlanAsync(int id)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.DeleteAsync($"/api/admin/subscription-plans/{id}"));

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling SubscriptionService to delete subscription plan {id}");
                return false;
            }
        }

        public async Task<List<Dictionary<string, object>>?> GetAllUserSubscriptionsAsync()
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync("/api/admin/usersubscriptions"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var subscriptions = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(content);
                    return subscriptions;
                }

                _logger.LogWarning($"Failed to get all user subscriptions: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling SubscriptionService for all user subscriptions");
                return null;
            }
        }

        public async Task<List<Dictionary<string, object>>?> GetExpiringUserSubscriptionsAsync(int daysBeforeExpiry = 30)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync($"/api/admin/usersubscriptions/expiring?daysBeforeExpiry={daysBeforeExpiry}"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var subscriptions = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(content);
                    return subscriptions;
                }

                _logger.LogWarning($"Failed to get expiring user subscriptions: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling SubscriptionService for expiring user subscriptions");
                return null;
            }
        }
    }
}
