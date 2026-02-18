using System.ComponentModel.DataAnnotations;

namespace AdminService.Application.DTOs
{
    public class SubscriptionPlanDto
    {
        public int Id { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public string ExamType { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Duration { get; set; } = string.Empty;
        public string? ColorCode { get; set; }
        public bool IsPopular { get; set; }
        public bool IsRecommended { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateSubscriptionPlanRequest
    {
        [Required(ErrorMessage = "Plan name is required")]
        [MaxLength(100, ErrorMessage = "Plan name cannot exceed 100 characters")]
        public string PlanName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Exam type is required")]
        [MaxLength(50, ErrorMessage = "Exam type cannot exceed 50 characters")]
        public string ExamType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0, 999999, ErrorMessage = "Price must be between 0 and 999999")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [MaxLength(50, ErrorMessage = "Duration cannot exceed 50 characters")]
        public string Duration { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "Color code cannot exceed 20 characters")]
        public string? ColorCode { get; set; }

        public bool IsPopular { get; set; } = false;
        public bool IsRecommended { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }

    public class UpdateSubscriptionPlanRequest
    {
        [Required(ErrorMessage = "Plan name is required")]
        [MaxLength(100, ErrorMessage = "Plan name cannot exceed 100 characters")]
        public string PlanName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Exam type is required")]
        [MaxLength(50, ErrorMessage = "Exam type cannot exceed 50 characters")]
        public string ExamType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0, 999999, ErrorMessage = "Price must be between 0 and 999999")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [MaxLength(50, ErrorMessage = "Duration cannot exceed 50 characters")]
        public string Duration { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "Color code cannot exceed 20 characters")]
        public string? ColorCode { get; set; }

        public bool IsPopular { get; set; }
        public bool IsRecommended { get; set; }
        public bool IsActive { get; set; }
    }

    public class SubscriptionPlanFilterRequest
    {
        public string? ExamType { get; set; }
        public bool? IsPopular { get; set; }
        public bool? IsRecommended { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class SubscriptionPlanStatsDto
    {
        public int ActivePlans { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int ExpiringSoon { get; set; }
        public int NewSubscribers { get; set; }
    }
}
