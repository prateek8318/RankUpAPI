using System.ComponentModel.DataAnnotations;

namespace RankUpAPI.Models
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
        public int UserId { get; set; }
        public virtual User? User { get; set; }
        
        public int? SubscriptionId { get; set; }
        public virtual Subscription? Subscription { get; set; }
        
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
