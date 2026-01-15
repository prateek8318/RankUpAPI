using System.ComponentModel.DataAnnotations;
using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Application.DTOs
{
    public class SubscriptionPlanDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PlanType Type { get; set; }
        public decimal Price { get; set; }
        public int ValidityDays { get; set; }
        public string? ExamCategory { get; set; }
        public List<string> Features { get; set; } = new();
        public string? ImageUrl { get; set; }
        public bool IsPopular { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateSubscriptionPlanDto
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

        public List<string> Features { get; set; } = new();

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public bool IsPopular { get; set; } = false;

        public int SortOrder { get; set; } = 0;
    }

    public class UpdateSubscriptionPlanDto
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

        public List<string> Features { get; set; } = new();

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public bool IsPopular { get; set; } = false;

        public int SortOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }

    public class SubscriptionPlanListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PlanType Type { get; set; }
        public decimal Price { get; set; }
        public int ValidityDays { get; set; }
        public string? ExamCategory { get; set; }
        public List<string> Features { get; set; } = new();
        public string? ImageUrl { get; set; }
        public bool IsPopular { get; set; }
        public bool IsActive { get; set; }
    }
}
