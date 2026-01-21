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
        
        // Notifications
        public int UnreadNotificationCount { get; set; }
        
        // Banners
        public List<HomeBannerDto> Banners { get; set; } = new();
        public List<OfferBannerDto> OfferBanners { get; set; } = new();
        
        // Trending Tests
        public List<TrendingTestDto> TrendingTests { get; set; } = new();
        
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

    public class HomeBannerDto
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

    public class DashboardBannerDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? LinkUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
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

    // Comprehensive Home Page Data DTO - Updated to match exact required structure
    public class HomePageResponseDto
    {
        public GreetingDto Greeting { get; set; } = new();
        public TargetExamDto Target { get; set; } = new();
        public List<PracticeModeItemDto> PracticeModes { get; set; } = new();
        public List<ContinuePracticeItemDto> ContinuePractice { get; set; } = new();
        public List<DailyTargetItemDto> DailyTargets { get; set; } = new();
        public List<RapidFireTestItemDto> RapidFireTests { get; set; } = new();
        public List<FreeTestItemDto> FreeTests { get; set; } = new();
        public List<BannerItemDto> Banners { get; set; } = new();
        public List<OfferBannerItemDto> OfferBanners { get; set; } = new();
    }

    // Legacy DTO - kept for backward compatibility during migration
    public class HomePageDataDto
    {
        // Greeting Section
        public string? GreetingMessage { get; set; }
        public string? GreetingAuthor { get; set; }

        // Target Card
        public TargetCardDto? TargetCard { get; set; }

        // Practice Modes
        public List<PracticeModeDto> PracticeModes { get; set; } = new();

        // Continue Practice
        public List<ContinuePracticeItemDto> ContinuePractice { get; set; } = new();

        // Daily Targets
        public DailyTargetDto? DailyTarget { get; set; }

        // Rapid Fire Tests
        public List<RapidFireTestDto> RapidFireTests { get; set; } = new();

        // Free Tests
        public List<FreeTestDto> FreeTests { get; set; } = new();

        // Banners
        public List<HomeBannerDto> Banners { get; set; } = new();
        public List<OfferBannerDto> OfferBanners { get; set; } = new();

        // Notification Count
        public int NotificationCount { get; set; }

        // Leaderboard Preview
        public List<LeaderboardPreviewDto> LeaderboardPreview { get; set; } = new();

        // Subscription Banner
        public SubscriptionBannerConfigDto? SubscriptionBanner { get; set; }
    }

    // New DTOs matching required structure
    public class GreetingDto
    {
        public string? Message { get; set; }
        public string? Author { get; set; }
    }

    public class TargetExamDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string? IconUrl { get; set; }
        public string? NavigationKey { get; set; }
        public bool IsActive { get; set; }
    }

    public class PracticeModeItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string? IconUrl { get; set; }
        public string? NavigationKey { get; set; }
        public bool IsActive { get; set; }
    }

    public class DailyTargetItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string? IconUrl { get; set; }
        public string? NavigationKey { get; set; }
        public bool IsActive { get; set; }
        public int Duration { get; set; }
        public string? Subject { get; set; }
    }

    public class RapidFireTestItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string? IconUrl { get; set; }
        public string? NavigationKey { get; set; }
        public bool IsActive { get; set; }
        public int Duration { get; set; }
    }

    public class FreeTestItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string? IconUrl { get; set; }
        public string? NavigationKey { get; set; }
        public bool IsActive { get; set; }
        public int QuestionCount { get; set; }
        public int Time { get; set; }
    }

    public class BannerItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string? IconUrl { get; set; }
        public string? NavigationKey { get; set; }
        public bool IsActive { get; set; }
    }

    public class OfferBannerItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string? IconUrl { get; set; }
        public string? NavigationKey { get; set; }
        public bool IsActive { get; set; }
    }

    public class TargetCardDto
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public int Year { get; set; }
        public int ProgressPercentage { get; set; }
    }

    public class PracticeModeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
        public string? ImageUrl { get; set; }
        public string? LinkUrl { get; set; }
        public PracticeModeType Type { get; set; }
        public bool IsFeatured { get; set; }
    }

    public class ContinuePracticeItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string? IconUrl { get; set; }
        public string? NavigationKey { get; set; }
        public bool IsActive { get; set; }
        public int QuizId { get; set; }
        public string QuizTitle { get; set; } = string.Empty;
        public int ProgressPercentage { get; set; }
        public DateTime? LastAccessedAt { get; set; }
        public int TimeRemainingSeconds { get; set; }
    }

    public class DailyTargetDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int TargetQuizzes { get; set; }
        public int CompletedQuizzes { get; set; }
        public int TargetMinutes { get; set; }
        public int CompletedMinutes { get; set; }
        public int TargetScore { get; set; }
        public int AchievedScore { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime TargetDate { get; set; }
    }

    public class RapidFireTestDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int QuizId { get; set; }
        public int DurationSeconds { get; set; }
        public int TotalQuestions { get; set; }
        public bool IsFeatured { get; set; }
    }

    public class FreeTestDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int QuizId { get; set; }
        public int ExamId { get; set; }
        public int DurationMinutes { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalMarks { get; set; }
        public bool IsFeatured { get; set; }
        public string? LinkUrl { get; set; }
    }

    public class LeaderboardPreviewDto
    {
        public int Rank { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int Score { get; set; }
        public int TotalMarks { get; set; }
        public decimal Accuracy { get; set; }
    }

    public class SubscriptionBannerConfigDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? LinkUrl { get; set; }
        public string? CtaText { get; set; }
    }

    // Create DTOs
    public class CreateBannerDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? LinkUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class CreateDailyTargetDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int TargetQuizzes { get; set; }
        public int TargetMinutes { get; set; }
        public int TargetScore { get; set; }
        public DateTime TargetDate { get; set; }
    }

    public class CreateTrendingTestDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int QuizId { get; set; }
        public int ExamId { get; set; }
        public int DurationMinutes { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalMarks { get; set; }
        public bool IsFeatured { get; set; }
        public int AttemptCount { get; set; }
        public decimal AverageScore { get; set; }
    }

    // Update DTOs
    public class UpdateBannerDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? LinkUrl { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? IsActive { get; set; }
    }

    public class UpdateDailyTargetDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? TargetQuizzes { get; set; }
        public int? TargetMinutes { get; set; }
        public int? TargetScore { get; set; }
        public DateTime? TargetDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
