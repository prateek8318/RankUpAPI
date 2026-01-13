using System.ComponentModel.DataAnnotations;

namespace RankUpAPI.Models
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
        public int UserId { get; set; }
        public virtual User? User { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string PlanName { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? PlanType { get; set; } // Monthly, Yearly, Lifetime
        
        public decimal Amount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Pending;
        
        public bool IsActive { get; set; } = true;
        public bool AutoRenew { get; set; } = false;
    }
}
