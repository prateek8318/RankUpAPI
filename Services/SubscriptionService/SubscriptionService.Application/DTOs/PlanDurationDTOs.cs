using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Application.DTOs
{
    public class PlanDurationOptionDto
    {
        public int Id { get; set; }
        public int SubscriptionPlanId { get; set; }
        public int DurationMonths { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; }
        public string DisplayLabel { get; set; } = string.Empty;
        public bool IsPopular { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        
        // Calculated properties
        public decimal EffectivePrice => Math.Round(Price * (1 - DiscountPercentage / 100), 2);
        public decimal MonthlyPrice => Math.Round(EffectivePrice / DurationMonths, 2);
        public decimal SavingsAmount => Math.Round(Price - EffectivePrice, 2);
    }

    public class CreatePlanDurationOptionDto
    {
        [Required]
        [Range(1, 36)]
        public int DurationMonths { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, 100)]
        public decimal DiscountPercentage { get; set; } = 0;

        [Required]
        [MaxLength(50)]
        public string DisplayLabel { get; set; } = string.Empty;

        public bool IsPopular { get; set; } = false;

        public int SortOrder { get; set; } = 0;
    }

    public class UpdatePlanDurationOptionDto
    {
        [Required]
        [Range(1, 36)]
        public int DurationMonths { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, 100)]
        public decimal DiscountPercentage { get; set; } = 0;

        [Required]
        [MaxLength(50)]
        public string DisplayLabel { get; set; } = string.Empty;

        public bool IsPopular { get; set; } = false;

        public int SortOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }

    public class PlanWithDurationOptionsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LocalizedName { get; set; } = string.Empty;
        public string LocalizedDescription { get; set; } = string.Empty;
        public PlanType Type { get; set; }
        public int TestPapersCount { get; set; }
        public int? ExamId { get; set; }
        public string? ExamCategory { get; set; }
        public List<string> Features { get; set; } = new();
        public List<string> LocalizedFeatures { get; set; } = new();
        public string? ImageUrl { get; set; }
        public bool IsPopular { get; set; }
        public bool IsRecommended { get; set; }
        public string? CardColorTheme { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<PlanDurationOptionDto> DurationOptions { get; set; } = new();
    }

    public class CreateSubscriptionPlanWithDurationDto
    {
        /// <summary>
        /// Optional: when provided, the plan is updated (upsert flow).
        /// </summary>
        public int? Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public PlanType Type { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal BasePrice { get; set; } // Base price for 1 month

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "INR";

        public int TestPapersCount { get; set; } = 0;

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

        [MaxLength(50)]
        public string? CardColorTheme { get; set; }

        public int SortOrder { get; set; } = 0;

        public bool IsPopular { get; set; } = false;

        public bool IsRecommended { get; set; } = false;

        [Required]
        public List<CreatePlanDurationOptionDto> DurationOptions { get; set; } = new();

        /// <summary>
        /// Optional non-English translations. LanguageCode should match Master Service Language.Code (multi-language dynamic from Master).
        /// </summary>
        public List<SubscriptionPlanTranslationDto>? Translations { get; set; }
    }

    public class PurchasePlanRequestDto
    {
        [Required]
        public int PlanId { get; set; }

        [Required]
        public int DurationOptionId { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = "Razorpay";

        public string Currency { get; set; } = "INR";
    }

    public class PurchasePlanResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? RazorpayOrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public PlanWithDurationOptionsDto? Plan { get; set; }
        public PlanDurationOptionDto? SelectedDuration { get; set; }
    }
}
