namespace UserService.Application.DTOs
{
    public class MinimalSocialLoginRequestDto
    {
        public string? Provider { get; set; }
        public string? GoogleId { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? AvatarUrl { get; set; }
        public string? MobileNumber { get; set; }
        public string? DeviceId { get; set; }
        public string? DeviceType { get; set; }
        public string? DeviceName { get; set; }
        public string? FcmToken { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
