using System.ComponentModel.DataAnnotations;

namespace SubscriptionService.Domain.Entities
{
    public enum PaymentStatus
    {
        Pending = 1,
        Success = 2,
        Failed = 3,
        Refunded = 4
    }

    public enum PaymentMethod
    {
        Card = 1,
        UPI = 2,
        NetBanking = 3,
        Wallet = 4
    }

    public class Payment : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int SubscriptionPlanId { get; set; }

        public int? UserSubscriptionId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "INR";

        [Required]
        public decimal DiscountAmount { get; set; } = 0;

        [Required]
        public decimal FinalAmount { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        [MaxLength(50)]
        public string PaymentProvider { get; set; } = "Razorpay";

        [Required]
        [MaxLength(100)]
        public string RazorpayOrderId { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? RazorpayPaymentId { get; set; }

        [MaxLength(100)]
        public string? RazorpaySignature { get; set; }

        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public DateTime? PaymentDate { get; set; }

        [MaxLength(500)]
        public string? FailureReason { get; set; }

        public decimal? RefundAmount { get; set; }

        public DateTime? RefundDate { get; set; }

        [MaxLength(500)]
        public string? RefundReason { get; set; }

        [MaxLength(100)]
        public string? RazorpayRefundId { get; set; }

        public string? Metadata { get; set; } // JSON metadata

        // Navigation properties
        public virtual SubscriptionPlan SubscriptionPlan { get; set; } = null!;
        public virtual UserSubscription? UserSubscription { get; set; }
    }
}
