using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Application.DTOs
{
    public class SubscriptionPlanTranslationDto
    {
        public string LanguageCode { get; set; } = "en";
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Features { get; set; } = new();
    }

    public class SubscriptionPlanDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PlanType Type { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = "INR";
        public int TestPapersCount { get; set; }
        public decimal Discount { get; set; }
        public int Duration { get; set; }
        public string DurationType { get; set; } = "Monthly";
        public int ValidityDays { get; set; }
        public int? ExamId { get; set; }
        public string? ExamCategory { get; set; }
        public string? ExamType { get; set; } // alias for UI
        public List<string> Features { get; set; } = new();
        public string? ImageUrl { get; set; }
        public bool IsPopular { get; set; }
        public bool IsRecommended { get; set; }
        public string? CardColorTheme { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Admin/editing support
        public IReadOnlyList<SubscriptionPlanTranslationDto>? Translations { get; set; }
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
        [MaxLength(3)]
        public string Currency { get; set; } = "INR";

        public int TestPapersCount { get; set; } = 0;

        public decimal Discount { get; set; } = 0;

        [Range(1, int.MaxValue)]
        public int Duration { get; set; } = 1;

        [MaxLength(50)]
        public string DurationType { get; set; } = "Monthly";

        /// <summary>
        /// Optional: if not provided by UI, backend will compute from Duration + DurationType.
        /// </summary>
        public int ValidityDays { get; set; } = 0;

        /// <summary>Master Service Exam Id - dynamic exam selection from Master Service.</summary>
        public int? ExamId { get; set; }

        [MaxLength(100)]
        public string? ExamCategory { get; set; }

        public List<string> Features { get; set; } = new();

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Image file upload for subscription plan (preferred over ImageUrl)
        /// </summary>
        public IFormFile? ImageFile { get; set; }

        public bool IsPopular { get; set; } = false;

        public bool IsRecommended { get; set; } = false;

        [MaxLength(50)]
        public string? CardColorTheme { get; set; }

        public int SortOrder { get; set; } = 0;

        /// <summary>
        /// Optional non-English translations. LanguageCode should match Master Service Language.Code (multi-language dynamic from Master).
        /// </summary>
        public List<SubscriptionPlanTranslationDto>? Translations { get; set; }
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
        [MaxLength(3)]
        public string Currency { get; set; } = "INR";

        public int TestPapersCount { get; set; } = 0;

        public decimal Discount { get; set; } = 0;

        [Range(1, int.MaxValue)]
        public int Duration { get; set; } = 1;

        [MaxLength(50)]
        public string DurationType { get; set; } = "Monthly";

        /// <summary>
        /// Optional: if not provided by UI, backend will compute from Duration + DurationType.
        /// </summary>
        public int ValidityDays { get; set; } = 0;

        /// <summary>Master Service Exam Id - dynamic exam from Master Service.</summary>
        public int? ExamId { get; set; }

        [MaxLength(100)]
        public string? ExamCategory { get; set; }

        public List<string> Features { get; set; } = new();

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Image file upload for subscription plan (preferred over ImageUrl)
        /// </summary>
        public IFormFile? ImageFile { get; set; }

        public bool IsPopular { get; set; } = false;

        public bool IsRecommended { get; set; } = false;

        [MaxLength(50)]
        public string? CardColorTheme { get; set; }

        public int SortOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        /// <summary>Translations: LanguageCode from Master Service Language.Code.</summary>
        public List<SubscriptionPlanTranslationDto>? Translations { get; set; }
    }

    public class SubscriptionPlanListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PlanType Type { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = "INR";
        public int TestPapersCount { get; set; }
        public decimal Discount { get; set; }
        public int Duration { get; set; }
        public string DurationType { get; set; } = "Monthly";
        public int ValidityDays { get; set; }
        public int? ExamId { get; set; }
        public string? ExamCategory { get; set; }
        public string? ExamType { get; set; } // alias for UI
        public List<string> Features { get; set; } = new();
        public string? ImageUrl { get; set; }
        public bool IsPopular { get; set; }
        public bool IsRecommended { get; set; }
        public string? CardColorTheme { get; set; }
        public bool IsActive { get; set; }
        public List<PlanDurationOptionDto> DurationOptions { get; set; } = new();
    }

    public class SubscriptionPlanPagedRequestDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public bool IncludeInactive { get; set; } = true;
        public int? ExamId { get; set; }
        public string? Language { get; set; }
    }

    public class SubscriptionPlanPagedResponseDto
    {
        public IReadOnlyList<SubscriptionPlanListDto> Items { get; set; } = new List<SubscriptionPlanListDto>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class SubscriptionPlanStatsDto
    {
        public int ActivePlans { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int ExpiringSoon { get; set; }
        public int NewSubscribers { get; set; }
    }
}
