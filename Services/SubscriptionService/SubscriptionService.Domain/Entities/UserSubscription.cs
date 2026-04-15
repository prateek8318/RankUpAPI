using System.ComponentModel.DataAnnotations;

namespace SubscriptionService.Domain.Entities
{
    public class UserSubscription : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int SubscriptionPlanId { get; set; }

        public int? PaymentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string RazorpayOrderId { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? RazorpayPaymentId { get; set; }

        [MaxLength(100)]
        public string? RazorpaySignature { get; set; }

        [Required]
        public DateTime PurchasedDate { get; set; }

        [Required]
        public DateTime ValidTill { get; set; }

        [Required]
        public int TestsUsed { get; set; } = 0;

        [Required]
        public int TestsTotal { get; set; } = 0;

        [Required]
        public decimal AmountPaid { get; set; }

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "INR";

        [Required]
        public decimal DiscountApplied { get; set; } = 0;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Active"; // Active, Expired, Cancelled

        public bool AutoRenewal { get; set; } = false;

        public DateTime? RenewalDate { get; set; }

        // Navigation properties
        public virtual SubscriptionPlan SubscriptionPlan { get; set; } = null!;
        public virtual Payment? Payment { get; set; }
    }
}
