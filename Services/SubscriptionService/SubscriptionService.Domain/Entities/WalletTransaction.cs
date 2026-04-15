using System.ComponentModel.DataAnnotations;

namespace SubscriptionService.Domain.Entities
{
    public enum WalletTransactionType
    {
        Recharge = 1,
        Payment = 2,
        Refund = 3,
        Withdrawal = 4
    }

    public enum WalletTransactionStatus
    {
        Pending = 1,
        Success = 2,
        Failed = 3
    }

    public class WalletTransaction : BaseEntity
    {
        [Required]
        public int WalletId { get; set; }

        [Required]
        [MaxLength(100)]
        public string TransactionId { get; set; } = string.Empty;

        [Required]
        public WalletTransactionType TransactionType { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public decimal BalanceBefore { get; set; }

        [Required]
        public decimal BalanceAfter { get; set; }

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "INR";

        [MaxLength(50)]
        public string? PaymentMethod { get; set; }

        [MaxLength(50)]
        public string? PaymentProvider { get; set; }

        [MaxLength(100)]
        public string? ProviderTransactionId { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public WalletTransactionStatus Status { get; set; } = WalletTransactionStatus.Pending;

        [MaxLength(500)]
        public string? FailureReason { get; set; }

        public int? ReferenceId { get; set; }

        [MaxLength(50)]
        public string? ReferenceType { get; set; }

        public string? Metadata { get; set; }

        // Navigation properties
        public virtual UserWallet Wallet { get; set; } = null!;
    }
}
