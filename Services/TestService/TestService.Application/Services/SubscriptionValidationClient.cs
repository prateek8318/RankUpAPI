using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TestService.Application.Interfaces;

namespace TestService.Application.Services
{
    public class SubscriptionValidationClient : ISubscriptionValidationClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SubscriptionValidationClient> _logger;

        public SubscriptionValidationClient(HttpClient httpClient, ILogger<SubscriptionValidationClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<SubscriptionValidationResponse?> ValidateAsync(int userId)
        {
            try
            {
                using var response = await _httpClient.PostAsJsonAsync("/api/system/subscriptionvalidation/validate", new { userId });
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Subscription validation returned status {StatusCode} for user {UserId}", response.StatusCode, userId);
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<SubscriptionValidationResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to validate subscription for user {UserId}", userId);
                return null;
            }
        }
    }

    public class SubscriptionValidationResponse
    {
        public bool IsValid { get; set; }
        public bool HasActiveSubscription { get; set; }
        public bool IsExpired { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Message { get; set; } = string.Empty;
        public int DaysUntilExpiry { get; set; }
        public bool RequiresRenewal { get; set; }
    }
}
