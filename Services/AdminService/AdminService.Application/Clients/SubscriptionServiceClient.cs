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
                var json = JsonSerializer.Serialize(createDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.PostAsync("/api/admin/subscription-plans", content));

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(responseContent);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling SubscriptionService to create subscription plan");
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
    }
}
