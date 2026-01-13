namespace UserService.Application.Interfaces
{
    public interface IOtpService
    {
        string GenerateOtp();
        void StoreOtp(string phoneNumber, string otp);
        bool ValidateOtp(string phoneNumber, string otp);
        void RemoveOtp(string phoneNumber);
    }
}
