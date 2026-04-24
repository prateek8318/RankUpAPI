namespace UserService.Application.Interfaces
{
    public interface IOtpService
    {
        Task<bool> SendOtpAsync(string phoneNumber, string otp, CancellationToken cancellationToken = default);
        string GenerateOtp();
        void StoreOtp(string phoneNumber, string otp);
        bool ValidateOtp(string phoneNumber, string otp);
        void RemoveOtp(string phoneNumber);
    }
}
