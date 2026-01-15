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
        [Range(1, int.MaxValue)]
        public int ValidityDays { get; set; }

        [MaxLength(100)]
        public string? ExamCategory { get; set; }

        public List<string> Features { get; set; } = new List<string>();

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public bool IsPopular { get; set; } = false;

        public int SortOrder { get; set; } = 0;

        // Navigation properties
        public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
    }
}
