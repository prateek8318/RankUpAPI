namespace SubscriptionService.Application.DTOs
{
    public class PaymentStatisticsDto
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
        public List<PaymentMethodStatisticsDto> PaymentsByMethod { get; set; } = new();
        public List<DailyRevenueDto> DailyRevenue { get; set; } = new();
    }

    public class PaymentMethodStatisticsDto
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class DailyRevenueDto
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int TransactionCount { get; set; }
    }
}
