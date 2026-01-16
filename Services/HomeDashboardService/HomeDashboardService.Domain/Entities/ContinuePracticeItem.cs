using System.ComponentModel.DataAnnotations;

namespace HomeDashboardService.Domain.Entities
{
    public class ContinuePracticeItem : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int QuizAttemptId { get; set; }

        [Required]
        public int QuizId { get; set; }

        [MaxLength(200)]
        public string QuizTitle { get; set; } = string.Empty;

        public int ProgressPercentage { get; set; } = 0;

        public DateTime? LastAccessedAt { get; set; }

        public DateTime? StartedAt { get; set; }

        public int TimeRemainingSeconds { get; set; } = 0;
    }
}
