using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Domain.Interfaces
{
    public interface ISubscriptionValidationService
    {
        Task<SubscriptionValidationResult> ValidateSubscriptionAsync(int userId, string examCategory = null);
        Task<DemoAccessResult> CheckDemoEligibilityAsync(int userId, string examCategory);
        Task<bool> IsSubscriptionActiveAsync(int userId);
        Task<UserSubscription?> GetActiveSubscriptionAsync(int userId);
    }

    public class SubscriptionValidationResult
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
        public SubscriptionStatus Status { get; set; }
    }

    public class DemoAccessResult
    {
        public bool IsEligible { get; set; }
        public bool HasUsedDemo { get; set; }
        public DateTime? LastAccessDate { get; set; }
        public int QuestionsAttempted { get; set; }
        public int RemainingQuestions { get; set; }
        public int MaxDemoQuestions { get; set; } = 10;
        public string Message { get; set; } = string.Empty;
        public bool CanProceed { get; set; }
    }
}
