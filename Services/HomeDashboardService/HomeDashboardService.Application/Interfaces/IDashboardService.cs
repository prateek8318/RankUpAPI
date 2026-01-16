using HomeDashboardService.Application.DTOs;
using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<UserDashboardDto> GetUserDashboardAsync(int userId);
        Task<List<OngoingQuizDto>> GetOngoingQuizzesAsync(int userId);
        Task<QuizAttemptDto?> ResumeQuizAsync(int userId, int quizAttemptId);
        Task<DailyVideoDto?> GetDailyVideoAsync();
        Task<List<RecentAttemptDto>> GetRecentAttemptsAsync(int userId, int limit = 10);
        Task<SubscriptionBannerDto?> GetUserSubscriptionBannerAsync(int userId);
        Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(int quizId, int limit = 10);
        Task<List<TrendingTestDto>> GetTrendingTestsAsync(int limit = 10);
        Task<List<NotificationDto>> GetNotificationsAsync(int userId, int limit = 50);
        Task<List<HomeBannerDto>> GetBannersAsync();
        Task<List<OfferBannerDto>> GetOfferBannersAsync();
        Task<HomePageDataDto> GetHomePageDataAsync(int userId);
        Task<HomePageResponseDto> GetHomePageResponseAsync(int userId);
        Task<List<PracticeModeDto>> GetPracticeModesAsync();
        Task<List<ContinuePracticeItemDto>> GetContinuePracticeAsync(int userId);
        Task<DailyTargetDto?> GetDailyTargetsAsync(int userId);
        Task<List<RapidFireTestDto>> GetRapidFireTestsAsync();
        Task<List<FreeTestDto>> GetFreeTestsAsync();
        Task<int> GetNotificationsCountAsync(int userId);
        Task<List<LeaderboardPreviewDto>> GetLeaderboardPreviewAsync(int limit = 10);
        Task<SubscriptionBannerConfigDto?> GetSubscriptionBannerAsync(int userId);
    }

    public class QuizAttemptDto
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public QuizDto Quiz { get; set; } = null!;
        public QuizAttemptStatus Status { get; set; }
        public int CurrentQuestionIndex { get; set; }
        public DateTime? StartedAt { get; set; }
    }
}
