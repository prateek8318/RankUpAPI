namespace RankUpAPI.Models
{
    public class UserDevice : BaseEntity
    {
        public int UserId { get; set; }
        public virtual User? User { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLength(255)]
        public string DeviceId { get; set; } = string.Empty;
        
        [System.ComponentModel.DataAnnotations.MaxLength(100)]
        public string? DeviceName { get; set; }
        
        [System.ComponentModel.DataAnnotations.MaxLength(50)]
        public string? DeviceType { get; set; } // iOS, Android, Web
        
        [System.ComponentModel.DataAnnotations.MaxLength(255)]
        public string? FcmToken { get; set; }
        
        public bool IsActive { get; set; } = true;
        public bool IsBlocked { get; set; } = false;
        public DateTime? LastUsedAt { get; set; }
    }
}
