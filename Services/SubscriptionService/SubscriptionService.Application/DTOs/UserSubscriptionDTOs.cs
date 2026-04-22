using System.ComponentModel.DataAnnotations;
using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Application.DTOs
{
    public class UserSubscriptionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SubscriptionPlanId { get; set; }
        public int? PaymentId { get; set; }
        public string RazorpayOrderId { get; set; } = string.Empty;
        public DateTime PurchasedDate { get; set; }
        public DateTime ValidTill { get; set; }
        public int TestsUsed { get; set; }
        public int TestsTotal { get; set; }
        public decimal AmountPaid { get; set; }
        public string Currency { get; set; } = "INR";
        public decimal DiscountApplied { get; set; }
        public string Status { get; set; } = "Active";
        public bool AutoRenewal { get; set; }
        public DateTime? RenewalDate { get; set; }
        public SubscriptionPlanDto? SubscriptionPlan { get; set; }
        public PaymentDto? Payment { get; set; }
        public int DaysLeft { get; set; }
        public string CurrentStatus { get; set; } = "Active";
        public int DaysUntilExpiry { get; set; }
        public bool IsExpired { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Exam details
        public string? ExamName { get; set; }
        public string? ExamDescription { get; set; }
        public string? ExamImageUrl { get; set; }
    }

    public class CreateUserSubscriptionDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int SubscriptionPlanId { get; set; }

        [Required]
        public int DurationOptionId { get; set; }

        public string? RazorpayOrderId { get; set; }

        public bool AutoRenewal { get; set; } = false;
    }

    public class PurchaseSubscriptionDto
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

        public bool AutoRenewal { get; set; } = false;
    }

    public class RenewSubscriptionDto
    {
        [Required]
        public int SubscriptionId { get; set; }

        public bool AutoRenewal { get; set; } = false;
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
