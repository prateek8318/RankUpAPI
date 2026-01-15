using System.ComponentModel.DataAnnotations;

namespace HomeDashboardService.Domain.Entities
{
    public class Quiz : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int ChapterId { get; set; }

        public int DurationMinutes { get; set; }

        public int TotalQuestions { get; set; }

        public int PassingMarks { get; set; }

        public QuizType Type { get; set; } = QuizType.Practice;

        public QuizDifficulty Difficulty { get; set; } = QuizDifficulty.Medium;

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        // Navigation properties
        public virtual Chapter Chapter { get; set; } = null!;
        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
        public virtual ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
    }

    public enum QuizType
    {
        Practice = 1,
        MockTest = 2,
        TestSeries = 3,
        PreviousYear = 4,
        SpeedTest = 5,
        BattleMode = 6
    }

    public enum QuizDifficulty
    {
        Easy = 1,
        Medium = 2,
        Hard = 3
    }
}
