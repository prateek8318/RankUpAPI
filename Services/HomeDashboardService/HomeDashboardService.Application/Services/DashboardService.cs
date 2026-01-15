using AutoMapper;
using HomeDashboardService.Application.DTOs;
using HomeDashboardService.Application.Interfaces;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace HomeDashboardService.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IQuizAttemptRepository _quizAttemptRepository;
        private readonly IDailyVideoRepository _dailyVideoRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IDashboardBannerRepository _dashboardBannerRepository;
        private readonly IOfferBannerRepository _offerBannerRepository;
        private readonly IQuizRepository _quizRepository;
        private readonly ILeaderboardEntryRepository _leaderboardEntryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(
            IQuizAttemptRepository quizAttemptRepository,
            IDailyVideoRepository dailyVideoRepository,
            INotificationRepository notificationRepository,
            IDashboardBannerRepository dashboardBannerRepository,
            IOfferBannerRepository offerBannerRepository,
            IQuizRepository quizRepository,
            ILeaderboardEntryRepository leaderboardEntryRepository,
            IMapper mapper,
            ILogger<DashboardService> logger)
        {
            _quizAttemptRepository = quizAttemptRepository;
            _dailyVideoRepository = dailyVideoRepository;
            _notificationRepository = notificationRepository;
            _dashboardBannerRepository = dashboardBannerRepository;
            _offerBannerRepository = offerBannerRepository;
            _quizRepository = quizRepository;
            _leaderboardEntryRepository = leaderboardEntryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserDashboardDto> GetUserDashboardAsync(int userId)
        {
            try
            {
                var dashboard = new UserDashboardDto();

                // Get ongoing quizzes
                dashboard.OngoingQuizzes = await GetOngoingQuizzesAsync(userId);

                // Get recent attempts
                dashboard.RecentAttempts = await GetRecentAttemptsAsync(userId);

                // Get subscription banner (would need to call SubscriptionService)
                dashboard.SubscriptionBanner = await GetSubscriptionBannerAsync(userId);

                // Get leaderboard recent scores
                var recentHistory = await _leaderboardEntryRepository.GetUserHistoryAsync(userId, 10);
                dashboard.RecentScores = recentHistory.Select(e => new LeaderboardEntryDto
                {
                    Rank = e.Rank,
                    UserId = e.UserId,
                    QuizTitle = e.Quiz?.Title ?? string.Empty,
                    Score = e.Score,
                    TotalMarks = e.TotalMarks,
                    Accuracy = e.Accuracy,
                    AttemptDate = e.AttemptDate
                }).ToList();

                // Get trending tests
                dashboard.TrendingTests = await GetTrendingTestsAsync();

                // Get notifications count
                dashboard.UnreadNotificationCount = await _notificationRepository.GetUnreadCountAsync(userId);

                // Get banners
                dashboard.Banners = await GetBannersAsync();
                dashboard.OfferBanners = await GetOfferBannersAsync();

                // Get daily video
                dashboard.DailyVideo = await GetDailyVideoAsync();

                return dashboard;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user dashboard for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<List<OngoingQuizDto>> GetOngoingQuizzesAsync(int userId)
        {
            try
            {
                var ongoingAttempts = await _quizAttemptRepository.GetOngoingByUserIdAsync(userId);
                var now = DateTime.UtcNow;

                return ongoingAttempts.Select(attempt =>
                {
                    var quiz = attempt.Quiz;
                    var startedAt = attempt.StartedAt ?? DateTime.UtcNow;
                    var elapsedSeconds = (int)(now - startedAt).TotalSeconds;
                    var totalSeconds = quiz.DurationMinutes * 60;
                    var remainingSeconds = Math.Max(0, totalSeconds - elapsedSeconds);
                    var progressPercentage = attempt.CurrentQuestionIndex > 0 && quiz.TotalQuestions > 0
                        ? (int)((attempt.CurrentQuestionIndex * 100.0) / quiz.TotalQuestions)
                        : 0;

                    return new OngoingQuizDto
                    {
                        Id = attempt.Id,
                        QuizId = attempt.QuizId,
                        QuizTitle = quiz.Title,
                        ExamName = quiz.Chapter?.Subject?.Exam?.Name ?? string.Empty,
                        SubjectName = quiz.Chapter?.Subject?.Name ?? string.Empty,
                        ProgressPercentage = progressPercentage,
                        StartedAt = attempt.StartedAt,
                        DurationMinutes = quiz.DurationMinutes,
                        TimeRemainingSeconds = remainingSeconds
                    };
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ongoing quizzes for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<QuizAttemptDto?> ResumeQuizAsync(int userId, int quizAttemptId)
        {
            try
            {
                var attempt = await _quizAttemptRepository.GetByIdWithQuizAsync(quizAttemptId);
                if (attempt == null || attempt.UserId != userId || attempt.Status != QuizAttemptStatus.InProgress)
                {
                    return null;
                }

                var quizDto = _mapper.Map<QuizDto>(attempt.Quiz);
                return new QuizAttemptDto
                {
                    Id = attempt.Id,
                    QuizId = attempt.QuizId,
                    Quiz = quizDto,
                    Status = attempt.Status,
                    CurrentQuestionIndex = attempt.CurrentQuestionIndex,
                    StartedAt = attempt.StartedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resuming quiz for user: {UserId}, attempt: {AttemptId}", userId, quizAttemptId);
                throw;
            }
        }

        public async Task<DailyVideoDto?> GetDailyVideoAsync()
        {
            try
            {
                var video = await _dailyVideoRepository.GetTodayVideoAsync();
                return video != null ? _mapper.Map<DailyVideoDto>(video) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting daily video");
                throw;
            }
        }

        public async Task<List<RecentAttemptDto>> GetRecentAttemptsAsync(int userId, int limit = 10)
        {
            try
            {
                var attempts = await _quizAttemptRepository.GetRecentByUserIdAsync(userId, limit);
                return attempts.Select(attempt => new RecentAttemptDto
                {
                    Id = attempt.Id,
                    QuizId = attempt.QuizId,
                    QuizTitle = attempt.Quiz?.Title ?? string.Empty,
                    ExamName = attempt.Quiz?.Chapter?.Subject?.Exam?.Name ?? string.Empty,
                    Score = attempt.Score,
                    TotalMarks = attempt.TotalMarks,
                    Accuracy = attempt.Accuracy,
                    CompletedAt = attempt.CompletedAt,
                    Status = attempt.Status
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent attempts for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<SubscriptionBannerDto?> GetSubscriptionBannerAsync(int userId)
        {
            try
            {
                // This would typically call SubscriptionService API
                // For now, return null as we need to integrate with SubscriptionService
                // TODO: Integrate with SubscriptionService to get user subscription details
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription banner for user: {UserId}", userId);
                return null;
            }
        }

        public async Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(int quizId, int limit = 10)
        {
            try
            {
                var entries = await _leaderboardEntryRepository.GetTopEntriesAsync(quizId, limit);
                return entries.Select(e => new LeaderboardEntryDto
                {
                    Rank = e.Rank,
                    UserId = e.UserId,
                    QuizTitle = e.Quiz?.Title ?? string.Empty,
                    Score = e.Score,
                    TotalMarks = e.TotalMarks,
                    Accuracy = e.Accuracy,
                    AttemptDate = e.AttemptDate
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting leaderboard for quiz: {QuizId}", quizId);
                throw;
            }
        }

        public async Task<List<TrendingTestDto>> GetTrendingTestsAsync(int limit = 10)
        {
            try
            {
                var trendingQuizzes = await _quizRepository.GetTrendingQuizzesAsync(limit);
                return trendingQuizzes.Select(q => new TrendingTestDto
                {
                    QuizId = q.Id,
                    QuizTitle = q.Title,
                    ExamName = q.Chapter?.Subject?.Exam?.Name ?? string.Empty,
                    TotalAttempts = q.QuizAttempts.Count(a => a.StartedAt >= DateTime.UtcNow.AddDays(-30)),
                    AverageScore = q.QuizAttempts.Where(a => a.Status == QuizAttemptStatus.Completed && a.StartedAt >= DateTime.UtcNow.AddDays(-30))
                        .Select(a => (decimal)a.Score).DefaultIfEmpty(0).Average(),
                    Type = q.Type
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting trending tests");
                throw;
            }
        }

        public async Task<List<NotificationDto>> GetNotificationsAsync(int userId, int limit = 50)
        {
            try
            {
                var notifications = await _notificationRepository.GetByUserIdAsync(userId, limit);
                return _mapper.Map<List<NotificationDto>>(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<List<DashboardBannerDto>> GetBannersAsync()
        {
            try
            {
                var banners = await _dashboardBannerRepository.GetActiveBannersAsync();
                return _mapper.Map<List<DashboardBannerDto>>(banners);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting banners");
                throw;
            }
        }

        public async Task<List<OfferBannerDto>> GetOfferBannersAsync()
        {
            try
            {
                var offers = await _offerBannerRepository.GetActiveOffersAsync();
                return _mapper.Map<List<OfferBannerDto>>(offers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting offer banners");
                throw;
            }
        }
    }
}
