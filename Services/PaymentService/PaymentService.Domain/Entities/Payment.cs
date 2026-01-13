using System.ComponentModel.DataAnnotations;

namespace PaymentService.Domain.Entities
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
        Razorpay = 1,
        UPI = 2,
        Card = 3,
        NetBanking = 4,
        Wallet = 5
    }

    public class Payment : BaseEntity
    {
        public int UserId { get; set; } // Reference to UserService
        public int? SubscriptionId { get; set; } // Reference to SubscriptionService

        [Required]
        [MaxLength(100)]
        public string TransactionId { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? RazorpayOrderId { get; set; }

        [MaxLength(100)]
        public string? RazorpayPaymentId { get; set; }

        public decimal Amount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }

        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        [MaxLength(500)]
        public string? FailureReason { get; set; }

        public DateTime? PaidAt { get; set; }
    }
}
