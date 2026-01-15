using System.ComponentModel.DataAnnotations;

namespace SubscriptionService.Domain.Entities
{
    public class DemoAccessLog : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ExamCategory { get; set; } = string.Empty;

        [Required]
        public DateTime AccessDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int QuestionsAttempted { get; set; } = 0;

        [Required]
        public int TimeSpentMinutes { get; set; } = 0;

        [MaxLength(100)]
        public string? IPAddress { get; set; }

        [MaxLength(500)]
        public string? UserAgent { get; set; }

        [MaxLength(50)]
        public string? DeviceType { get; set; }

        public bool IsCompleted { get; set; } = false;

        [MaxLength(1000)]
        public string? AccessDetails { get; set; } // JSON data for additional info
    }
}
