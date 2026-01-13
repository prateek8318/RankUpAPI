using System.ComponentModel.DataAnnotations;

namespace RankUpAPI.Models
{
    public enum SupportStatus
    {
        Open = 1,
        InProgress = 2,
        Resolved = 3,
        Closed = 4
    }

    public class ContactSupport : BaseEntity
    {
        public int? UserId { get; set; }
        public virtual User? User { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Subject { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(2000)]
        public string Message { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? Email { get; set; }
        
        [MaxLength(15)]
        public string? PhoneNumber { get; set; }
        
        public SupportStatus Status { get; set; } = SupportStatus.Open;
        
        [MaxLength(2000)]
        public string? AdminResponse { get; set; }
        
        public int? AssignedToAdminId { get; set; }
        public virtual Admin? AssignedToAdmin { get; set; }
        
        public DateTime? ResolvedAt { get; set; }
    }
}
