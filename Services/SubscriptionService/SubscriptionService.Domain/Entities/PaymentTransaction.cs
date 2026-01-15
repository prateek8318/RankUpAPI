using System.ComponentModel.DataAnnotations;

namespace SubscriptionService.Domain.Entities
{
    public enum PaymentStatus
    {
        Pending = 1,
        Completed = 2,
        Failed = 3,
        Refunded = 4,
        PartiallyRefunded = 5
    }

    public enum PaymentMethod
    {
        UPI = 1,
        Card = 2,
        NetBanking = 3,
        Wallet = 4,
        RazorpaySubscription = 5
    }

    public class PaymentTransaction : BaseEntity
    {
        [Required]
        public int UserSubscriptionId { get; set; }

        [Required]
        [MaxLength(100)]
        public string TransactionId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string RazorpayOrderId { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? RazorpayPaymentId { get; set; }

        [MaxLength(100)]
        public string? RazorpaySignature { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; } = "INR";

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public PaymentMethod Method { get; set; }

        [MaxLength(500)]
        public string? GatewayResponse { get; set; } // Store full response from Razorpay

        [MaxLength(500)]
        public string? FailureReason { get; set; }

        public DateTime? CompletedAt { get; set; }

        public DateTime? RefundedAt { get; set; }

        public decimal RefundAmount { get; set; } = 0;

        [MaxLength(100)]
        public string? RefundId { get; set; }

        // Navigation properties
        public virtual UserSubscription UserSubscription { get; set; } = null!;
    }
}
