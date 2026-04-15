using System.ComponentModel.DataAnnotations;
using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Application.DTOs
{
    public class UserWalletDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "INR";
        public DateTime? LastRechargeDate { get; set; }
        public decimal TotalRecharged { get; set; }
        public decimal TotalSpent { get; set; }
        public bool IsBlocked { get; set; }
        public string? BlockReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int TotalTransactions { get; set; }
    }

    public class WalletTransactionDto
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public WalletTransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
        public string Currency { get; set; } = "INR";
        public string? PaymentMethod { get; set; }
        public string? PaymentProvider { get; set; }
        public string? ProviderTransactionId { get; set; }
        public string? Description { get; set; }
        public WalletTransactionStatus Status { get; set; }
        public string? FailureReason { get; set; }
        public int? ReferenceId { get; set; }
        public string? ReferenceType { get; set; }
        public string? Metadata { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class RechargeWalletDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(10.00, 50000.00)]
        public decimal Amount { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public PaymentProvider PaymentProvider { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
    }

    public class RechargeWalletRazorpayDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(10.00, 50000.00)]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; } = "INR";

        [MaxLength(100)]
        public string? Receipt { get; set; }
    }

    public class RazorpayWalletRechargeResponseDto
    {
        public string OrderId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string Receipt { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Key { get; set; } = string.Empty; // Razorpay key for frontend
    }

    public class VerifyWalletRechargeDto
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

    public class WalletRechargeVerificationResultDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserWalletDto? Wallet { get; set; }
        public WalletTransactionDto? Transaction { get; set; }
    }

    public class WalletTransactionListRequestDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public WalletTransactionType? TransactionType { get; set; }
        public WalletTransactionStatus? Status { get; set; }
    }

    public class WalletTransactionListResponseDto
    {
        public IReadOnlyList<WalletTransactionDto> Items { get; set; } = new List<WalletTransactionDto>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class WalletStatisticsDto
    {
        public decimal Balance { get; set; }
        public decimal TotalRecharged { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime? LastRechargeDate { get; set; }
        public int TotalTransactions { get; set; }
        public int RechargeTransactions { get; set; }
        public int PaymentTransactions { get; set; }
        public int RefundTransactions { get; set; }
    }

    public class AdminWalletStatisticsDto
    {
        public int TotalWallets { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal TotalRechargedAmount { get; set; }
        public decimal TotalSpentAmount { get; set; }
        public int ActiveWallets { get; set; }
        public decimal AverageBalance { get; set; }
        public decimal HighestBalance { get; set; }
        public int TotalTransactions { get; set; }
        public int TotalRecharges { get; set; }
        public decimal TotalRechargeAmount { get; set; }
        public int TotalPayments { get; set; }
        public decimal TotalPaymentAmount { get; set; }
    }

    public class PayWithWalletDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int SubscriptionPlanId { get; set; }

        [Required]
        [Range(1.00, 50000.00)]
        public decimal Amount { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
    }

    public class WalletPaymentResultDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserWalletDto? Wallet { get; set; }
        public WalletTransactionDto? Transaction { get; set; }
        public UserSubscriptionDto? Subscription { get; set; }
    }

    public class CheckWalletBalanceDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(1.00, 50000.00)]
        public decimal RequiredAmount { get; set; }
    }

    public class WalletBalanceCheckResultDto
    {
        public decimal CurrentBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal BlockedAmount { get; set; }
        public bool HasSufficientBalance { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
