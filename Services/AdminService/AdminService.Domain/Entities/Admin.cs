using System.ComponentModel.DataAnnotations;

namespace AdminService.Domain.Entities
{
    public class Admin : BaseEntity
    {
        public int UserId { get; set; } // Reference to UserService
        
        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "ContentManager";
        
        public bool IsTwoFactorEnabled { get; set; } = false;
        public string? TwoFactorSecret { get; set; }
        
        [MaxLength(20)]
        public string? MobileNumber { get; set; }
        
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public DateTime? LastLoginAt { get; set; }
        
        // Navigation properties
        public virtual ICollection<AdminRole> AdminRoles { get; set; } = new List<AdminRole>();
        public virtual ICollection<AdminSession> AdminSessions { get; set; } = new List<AdminSession>();
        public virtual ICollection<AdminActivityLog> ActivityLogs { get; set; } = new List<AdminActivityLog>();
    }
}
