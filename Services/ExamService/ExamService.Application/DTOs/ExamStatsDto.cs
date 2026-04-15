namespace ExamService.Application.DTOs
{
    public class ExamStatsDto
    {
        public int TotalExams { get; set; }
        public int ActiveExams { get; set; }
        public int ScheduledExams { get; set; }
        public int DraftExams { get; set; }
        public int PaidExams { get; set; }
        public int TestSeriesCount { get; set; }
        public int MockTestCount { get; set; }
        public int DeepPracticeCount { get; set; }
        public int PreviousYearCount { get; set; }
    }

    public class ExamDashboardDto
    {
        public ExamStatsDto Stats { get; set; } = new();
        public List<RecentExamDto> RecentExams { get; set; } = new();
        public List<CategoryDistributionDto> CategoryDistribution { get; set; } = new();
    }

    public class RecentExamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string AccessType { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CategoryDistributionDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public int ExamCount { get; set; }
    }
}
