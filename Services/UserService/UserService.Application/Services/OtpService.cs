using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using UserService.Application.Interfaces;
using UserService.Domain.ValueObjects;

namespace UserService.Application.Services
{
    public class OtpService : IOtpService
    {
        private readonly ConcurrentDictionary<string, OtpCode> _otpStore = new();
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OtpService> _logger;

        public OtpService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ILogger<OtpService> logger)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<bool> SendOtpAsync(string phoneNumber, string otp, CancellationToken cancellationToken = default)
        {
            var useExternalProvider = _configuration.GetValue<bool>("OtpSettings:UseExternalProvider", true);
            var apiKey = _configuration.GetValue<string>("OtpSettings:TwoFactorApiKey");
            var templateName = _configuration.GetValue<string>("OtpSettings:TemplateName");

            if (!useExternalProvider || string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.LogInformation("External OTP provider disabled or API key missing. OTP generated locally for {PhoneNumber}.", phoneNumber);
                return true;
            }

            try
            {
                var encodedTemplate = string.IsNullOrWhiteSpace(templateName)
                    ? string.Empty
                    : $"/{Uri.EscapeDataString(templateName)}";

                var requestUrl = $"https://2factor.in/API/V1/{apiKey}/SMS/{phoneNumber}/{otp}{encodedTemplate}";
                var client = _httpClientFactory.CreateClient(nameof(OtpService));

                using var response = await client.GetAsync(requestUrl, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("2Factor OTP send failed for {PhoneNumber}. Status: {StatusCode}, Response: {Response}",
                    phoneNumber, response.StatusCode, responseBody);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send OTP via 2Factor for {PhoneNumber}", phoneNumber);
                return false;
            }
        }

        public string GenerateOtp()
        {
            var otpLength = _configuration.GetValue<int>("OtpSettings:OtpLength", 4);
            var minValue = (int)Math.Pow(10, otpLength - 1);
            var maxValue = (int)Math.Pow(10, otpLength) - 1;

            return RandomNumberGenerator.GetInt32(minValue, maxValue + 1).ToString();
        }

        public void StoreOtp(string phoneNumber, string otp)
        {
            var expiryMinutes = _configuration.GetValue<int>("OtpSettings:ExpirationMinutes", 5);
            _otpStore[phoneNumber] = new OtpCode(otp, phoneNumber, expiryMinutes);
        }

        public bool ValidateOtp(string phoneNumber, string otp)
        {
            if (!_otpStore.TryGetValue(phoneNumber, out var storedOtp))
                return false;

            if (storedOtp.IsExpired)
            {
                _otpStore.TryRemove(phoneNumber, out _);
                return false;
            }

            return storedOtp.IsValid(otp);
        }

        public void RemoveOtp(string phoneNumber)
        {
            _otpStore.TryRemove(phoneNumber, out _);
        }
    }
}
