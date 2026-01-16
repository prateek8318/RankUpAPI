using System.ComponentModel.DataAnnotations;

namespace HomeDashboardService.Domain.Entities
{
    public class SubscriptionBanner : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        public string? LinkUrl { get; set; }

        [MaxLength(100)]
        public string? CtaText { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool ShowToSubscribedUsers { get; set; } = false;

        public bool ShowToNonSubscribedUsers { get; set; } = true;
    }
}
