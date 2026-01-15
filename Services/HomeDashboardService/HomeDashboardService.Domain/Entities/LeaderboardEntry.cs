using System.ComponentModel.DataAnnotations;

namespace HomeDashboardService.Domain.Entities
{
    public class LeaderboardEntry : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int QuizId { get; set; }

        public int Score { get; set; }

        public int TotalMarks { get; set; }

        public decimal Accuracy { get; set; }

        public int TimeTakenSeconds { get; set; }

        public DateTime AttemptDate { get; set; } = DateTime.UtcNow;

        public int Rank { get; set; }

        // Navigation properties
        public virtual Quiz Quiz { get; set; } = null!;
    }
}
