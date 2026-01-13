namespace RankUpAPI.Models
{
    public class AdminSession : BaseEntity
    {
        public int AdminId { get; set; }
        public virtual Admin? Admin { get; set; }
        
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string? DeviceInfo { get; set; }
        public string? IpAddress { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
