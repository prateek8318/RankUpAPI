using System.ComponentModel.DataAnnotations;
using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Application.DTOs
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SubscriptionPlanId { get; set; }
        public int? UserSubscriptionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentProvider PaymentProvider { get; set; }
        public string? TransactionId { get; set; }
        public string? ProviderOrderId { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? FailureReason { get; set; }
        public decimal? RefundAmount { get; set; }
        public DateTime? RefundDate { get; set; }
        public string? RefundReason { get; set; }
        public string? Metadata { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public SubscriptionPlanDto? SubscriptionPlan { get; set; }
        public UserSubscriptionDto? UserSubscription { get; set; }
    }

    public class CreatePaymentDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int SubscriptionPlanId { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public PaymentProvider PaymentProvider { get; set; }

        [MaxLength(100)]
        public string? TransactionId { get; set; }

        public string? Metadata { get; set; }
    }

    public class InitiatePaymentDto
    {
        [Required]
        public int PlanId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public PaymentProvider PaymentProvider { get; set; }

        [Required]
        public string Currency { get; set; } = "INR";

        [MaxLength(100)]
        public string? Receipt { get; set; }
    }

    public class PaymentInitiationResponseDto
    {
        public string OrderId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string Receipt { get; set; } = string.Empty;
        public PaymentProvider PaymentProvider { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PaymentUrl { get; set; } = string.Empty; // UPI payment URL for frontend
        public string QrCode { get; set; } = string.Empty; // Base64 QR code for UPI
    }

    public class VerifyPaymentDto
    {
        [Required]
        public string TransactionId { get; set; } = string.Empty;

        [Required]
        public PaymentProvider PaymentProvider { get; set; }

        [Required]
        public int UserId { get; set; }
    }

    public class PaymentVerificationResultDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserSubscriptionDto? UserSubscription { get; set; }
        public PaymentTransactionDto? PaymentTransaction { get; set; }
    }

    public class RefundRequestDto
    {
        [Required]
        public int PaymentId { get; set; }

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
