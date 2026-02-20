using System.ComponentModel.DataAnnotations;

namespace SubscriptionService.Domain.Entities
{
    public enum PlanType
    {
        Monthly = 1,
        Yearly = 2,
        ExamSpecific = 3
    }

    public class SubscriptionPlan : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public PlanType Type { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "INR";

        [Range(0, int.MaxValue)]
        public int TestPapersCount { get; set; } = 0;

        [Range(0, double.MaxValue)]
        public decimal Discount { get; set; } = 0;

        /// <summary>
        /// Duration value as entered in UI (e.g. 1, 3, 12).
        /// ValidityDays is still used internally for expiry calculations.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int Duration { get; set; } = 1;

        /// <summary>
        /// Duration type label like "Monthly", "Yearly" etc. Keep as string for flexibility.
        /// </summary>
        [MaxLength(50)]
        public string DurationType { get; set; } = "Monthly";

        [Required]
        [Range(1, int.MaxValue)]
        public int ValidityDays { get; set; }

        [MaxLength(100)]
        public string? ExamCategory { get; set; }

        public List<string> Features { get; set; } = new List<string>();

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public bool IsPopular { get; set; } = false;

        public bool IsRecommended { get; set; } = false;

        [MaxLength(50)]
        public string? CardColorTheme { get; set; }

        public int SortOrder { get; set; } = 0;

        public ICollection<SubscriptionPlanTranslation> Translations { get; set; } = new List<SubscriptionPlanTranslation>();

        // Navigation properties
        public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
    }
}
