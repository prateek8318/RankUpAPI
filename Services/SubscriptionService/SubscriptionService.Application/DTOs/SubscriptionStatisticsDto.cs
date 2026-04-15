namespace SubscriptionService.Application.DTOs
{
    public class SubscriptionStatisticsDto
    {
        public int TotalPayments { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TodayRevenue { get; set; }
        public decimal ThisMonthRevenue { get; set; }
        public int SuccessfulPayments { get; set; }
        public int FailedPayments { get; set; }
        public int PendingPayments { get; set; }
        public decimal AverageTransactionAmount { get; set; }
        public int UniquePayingUsers { get; set; }
    }
}
