using PaymentService.Domain.Entities;

namespace PaymentService.Application.DTOs
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? SubscriptionId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string? RazorpayOrderId { get; set; }
        public string? RazorpayPaymentId { get; set; }
        public decimal Amount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public string? FailureReason { get; set; }
        public DateTime? PaidAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreatePaymentDto
    {
        public int UserId { get; set; }
        public int? SubscriptionId { get; set; }
        public decimal Amount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public PaymentMethod Method { get; set; }
    }

    public class UpdatePaymentStatusDto
    {
        public PaymentStatus Status { get; set; }
        public string? RazorpayOrderId { get; set; }
        public string? RazorpayPaymentId { get; set; }
        public string? FailureReason { get; set; }
    }
}
