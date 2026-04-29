using AdminService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Data.SqlClient;

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

        public async Task<object?> GetUserSubscriptionHistoryAsync(int userId)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync($"/api/admin/usersubscriptions/user/{userId}"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(content);
                }

                _logger.LogWarning("Failed to get subscription history for user {UserId}: {StatusCode}", userId, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling SubscriptionService for user subscription history {UserId}", userId);
                return null;
            }
        }

        public async Task<object?> GetUserSubscriptionDetailsAsync(int userId)
        {
            try
            {
                // Try subscription service API first
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync($"/api/admin/usersubscriptions/user/{userId}/active"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(content);
                }

                // Fallback to direct database query if API fails
                _logger.LogWarning("Subscription service API failed for user {UserId}, falling back to direct database query", userId);
                return await GetSubscriptionFromDatabaseAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling SubscriptionService for user subscription details {UserId}", userId);
                return await GetSubscriptionFromDatabaseAsync(userId);
            }
        }

        private async Task<object?> GetSubscriptionFromDatabaseAsync(int userId)
        {
            try
            {
                using var connection = new SqlConnection(
                    "Server=ABHIJEET;Database=RankUp_SubscriptionDB;Trusted_Connection=True;TrustServerCertificate=True;");
                
                await connection.OpenAsync();
                
                var query = @"
                    SELECT 
                        us.Id,
                        us.UserId,
                        us.SubscriptionPlanId,
                        us.Status,
                        us.PurchasedDate,
                        us.ValidTill,
                        us.TestsUsed,
                        us.TestsTotal,
                        us.AmountPaid,
                        us.Currency,
                        sp.Name as PlanName,
                        sp.Price as PlanPrice
                    FROM UserSubscriptions us
                    LEFT JOIN SubscriptionPlans sp ON us.SubscriptionPlanId = sp.Id
                    WHERE us.UserId = @UserId AND us.Status = 'Active'
                    ORDER BY us.CreatedAt DESC";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);

                using var reader = await command.ExecuteReaderAsync();
                
                if (await reader.ReadAsync())
                {
                    var validTill = reader.GetDateTime(reader.GetOrdinal("ValidTill"));
                    var daysUntilExpiry = (int)Math.Ceiling((validTill - DateTime.UtcNow).TotalDays);

                    var subscription = new
                    {
                        id = reader.GetInt32(reader.GetOrdinal("Id")),
                        userId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        subscriptionPlanId = reader.GetInt32(reader.GetOrdinal("SubscriptionPlanId")),
                        status = reader.GetString(reader.GetOrdinal("Status")),
                        purchasedDate = reader.GetDateTime(reader.GetOrdinal("PurchasedDate")),
                        validTill,
                        testsUsed = reader.GetInt32(reader.GetOrdinal("TestsUsed")),
                        testsTotal = reader.GetInt32(reader.GetOrdinal("TestsTotal")),
                        amountPaid = reader.GetDecimal(reader.GetOrdinal("AmountPaid")),
                        currency = reader.GetString(reader.GetOrdinal("Currency")),
                        subscriptionPlan = new
                        {
                            name = reader.IsDBNull(reader.GetOrdinal("PlanName")) ? null : reader.GetString(reader.GetOrdinal("PlanName")),
                            price = reader.IsDBNull(reader.GetOrdinal("PlanPrice")) ? 0 : reader.GetDecimal(reader.GetOrdinal("PlanPrice"))
                        },
                        daysLeft = daysUntilExpiry,
                        daysUntilExpiry,
                        currentStatus = validTill <= DateTime.UtcNow ? "Expired" : "Active"
                    };
                    
                    return subscription;
                }
                
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription from database for user {UserId}", userId);
                return null;
            }
        }

        public async Task<object?> GetStatsAsync()
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync("/api/admin/subscription-plans/stats"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<object>(content);
                }

                _logger.LogWarning("Failed to get subscription plan stats: {StatusCode}", response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling SubscriptionService for subscription plan stats");
                return null;
            }
        }
    }
}
