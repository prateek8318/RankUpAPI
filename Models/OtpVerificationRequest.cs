namespace RankUpAPI.Models;

public class OtpVerificationRequest
{
    public string MobileNumber { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
}
