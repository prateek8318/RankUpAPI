using System.ComponentModel.DataAnnotations;
using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Application.DTOs
{
    public class UserSubscriptionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SubscriptionPlanId { get; set; }
        public string RazorpayOrderId { get; set; } = string.Empty;
        public string? RazorpayPaymentId { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public SubscriptionStatus Status { get; set; }
        public bool AutoRenew { get; set; }
        public string? RazorpaySubscriptionId { get; set; }
        public DateTime? LastRenewalDate { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string? CancellationReason { get; set; }
        public SubscriptionPlanDto? SubscriptionPlan { get; set; }
        public PaymentTransactionDto? PaymentTransaction { get; set; }
        public InvoiceDto? Invoice { get; set; }
        public int DaysUntilExpiry { get; set; }
        public bool IsExpired { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateUserSubscriptionDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int SubscriptionPlanId { get; set; }

        [Required]
        public string RazorpayOrderId { get; set; } = string.Empty;

        public bool AutoRenew { get; set; } = false;
    }

    public class ActivateSubscriptionDto
    {
        [Required]
        public string RazorpayOrderId { get; set; } = string.Empty;

        [Required]
        public string RazorpayPaymentId { get; set; } = string.Empty;

        [Required]
        public string RazorpaySignature { get; set; } = string.Empty;
    }

    public class RenewSubscriptionDto
    {
        [Required]
        public int SubscriptionId { get; set; }

        public bool AutoRenew { get; set; } = false;
    }

    public class CancelSubscriptionDto
    {
        [Required]
        public int SubscriptionId { get; set; }

        [MaxLength(500)]
        public string? CancellationReason { get; set; }
    }

    public class SubscriptionHistoryDto
    {
        public int UserId { get; set; }
        public List<UserSubscriptionDto> Subscriptions { get; set; } = new();
        public int ActiveSubscriptionCount { get; set; }
        public int ExpiredSubscriptionCount { get; set; }
        public int CancelledSubscriptionCount { get; set; }
        public decimal TotalSpent { get; set; }
    }
}
