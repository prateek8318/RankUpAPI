using System.ComponentModel.DataAnnotations;

namespace RankUpAPI.Models
{
    public enum NotificationType
    {
        General = 1,
        Offer = 2,
        Exam = 3,
        Subscription = 4,
        Warning = 5,
        System = 6
    }

    public class Notification : BaseEntity
    {
        public int? UserId { get; set; } // null = broadcast to all users
        public virtual User? User { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string? Message { get; set; }
        
        public NotificationType Type { get; set; } = NotificationType.General;
        
        [MaxLength(500)]
        public string? ImageUrl { get; set; }
        
        [MaxLength(500)]
        public string? ActionUrl { get; set; }
        
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }
        public DateTime? SentAt { get; set; }
    }
}
