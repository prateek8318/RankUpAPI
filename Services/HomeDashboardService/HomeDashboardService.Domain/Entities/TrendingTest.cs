using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Domain.Entities
{
    public class TrendingTest : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int QuizId { get; set; }
        public int ExamId { get; set; }
        public int DurationMinutes { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalMarks { get; set; }
        public bool IsFeatured { get; set; }
        public int AttemptCount { get; set; }
        public decimal AverageScore { get; set; }
    }
}
