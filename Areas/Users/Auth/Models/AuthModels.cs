namespace RankUpAPI.Areas.Users.Auth.Models
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public int? UserId { get; set; }
    }

    public class OtpRequest
    {
        public string MobileNumber { get; set; } = string.Empty;
    }

    public class OtpVerificationRequest : OtpRequest
    {
        public string Otp { get; set; } = string.Empty;
    }
}
