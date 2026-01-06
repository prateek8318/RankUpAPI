using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RankUpAPI.Models
{
    public class User : BaseEntity
    {
        [MaxLength(100)]
        public string? Name { get; set; }
        
        [EmailAddress, MaxLength(100)]
        public string? Email { get; set; }
        
        [MaxLength(255)]
        public string? PasswordHash { get; set; }
        
        [Required, MaxLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? Gender { get; set; }
        
        [MaxLength(255)]
        public string? ProfilePhoto { get; set; }
        
        [MaxLength(100)]
        public string? PreferredExam { get; set; }
        
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        
        // Allow setting LastLoginAt from code (remove computed attribute)
        public DateTime? LastLoginAt { get; set; }
        
        // Whether the phone number has been verified via OTP
        public bool IsPhoneVerified { get; set; } = false;
        
        // Navigation property
        public virtual Admin? Admin { get; set; }
    }
}