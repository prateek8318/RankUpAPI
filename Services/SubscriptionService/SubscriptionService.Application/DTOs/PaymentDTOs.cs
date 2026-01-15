using System.ComponentModel.DataAnnotations;
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
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateRazorpayOrderDto
    {
        [Required]
        public int PlanId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string Currency { get; set; } = "INR";

        [MaxLength(100)]
        public string? Receipt { get; set; }
    }

    public class RazorpayOrderResponseDto
    {
        public string OrderId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string Receipt { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Key { get; set; } = string.Empty; // Razorpay key for frontend
    }

    public class VerifyPaymentDto
    {
        [Required]
        public string RazorpayOrderId { get; set; } = string.Empty;

        [Required]
        public string RazorpayPaymentId { get; set; } = string.Empty;

        [Required]
        public string RazorpaySignature { get; set; } = string.Empty;

        [Required]
        public int UserId { get; set; }
    }

    public class PaymentVerificationResultDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserSubscriptionDto? Subscription { get; set; }
        public PaymentTransactionDto? PaymentTransaction { get; set; }
    }

    public class RefundRequestDto
    {
        [Required]
        public string PaymentId { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }
    }

    public class RefundResponseDto
    {
        public bool IsSuccess { get; set; }
        public string RefundId { get; set; } = string.Empty;
        public decimal RefundAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
