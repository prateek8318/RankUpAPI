using System.ComponentModel.DataAnnotations;

namespace RankUpAPI.Models
{
    public enum AdminRole
    {
        SuperAdmin = 1,
        ContentManager = 2,
        SupportExecutive = 3,
        FinanceTeam = 4
    }

    public class Admin : BaseEntity
    {
        public int UserId { get; set; }
        public virtual User? User { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "ContentManager"; // SuperAdmin, ContentManager, SupportExecutive, FinanceTeam
        
        // RBAC Permissions
        public bool CanManageUsers { get; set; } = false;
        public bool CanManageContent { get; set; } = false;
        public bool CanManagePayments { get; set; } = false;
        public bool CanManageSubscriptions { get; set; } = false;
        public bool CanManageReports { get; set; } = false;
        public bool CanManageCMS { get; set; } = false;
        public bool CanManageOffers { get; set; } = false;
        public bool CanManageSupport { get; set; } = false;
        public bool CanSendNotifications { get; set; } = false;
        public bool CanViewDashboard { get; set; } = true;
        
        // 2FA Settings
        public bool IsTwoFactorEnabled { get; set; } = false;
        public string? TwoFactorSecret { get; set; }
        
        // Session Management
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
}
