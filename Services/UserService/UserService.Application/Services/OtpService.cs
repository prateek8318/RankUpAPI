using UserService.Application.Interfaces;
using UserService.Domain.ValueObjects;

namespace UserService.Application.Services
{
    public class OtpService : IOtpService
    {
        private readonly Dictionary<string, OtpCode> _otpStore = new();
        private readonly Random _random = new();

        public string GenerateOtp()
        {
            return _random.Next(100000, 999999).ToString();
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
