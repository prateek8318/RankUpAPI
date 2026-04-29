namespace SubscriptionService.Domain.Entities
{
    public class SubscriptionStatisticsDto
    {
        // Payment Statistics
        public int TotalPayments { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TodayRevenue { get; set; }
        public decimal ThisMonthRevenue { get; set; }
        public int SuccessfulPayments { get; set; }
        public int FailedPayments { get; set; }
        public int PendingPayments { get; set; }
        public decimal AverageTransactionAmount { get; set; }
        public int UniquePayingUsers { get; set; }
        public decimal RefundedAmount { get; set; }
        public int RefundedPayments { get; set; }
        
        // Plan Statistics
        public int ActivePlansCount { get; set; }
        
        // User Subscription Statistics
        public int ActiveSubscribers { get; set; }
        public int ExpiringSoon { get; set; }
        public int NewSubscribersToday { get; set; }
        public int NewUsersLast7Days { get; set; }
        public int BlockedUsers { get; set; }
        public int TotalUsers { get; set; }
        public int FreeUsers { get; set; }
        
        public DateTime LastUpdated { get; set; }
    }
}
