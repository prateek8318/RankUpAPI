using System.ComponentModel.DataAnnotations;
using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Application.DTOs
{
    public class UserDashboardDto
    {
        // Quiz & Learning Actions
        public List<OngoingQuizDto> OngoingQuizzes { get; set; } = new();
        
        // Activity & Navigation
        public List<RecentAttemptDto> RecentAttempts { get; set; } = new();
        public SubscriptionBannerDto? SubscriptionBanner { get; set; }
        
        // Leadership Board
        public List<LeaderboardEntryDto> RecentScores { get; set; } = new();
        public List<TrendingTestDto> TrendingTests { get; set; } = new();
        
        // Notifications
        public int UnreadNotificationCount { get; set; }
        
        // Banners
        public List<DashboardBannerDto> Banners { get; set; } = new();
        public List<OfferBannerDto> OfferBanners { get; set; } = new();
        
        // Multimedia Content
        public DailyVideoDto? DailyVideo { get; set; }
    }

    public class OngoingQuizDto
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string QuizTitle { get; set; } = string.Empty;
        public string ExamName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int ProgressPercentage { get; set; }
        public DateTime? StartedAt { get; set; }
        public int DurationMinutes { get; set; }
        public int TimeRemainingSeconds { get; set; }
    }

    public class RecentAttemptDto
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string QuizTitle { get; set; } = string.Empty;
        public string ExamName { get; set; } = string.Empty;
        public int Score { get; set; }
        public int TotalMarks { get; set; }
        public decimal Accuracy { get; set; }
        public DateTime? CompletedAt { get; set; }
        public QuizAttemptStatus Status { get; set; }
    }

    public class SubscriptionBannerDto
    {
        public int UserSubscriptionId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public DateTime EndDate { get; set; }
        public int DaysUntilExpiry { get; set; }
        public bool IsExpiringSoon { get; set; }
        public bool CanRenew { get; set; }
        public bool CanUpgrade { get; set; }
        public string? PromotionMessage { get; set; }
    }

    public class LeaderboardEntryDto
    {
        public int Rank { get; set; }
        public int UserId { get; set; }
        public string QuizTitle { get; set; } = string.Empty;
        public int Score { get; set; }
        public int TotalMarks { get; set; }
        public decimal Accuracy { get; set; }
        public DateTime AttemptDate { get; set; }
    }

    public class TrendingTestDto
    {
        public int QuizId { get; set; }
        public string QuizTitle { get; set; } = string.Empty;
        public string ExamName { get; set; } = string.Empty;
        public int TotalAttempts { get; set; }
        public decimal AverageScore { get; set; }
        public QuizType Type { get; set; }
    }

    public class DashboardBannerDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? LinkUrl { get; set; }
        public bool IsPromoted { get; set; }
    }

    public class OfferBannerDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? LinkUrl { get; set; }
        public string? DiscountCode { get; set; }
        public decimal? DiscountPercentage { get; set; }
    }

    public class DailyVideoDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string VideoUrl { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public DateTime VideoDate { get; set; }
        public int ViewCount { get; set; }
    }

    public class NotificationDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Message { get; set; }
        public string? LinkUrl { get; set; }
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
