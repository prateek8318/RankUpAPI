namespace AdminService.Application.DTOs
{
    public class AdminDashboardMetricsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveSubscriptions { get; set; }
        public int TotalQuizzes { get; set; }
        public int TotalQuestions { get; set; }
        public decimal DailyRevenue { get; set; }
        public int DailyActiveUsers { get; set; }
        public int TotalExams { get; set; }
        public MetricTrendsDto Trends { get; set; } = new();
    }

    public class MetricTrendsDto
    {
        public decimal UsersChangePercentage { get; set; }
        public decimal SubscriptionsChangePercentage { get; set; }
        public decimal RevenueChangePercentage { get; set; }
        public decimal DailyActiveUsersChangePercentage { get; set; }
        public bool IsUsersUp { get; set; }
        public bool IsSubscriptionsUp { get; set; }
        public bool IsRevenueUp { get; set; }
        public bool IsDAUUp { get; set; }
    }
}
