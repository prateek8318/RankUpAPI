using System.ComponentModel.DataAnnotations;

namespace SubscriptionService.Domain.Entities
{
    /// <summary>
    /// Localized text for SubscriptionPlan. If requested language is missing, API falls back to English fields on SubscriptionPlan.
    /// </summary>
    public class SubscriptionPlanTranslation
    {
        public int Id { get; set; }

        [Required]
        public int SubscriptionPlanId { get; set; }

        [Required]
        [MaxLength(10)]
        public string LanguageCode { get; set; } = "en"; // "en", "hi", "ta", ...

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public List<string> Features { get; set; } = new();

        public SubscriptionPlan SubscriptionPlan { get; set; } = null!;
    }
}

