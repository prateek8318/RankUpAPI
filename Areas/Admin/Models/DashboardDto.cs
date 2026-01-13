namespace RankUpAPI.Areas.Admin.Models
{
    public class DashboardMetricsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveSubscriptions { get; set; }
        public int DailyActiveUsers { get; set; }
        public decimal DailyRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int MostAttemptedQuizId { get; set; }
        public string? MostAttemptedQuizName { get; set; }
        public int MostAttemptedQuizCount { get; set; }
        public int TotalQuizzes { get; set; }
        public int TotalVideos { get; set; }
        public int TotalMockTests { get; set; }
        public int PendingSupportTickets { get; set; }
        public List<TrendData>? UserTrend { get; set; }
        public List<TrendData>? RevenueTrend { get; set; }
    }

    public class TrendData
    {
        public string Date { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }
}
