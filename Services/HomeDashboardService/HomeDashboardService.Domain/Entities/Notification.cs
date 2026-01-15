using System.ComponentModel.DataAnnotations;

namespace HomeDashboardService.Domain.Entities
{
    public class Notification : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Message { get; set; }

        [MaxLength(500)]
        public string? LinkUrl { get; set; }

        public NotificationType Type { get; set; } = NotificationType.Info;

        public bool IsRead { get; set; } = false;

        public DateTime? ReadAt { get; set; }
    }

    public enum NotificationType
    {
        Info = 1,
        Success = 2,
        Warning = 3,
        Error = 4,
        QuizReminder = 5,
        SubscriptionRenewal = 6,
        Achievement = 7
    }
}
