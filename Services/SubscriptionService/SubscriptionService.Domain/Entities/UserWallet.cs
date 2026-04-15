using System.ComponentModel.DataAnnotations;

namespace SubscriptionService.Domain.Entities
{
    public class UserWallet : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public decimal Balance { get; set; } = 0.00m;

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "INR";

        public DateTime? LastRechargeDate { get; set; }

        [Required]
        public decimal TotalRecharged { get; set; } = 0.00m;

        [Required]
        public decimal TotalSpent { get; set; } = 0.00m;

        public bool IsBlocked { get; set; } = false;

        [MaxLength(500)]
        public string? BlockReason { get; set; }

        // Navigation properties
        public virtual ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();
    }
}
