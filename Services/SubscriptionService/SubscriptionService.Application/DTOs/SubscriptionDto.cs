using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Application.DTOs
{
    public class SubscriptionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public string? PlanType { get; set; }
        public decimal Amount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public SubscriptionStatus Status { get; set; }
        public bool AutoRenew { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateSubscriptionDto
    {
        public int UserId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public string? PlanType { get; set; }
        public decimal Amount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool AutoRenew { get; set; } = false;
    }
}
