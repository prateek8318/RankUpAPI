using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Application.DTOs
{
    public class PaymentTransactionDto
    {
        public int Id { get; set; }
        public int UserSubscriptionId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string RazorpayOrderId { get; set; } = string.Empty;
        public string? RazorpayPaymentId { get; set; }
        public string? RazorpaySignature { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public PaymentStatus Status { get; set; }
        public PaymentMethod Method { get; set; }
        public string? GatewayResponse { get; set; }
        public string? FailureReason { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? RefundedAt { get; set; }
        public decimal RefundAmount { get; set; } = 0;
        public string? RefundId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
