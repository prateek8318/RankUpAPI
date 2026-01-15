using System.ComponentModel.DataAnnotations;

namespace SubscriptionService.Domain.Entities
{
    public class UserSubscription : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int SubscriptionPlanId { get; set; }

        [Required]
        [MaxLength(100)]
        public string RazorpayOrderId { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? RazorpayPaymentId { get; set; }

        [MaxLength(100)]
        public string? RazorpaySignature { get; set; }

        [Required]
        public decimal OriginalAmount { get; set; }

        [Required]
        public decimal FinalAmount { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Pending;

        public bool AutoRenew { get; set; } = false;

        [MaxLength(100)]
        public string? RazorpaySubscriptionId { get; set; } // For recurring payments

        public DateTime? LastRenewalDate { get; set; }

        public DateTime? CancelledDate { get; set; }

        [MaxLength(500)]
        public string? CancellationReason { get; set; }

        // Navigation properties
        public virtual SubscriptionPlan SubscriptionPlan { get; set; } = null!;
        public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
        public virtual Invoice? Invoice { get; set; }
    }
}
