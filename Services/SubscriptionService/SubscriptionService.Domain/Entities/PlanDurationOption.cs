using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubscriptionService.Domain.Entities
{
    public class PlanDurationOption : BaseEntity
    {
        [Required]
        public int SubscriptionPlanId { get; set; }

        [Required]
        [Range(1, 36)]
        public int DurationMonths { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal DiscountPercentage { get; set; } = 0;

        [Required]
        [MaxLength(50)]
        public string DisplayLabel { get; set; } = string.Empty;

        public bool IsPopular { get; set; } = false;

        public int SortOrder { get; set; } = 0;

        public new bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("SubscriptionPlanId")]
        public virtual SubscriptionPlan? SubscriptionPlan { get; set; }

        // Calculated properties (not stored in database)
        [NotMapped]
        public decimal EffectivePrice => Math.Round(Price * (1 - DiscountPercentage / 100), 2);

        [NotMapped]
        public decimal MonthlyPrice => Math.Round(EffectivePrice / DurationMonths, 2);

        [NotMapped]
        public decimal SavingsAmount => Math.Round(Price - EffectivePrice, 2);
    }
}
