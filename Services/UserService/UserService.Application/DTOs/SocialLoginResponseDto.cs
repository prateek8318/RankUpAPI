namespace UserService.Application.DTOs
{
    public class SocialLoginResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public UserDto User { get; set; }
        public bool IsNewUser { get; set; }
    }
}
