using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SubscriptionService.Domain.Interfaces;
using System.Text;
using System.Security.Cryptography;
using System.Globalization;

namespace SubscriptionService.Infrastructure.Services
{
    public class RazorpayService : IRazorpayService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RazorpayService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _keyId;
        private readonly string _keySecret;
        private readonly string _baseUrl;

        public RazorpayService(IConfiguration configuration, ILogger<RazorpayService> logger, HttpClient httpClient)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
            
            _keyId = _configuration["Razorpay:KeyId"] ?? throw new ArgumentNullException("Razorpay:KeyId not configured");
            _keySecret = _configuration["Razorpay:KeySecret"] ?? throw new ArgumentNullException("Razorpay:KeySecret not configured");
            _baseUrl = _configuration["Razorpay:BaseUrl"] ?? "https://api.razorpay.com/v1";

            // Set up basic authentication
            var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_keyId}:{_keySecret}"));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authString);
        }

        public async Task<RazorpayOrderResponse> CreateOrderAsync(decimal amount, string currency = "INR", string receipt = null)
        {
            try
            {
                _logger.LogInformation("Creating Razorpay order for amount: {Amount} {Currency}", amount, currency);

                var orderRequest = new
                {
                    amount = (int)(amount * 100), // Convert to paise
                    currency = currency,
                    receipt = receipt ?? $"receipt_{DateTime.UtcNow:yyyyMMddHHmmss}",
                    notes = new
                    {
                        created_at = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                    }
                };

                var content = new StringContent(JsonConvert.SerializeObject(orderRequest), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/orders", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to create Razorpay order: {Error}", errorContent);
                    throw new Exception($"Failed to create Razorpay order: {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var orderResponse = JsonConvert.DeserializeObject<RazorpayOrderResponse>(responseContent);

                _logger.LogInformation("Successfully created Razorpay order: {OrderId}", orderResponse?.Id);
                return orderResponse!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Razorpay order");
                throw;
            }
        }

        public async Task<RazorpayOrderDetails> GetOrderDetailsAsync(string orderId)
        {
            try
            {
                _logger.LogInformation("Getting Razorpay order details for: {OrderId}", orderId);

                var response = await _httpClient.GetAsync($"{_baseUrl}/orders/{orderId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to get Razorpay order details: {Error}", errorContent);
                    throw new Exception($"Failed to get Razorpay order details: {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var orderDetails = JsonConvert.DeserializeObject<RazorpayOrderDetails>(responseContent);

                _logger.LogInformation("Successfully retrieved Razorpay order details: {OrderId}", orderDetails?.Id);
                return orderDetails!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Razorpay order details");
                throw;
            }
        }

        public async Task<bool> VerifyPaymentAsync(string orderId, string paymentId, string signature)
        {
            try
            {
                _logger.LogInformation("Verifying Razorpay payment: {OrderId}, {PaymentId}", orderId, paymentId);

                // Generate the expected signature
                var payload = $"{orderId}|{paymentId}";
                var expectedSignature = GenerateHmacSha256(payload, _keySecret);

                var isValid = expectedSignature == signature;

                if (isValid)
                {
                    _logger.LogInformation("Payment verification successful for: {PaymentId}", paymentId);
                }
                else
                {
                    _logger.LogWarning("Payment verification failed for: {PaymentId}", paymentId);
                }

                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying Razorpay payment");
                throw;
            }
        }

        public async Task<RazorpaySubscriptionResponse> CreateSubscriptionAsync(PlanSubscriptionRequest request)
        {
            try
            {
                _logger.LogInformation("Creating Razorpay subscription for user: {UserId}", request.UserId);

                var subscriptionRequest = new
                {
                    plan_id = $"plan_{request.PlanId}",
                    total_count = request.TotalCount,
                    customer_notify = 1,
                    notes = new
                    {
                        user_id = request.UserId,
                        created_at = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                    }
                };

                var content = new StringContent(JsonConvert.SerializeObject(subscriptionRequest), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/subscriptions", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to create Razorpay subscription: {Error}", errorContent);
                    throw new Exception($"Failed to create Razorpay subscription: {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var subscriptionResponse = JsonConvert.DeserializeObject<RazorpaySubscriptionResponse>(responseContent);

                _logger.LogInformation("Successfully created Razorpay subscription: {SubscriptionId}", subscriptionResponse?.Id);
                return subscriptionResponse!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Razorpay subscription");
                throw;
            }
        }

        public async Task<bool> CancelSubscriptionAsync(string subscriptionId)
        {
            try
            {
                _logger.LogInformation("Cancelling Razorpay subscription: {SubscriptionId}", subscriptionId);

                var cancelRequest = new
                {
                    cancel_at_cycle_end = 0
                };

                var content = new StringContent(JsonConvert.SerializeObject(cancelRequest), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/subscriptions/{subscriptionId}/cancel", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to cancel Razorpay subscription: {Error}", errorContent);
                    return false;
                }

                _logger.LogInformation("Successfully cancelled Razorpay subscription: {SubscriptionId}", subscriptionId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling Razorpay subscription");
                throw;
            }
        }

        public async Task<RazorpayRefundResponse> ProcessRefundAsync(string paymentId, decimal amount)
        {
            try
            {
                _logger.LogInformation("Processing refund for payment: {PaymentId}, amount: {Amount}", paymentId, amount);

                var refundRequest = new
                {
                    amount = (int)(amount * 100) // Convert to paise
                };

                var content = new StringContent(JsonConvert.SerializeObject(refundRequest), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/payments/{paymentId}/refund", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to process refund: {Error}", errorContent);
                    throw new Exception($"Failed to process refund: {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var refundResponse = JsonConvert.DeserializeObject<RazorpayRefundResponse>(responseContent);

                _logger.LogInformation("Successfully processed refund: {RefundId}", refundResponse?.Id);
                return refundResponse!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund");
                throw;
            }
        }

        public async Task<RazorpayPaymentDetails> GetPaymentDetailsAsync(string paymentId)
        {
            try
            {
                _logger.LogInformation("Getting payment details for: {PaymentId}", paymentId);

                var response = await _httpClient.GetAsync($"{_baseUrl}/payments/{paymentId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to get payment details: {Error}", errorContent);
                    throw new Exception($"Failed to get payment details: {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var paymentDetails = JsonConvert.DeserializeObject<RazorpayPaymentDetails>(responseContent);

                _logger.LogInformation("Successfully retrieved payment details: {PaymentId}", paymentDetails?.Id);
                return paymentDetails!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment details");
                throw;
            }
        }

        private string GenerateHmacSha256(string data, string key)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
