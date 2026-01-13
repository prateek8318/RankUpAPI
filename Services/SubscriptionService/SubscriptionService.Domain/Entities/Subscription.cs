using System.ComponentModel.DataAnnotations;

namespace SubscriptionService.Domain.Entities
{
    public enum SubscriptionStatus
    {
        Active = 1,
        Expired = 2,
        Cancelled = 3,
        Pending = 4
    }

    public class Subscription : BaseEntity
    {
        public int UserId { get; set; } // Reference to UserService

        [Required]
        [MaxLength(100)]
        public string PlanName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? PlanType { get; set; }

        public decimal Amount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Pending;

        public bool AutoRenew { get; set; } = false;
    }
}
