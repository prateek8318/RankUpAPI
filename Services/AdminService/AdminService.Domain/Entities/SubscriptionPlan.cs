using System.ComponentModel.DataAnnotations;

namespace AdminService.Domain.Entities
{
    public class SubscriptionPlan : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string PlanName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string ExamType { get; set; } = string.Empty;

        [Required]
        [Range(0, 999999)]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(50)]
        public string Duration { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? ColorCode { get; set; }

        public bool IsPopular { get; set; } = false;

        public bool IsRecommended { get; set; } = false;

        public bool IsActive { get; set; } = true;
    }
}
