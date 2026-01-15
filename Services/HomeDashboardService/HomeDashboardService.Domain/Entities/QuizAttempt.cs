using System.ComponentModel.DataAnnotations;

namespace HomeDashboardService.Domain.Entities
{
    public class QuizAttempt : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int QuizId { get; set; }

        public QuizAttemptStatus Status { get; set; } = QuizAttemptStatus.InProgress;

        public int Score { get; set; }

        public int TotalMarks { get; set; }

        public decimal Accuracy { get; set; }

        public int TimeTakenSeconds { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public DateTime? LastActivityAt { get; set; }

        public int CurrentQuestionIndex { get; set; } = 0;

        // Navigation properties
        public virtual Quiz Quiz { get; set; } = null!;
    }

    public enum QuizAttemptStatus
    {
        InProgress = 1,
        Completed = 2,
        Abandoned = 3,
        TimeUp = 4
    }
}
