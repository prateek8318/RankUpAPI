using System.ComponentModel.DataAnnotations;

namespace SubscriptionService.Application.DTOs
{
    public class SubscriptionValidationRequestDto
    {
        [Required]
        public int UserId { get; set; }

        public string? ExamCategory { get; set; }
    }

    public class SubscriptionValidationResponseDto
    {
        public bool IsValid { get; set; }
        public bool HasActiveSubscription { get; set; }
        public bool IsExpired { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? PlanName { get; set; }
        public string? ExamCategory { get; set; }
        public List<string> Features { get; set; } = new();
        public string Message { get; set; } = string.Empty;
        public int DaysUntilExpiry { get; set; }
        public bool RequiresRenewal { get; set; }
        public string? RenewalUrl { get; set; }
    }

    public class DemoEligibilityRequestDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string ExamCategory { get; set; } = string.Empty;
    }

    public class DemoEligibilityResponseDto
    {
        public bool IsEligible { get; set; }
        public bool HasUsedDemo { get; set; }
        public DateTime? LastAccessDate { get; set; }
        public int QuestionsAttempted { get; set; }
        public int RemainingQuestions { get; set; }
        public int MaxDemoQuestions { get; set; } = 10; // Configurable limit
        public string Message { get; set; } = string.Empty;
        public bool CanProceed { get; set; }
    }

    public class LogDemoAccessDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string ExamCategory { get; set; } = string.Empty;

        [Required]
        public int QuestionsAttempted { get; set; }

        [Required]
        public int TimeSpentMinutes { get; set; }

        [MaxLength(100)]
        public string? IPAddress { get; set; }

        [MaxLength(500)]
        public string? UserAgent { get; set; }

        [MaxLength(50)]
        public string? DeviceType { get; set; }

        public bool IsCompleted { get; set; } = false;

        [MaxLength(1000)]
        public string? AccessDetails { get; set; }
    }
}
