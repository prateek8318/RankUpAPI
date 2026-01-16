using System.ComponentModel.DataAnnotations;

namespace HomeDashboardService.Domain.Entities
{
    public class DailyTarget : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime TargetDate { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public int TargetQuizzes { get; set; } = 0;

        public int CompletedQuizzes { get; set; } = 0;

        public int TargetMinutes { get; set; } = 0;

        public int CompletedMinutes { get; set; } = 0;

        public int TargetScore { get; set; } = 0;

        public int AchievedScore { get; set; } = 0;

        public bool IsCompleted { get; set; } = false;

        public DateTime? CompletedAt { get; set; }
    }
}
