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
        Task<SubscriptionBannerDto?> GetSubscriptionBannerAsync(int userId);
        Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(int quizId, int limit = 10);
        Task<List<TrendingTestDto>> GetTrendingTestsAsync(int limit = 10);
        Task<List<NotificationDto>> GetNotificationsAsync(int userId, int limit = 50);
        Task<List<DashboardBannerDto>> GetBannersAsync();
        Task<List<OfferBannerDto>> GetOfferBannersAsync();
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
