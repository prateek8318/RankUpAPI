using Microsoft.Extensions.Configuration;
using UserService.Application.Interfaces;
using UserService.Domain.ValueObjects;

namespace UserService.Application.Services
{
    public class OtpService : IOtpService
    {
        private readonly Dictionary<string, OtpCode> _otpStore = new();
        private readonly Random _random = new();
        private readonly IConfiguration _configuration;

        public OtpService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateOtp()
        {
            var otpLength = _configuration.GetValue<int>("OtpSettings:OtpLength", 4);
            var minValue = (int)Math.Pow(10, otpLength - 1);
            var maxValue = (int)Math.Pow(10, otpLength) - 1;
            
            return _random.Next(minValue, maxValue).ToString();
        }

        public void StoreOtp(string phoneNumber, string otp)
        {
            _otpStore[phoneNumber] = new OtpCode(otp, phoneNumber);
        }

        public bool ValidateOtp(string phoneNumber, string otp)
        {
            if (!_otpStore.TryGetValue(phoneNumber, out var storedOtp))
                return false;

            if (storedOtp.IsExpired)
            {
                _otpStore.Remove(phoneNumber);
                return false;
            }

            return storedOtp.IsValid(otp);
        }

        public void RemoveOtp(string phoneNumber)
        {
            _otpStore.Remove(phoneNumber);
        }
    }
}
