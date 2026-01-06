namespace RankUpAPI.Areas.Admin.Models.Auth
{
    public class AdminAuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? Expiration { get; set; }
    }
} 
