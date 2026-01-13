using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using RankUpAPI.Core.Configuration;
using RankUpAPI.Core.Services.Interfaces;

namespace RankUpAPI.Core.Services.Implementations
{
    /// <summary>
    /// OTP Service Implementation
    /// Handles OTP generation, storage, and validation
    /// </summary>
    public class OtpService : IOtpService
    {
        private readonly OtpSettings _otpSettings;
        private readonly ILogger<OtpService> _logger;
        
        // In-memory storage for OTPs (in production, use a distributed cache like Redis)
        private static readonly ConcurrentDictionary<string, OtpEntry> OtpStore = new();
        
        private class OtpEntry
        {
            public string Otp { get; set; } = string.Empty;
            public DateTime ExpiresAt { get; set; }
        }

        public OtpService(IOptions<OtpSettings> otpSettings, ILogger<OtpService> logger)
        {
            _otpSettings = otpSettings.Value;
            _logger = logger;
        }

        public string GenerateOtp()
        {
            if (_otpSettings.UseRandomOtp)
            {
                // Generate random OTP
                var random = new Random();
                var otp = random.Next(
                    (int)Math.Pow(10, _otpSettings.OtpLength - 1),
                    (int)Math.Pow(10, _otpSettings.OtpLength)
                ).ToString();
                
                _logger.LogInformation("Generated random OTP");
                return otp;
            }
            else
            {
                // Use default OTP from configuration
                _logger.LogInformation($"Using default OTP from configuration: {_otpSettings.DefaultOtp}");
                return _otpSettings.DefaultOtp;
            }
        }

        public void StoreOtp(string mobileNumber, string otp)
        {
            var otpKey = $"otp_{mobileNumber}";
            var entry = new OtpEntry
            {
                Otp = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_otpSettings.ExpirationMinutes)
            };
            
            OtpStore.AddOrUpdate(otpKey, entry, (key, oldValue) => entry);
            _logger.LogInformation($"OTP stored for {mobileNumber}, expires at {entry.ExpiresAt}");
        }

        public bool ValidateOtp(string mobileNumber, string otp)
        {
            var otpKey = $"otp_{mobileNumber}";
            
            if (!OtpStore.TryGetValue(otpKey, out var entry))
            {
                _logger.LogWarning($"OTP not found for {mobileNumber}");
                return false;
            }

            // Check if OTP has expired
            if (DateTime.UtcNow > entry.ExpiresAt)
            {
                _logger.LogWarning($"OTP expired for {mobileNumber}");
                OtpStore.TryRemove(otpKey, out _);
                return false;
            }

            // Validate OTP
            var isValid = entry.Otp == otp;
            if (!isValid)
            {
                _logger.LogWarning($"Invalid OTP for {mobileNumber}");
            }
            
            return isValid;
        }

        public void RemoveOtp(string mobileNumber)
        {
            var otpKey = $"otp_{mobileNumber}";
            OtpStore.TryRemove(otpKey, out _);
            _logger.LogInformation($"OTP removed for {mobileNumber}");
        }

        public string? GetStoredOtp(string mobileNumber)
        {
            var otpKey = $"otp_{mobileNumber}";
            
            if (OtpStore.TryGetValue(otpKey, out var entry))
            {
                // Check if expired
                if (DateTime.UtcNow > entry.ExpiresAt)
                {
                    OtpStore.TryRemove(otpKey, out _);
                    return null;
                }
                return entry.Otp;
            }
            
            return null;
        }
    }
}
