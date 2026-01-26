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
        private readonly IHomeBannerRepository _homeBannerRepository;
        private readonly IOfferBannerRepository _offerBannerRepository;
        private readonly IQuizRepository _quizRepository;
        private readonly ILeaderboardEntryRepository _leaderboardEntryRepository;
        private readonly IPracticeModeRepository _practiceModeRepository;
        private readonly IDailyTargetRepository _dailyTargetRepository;
        private readonly IRapidFireTestRepository _rapidFireTestRepository;
        private readonly IFreeTestRepository _freeTestRepository;
        private readonly IMotivationMessageRepository _motivationMessageRepository;
        private readonly ISubscriptionBannerRepository _subscriptionBannerRepository;
        private readonly IContinuePracticeItemRepository _continuePracticeItemRepository;
        private readonly IExamRepository _examRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(
            IQuizAttemptRepository quizAttemptRepository,
            IDailyVideoRepository dailyVideoRepository,
            INotificationRepository notificationRepository,
            IHomeBannerRepository homeBannerRepository,
            IOfferBannerRepository offerBannerRepository,
            IQuizRepository quizRepository,
            ILeaderboardEntryRepository leaderboardEntryRepository,
            IPracticeModeRepository practiceModeRepository,
            IDailyTargetRepository dailyTargetRepository,
            IRapidFireTestRepository rapidFireTestRepository,
            IFreeTestRepository freeTestRepository,
            IMotivationMessageRepository motivationMessageRepository,
            ISubscriptionBannerRepository subscriptionBannerRepository,
            IContinuePracticeItemRepository continuePracticeItemRepository,
            IExamRepository examRepository,
            IMapper mapper,
            ILogger<DashboardService> logger)
        {
            _quizAttemptRepository = quizAttemptRepository;
            _dailyVideoRepository = dailyVideoRepository;
            _notificationRepository = notificationRepository;
            _homeBannerRepository = homeBannerRepository;
            _offerBannerRepository = offerBannerRepository;
            _quizRepository = quizRepository;
            _leaderboardEntryRepository = leaderboardEntryRepository;
            _practiceModeRepository = practiceModeRepository;
            _dailyTargetRepository = dailyTargetRepository;
            _rapidFireTestRepository = rapidFireTestRepository;
            _freeTestRepository = freeTestRepository;
            _motivationMessageRepository = motivationMessageRepository;
            _subscriptionBannerRepository = subscriptionBannerRepository;
            _continuePracticeItemRepository = continuePracticeItemRepository;
            _examRepository = examRepository;
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
                dashboard.SubscriptionBanner = await GetUserSubscriptionBannerAsync(userId);

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

        public async Task<SubscriptionBannerDto?> GetUserSubscriptionBannerAsync(int userId)
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
                    Type = q.Type.ToString()
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

        public async Task<List<HomeBannerDto>> GetBannersAsync()
        {
            try
            {
                var banners = await _homeBannerRepository.GetActiveBannersAsync();
                return _mapper.Map<List<HomeBannerDto>>(banners);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting banners");
                throw;
            }
        }

        public async Task<HomePageDataDto> GetHomePageDataAsync(int userId)
        {
            try
            {
                var homePageData = new HomePageDataDto();

                // Get greeting message
                var greetingMessage = await _motivationMessageRepository.GetGreetingMessageAsync();
                if (greetingMessage != null)
                {
                    homePageData.GreetingMessage = greetingMessage.Message;
                    homePageData.GreetingAuthor = greetingMessage.Author;
                }

                // Get practice modes
                var practiceModes = await _practiceModeRepository.GetActiveModesAsync();
                homePageData.PracticeModes = _mapper.Map<List<PracticeModeDto>>(practiceModes);

                // Get continue practice items
                var continueItems = await _continuePracticeItemRepository.GetUserActiveItemsAsync(userId);
                homePageData.ContinuePractice = _mapper.Map<List<ContinuePracticeItemDto>>(continueItems);

                // Get daily target
                var dailyTarget = await _dailyTargetRepository.GetUserTargetForDateAsync(userId, DateTime.UtcNow.Date);
                if (dailyTarget != null)
                {
                    homePageData.DailyTarget = _mapper.Map<DailyTargetDto>(dailyTarget);
                }

                // Get rapid fire tests
                var rapidFireTests = await _rapidFireTestRepository.GetActiveTestsAsync();
                homePageData.RapidFireTests = _mapper.Map<List<RapidFireTestDto>>(rapidFireTests);

                // Get free tests
                var freeTests = await _freeTestRepository.GetActiveTestsAsync();
                homePageData.FreeTests = _mapper.Map<List<FreeTestDto>>(freeTests);

                // Get banners
                homePageData.Banners = await GetBannersAsync();
                homePageData.OfferBanners = await GetOfferBannersAsync();

                // Get notification count
                homePageData.NotificationCount = await GetNotificationsCountAsync(userId);

                // Get leaderboard preview
                homePageData.LeaderboardPreview = await GetLeaderboardPreviewAsync(10);

                // Get subscription banner (check if user is subscribed - would need SubscriptionService integration)
                var subscriptionBanners = await _subscriptionBannerRepository.GetActiveBannersAsync(false);
                homePageData.SubscriptionBanner = subscriptionBanners.FirstOrDefault() != null 
                    ? _mapper.Map<SubscriptionBannerConfigDto>(subscriptionBanners.FirstOrDefault()) 
                    : null;

                return homePageData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting home page data for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<List<PracticeModeDto>> GetPracticeModesAsync()
        {
            try
            {
                var modes = await _practiceModeRepository.GetActiveModesAsync();
                return _mapper.Map<List<PracticeModeDto>>(modes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting practice modes");
                throw;
            }
        }

        public async Task<List<ContinuePracticeItemDto>> GetContinuePracticeAsync(int userId)
        {
            try
            {
                var items = await _continuePracticeItemRepository.GetUserActiveItemsAsync(userId);
                return _mapper.Map<List<ContinuePracticeItemDto>>(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting continue practice items for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<DailyTargetDto?> GetDailyTargetsAsync(int userId)
        {
            try
            {
                var target = await _dailyTargetRepository.GetUserTargetForDateAsync(userId, DateTime.UtcNow.Date);
                return target != null ? _mapper.Map<DailyTargetDto>(target) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting daily target for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<List<RapidFireTestDto>> GetRapidFireTestsAsync()
        {
            try
            {
                var tests = await _rapidFireTestRepository.GetActiveTestsAsync();
                return _mapper.Map<List<RapidFireTestDto>>(tests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rapid fire tests");
                throw;
            }
        }

        public async Task<List<FreeTestDto>> GetFreeTestsAsync()
        {
            try
            {
                var tests = await _freeTestRepository.GetActiveTestsAsync();
                return _mapper.Map<List<FreeTestDto>>(tests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting free tests");
                throw;
            }
        }

        public async Task<int> GetNotificationsCountAsync(int userId)
        {
            try
            {
                return await _notificationRepository.GetUnreadCountAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification count for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<List<LeaderboardPreviewDto>> GetLeaderboardPreviewAsync(int limit = 10)
        {
            try
            {
                // Get top entries from leaderboard
                var topQuizzes = await _quizRepository.GetTrendingQuizzesAsync(limit);
                var preview = new List<LeaderboardPreviewDto>();

                foreach (var quiz in topQuizzes)
                {
                    var topEntry = await _leaderboardEntryRepository.GetTopEntriesAsync(quiz.Id, 1);
                    if (topEntry.Any())
                    {
                        var entry = topEntry.First();
                        preview.Add(new LeaderboardPreviewDto
                        {
                            Rank = entry.Rank,
                            UserId = entry.UserId,
                            UserName = string.Empty, // Would need UserService integration
                            Score = entry.Score,
                            TotalMarks = entry.TotalMarks,
                            Accuracy = entry.Accuracy
                        });
                    }
                }

                return preview.Take(limit).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting leaderboard preview");
                throw;
            }
        }

        public async Task<SubscriptionBannerConfigDto?> GetSubscriptionBannerAsync(int userId)
        {
            try
            {
                // Check if user is subscribed (would need SubscriptionService integration)
                // For now, return banner for non-subscribed users
                var banners = await _subscriptionBannerRepository.GetActiveBannersAsync(false);
                return banners.FirstOrDefault() != null 
                    ? _mapper.Map<SubscriptionBannerConfigDto>(banners.FirstOrDefault()) 
                    : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription banner for user: {UserId}", userId);
                return null;
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

        public async Task<HomePageResponseDto> GetHomePageResponseAsync(int userId)
        {
            try
            {
                var response = new HomePageResponseDto();

                // Get greeting message
                var greetingMessage = await _motivationMessageRepository.GetGreetingMessageAsync();
                if (greetingMessage != null)
                {
                    response.Greeting = new GreetingDto
                    {
                        Message = greetingMessage.Message,
                        Author = greetingMessage.Author
                    };
                }

                // Get target exam - get first active exam as target (Admin should set this)
                // If no exam is found, return empty target object
                var activeExams = await _examRepository.GetActiveExamsAsync();
                var targetExam = activeExams.FirstOrDefault();
                response.Target = targetExam != null ? new TargetExamDto
                {
                    Id = targetExam.Id,
                    Title = targetExam.Name,
                    Subtitle = targetExam.Description,
                    IconUrl = null, // Can be added to Exam entity if needed
                    NavigationKey = $"exam/{targetExam.Id}",
                    IsActive = targetExam.IsActive
                } : new TargetExamDto();

                // Get practice modes
                var practiceModes = await _practiceModeRepository.GetActiveModesAsync();
                response.PracticeModes = practiceModes.Select(pm => new PracticeModeItemDto
                {
                    Id = pm.Id,
                    Title = pm.Name,
                    Subtitle = pm.Description,
                    IconUrl = pm.IconUrl,
                    NavigationKey = pm.LinkUrl ?? $"practice/{pm.Id}",
                    IsActive = pm.IsActive
                }).ToList();

                // Get continue practice items
                var continueItems = await _continuePracticeItemRepository.GetUserActiveItemsAsync(userId);
                response.ContinuePractice = continueItems.Select(ci => new ContinuePracticeItemDto
                {
                    Id = ci.Id,
                    Title = ci.QuizTitle,
                    Subtitle = $"Progress: {ci.ProgressPercentage}%",
                    IconUrl = null,
                    NavigationKey = $"quiz/{ci.QuizId}/continue",
                    IsActive = ci.IsActive,
                    QuizId = ci.QuizId,
                    QuizTitle = ci.QuizTitle,
                    ProgressPercentage = ci.ProgressPercentage,
                    LastAccessedAt = ci.LastAccessedAt,
                    TimeRemainingSeconds = ci.TimeRemainingSeconds
                }).ToList();

                // Get daily targets - global list (shared across users)
                // Show all active targets (no date window) so older entries remain visible until deactivated.
                var dailyTargets = await _dailyTargetRepository.GetTargetsAsync();
                response.DailyTargets = dailyTargets.Select(dt => new DailyTargetItemDto
                {
                    Id = dt.Id,
                    Title = dt.Title,
                    Subtitle = dt.Description,
                    IconUrl = null,
                    NavigationKey = $"target/{dt.Id}",
                    IsActive = dt.IsActive,
                    Duration = dt.TargetMinutes,
                    Subject = null // Can be enhanced if subject info is available
                }).ToList();

                // Get rapid fire tests
                var rapidFireTests = await _rapidFireTestRepository.GetActiveTestsAsync();
                response.RapidFireTests = rapidFireTests.Select(rft => new RapidFireTestItemDto
                {
                    Id = rft.Id,
                    Title = rft.Title,
                    Subtitle = rft.Description,
                    IconUrl = rft.ImageUrl,
                    LogoUrl = rft.LogoUrl,
                    BackgroundImageUrl = rft.BackgroundImageUrl,
                    NavigationKey = $"rapid-fire/{rft.Id}",
                    IsActive = rft.IsActive,
                    Duration = rft.DurationSeconds
                }).ToList();

                // Get free tests
                var freeTests = await _freeTestRepository.GetActiveTestsAsync();
                response.FreeTests = freeTests.Select(ft => new FreeTestItemDto
                {
                    Id = ft.Id,
                    Title = ft.Title,
                    Subtitle = ft.Description,
                    IconUrl = ft.ImageUrl ?? ft.ThumbnailUrl,
                    NavigationKey = ft.LinkUrl ?? $"free-test/{ft.Id}",
                    IsActive = ft.IsActive,
                    QuestionCount = ft.TotalQuestions,
                    Time = ft.DurationMinutes
                }).ToList();

                // Get banners
                var banners = await _homeBannerRepository.GetActiveBannersAsync();
                response.Banners = banners.Select(b => new BannerItemDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Subtitle = b.Description,
                    IconUrl = b.ImageUrl,
                    NavigationKey = b.LinkUrl ?? $"banner/{b.Id}",
                    IsActive = b.IsActive
                }).ToList();

                // Get offer banners
                var offerBanners = await _offerBannerRepository.GetActiveOffersAsync();
                response.OfferBanners = offerBanners.Select(ob => new OfferBannerItemDto
                {
                    Id = ob.Id,
                    Title = ob.Title,
                    Subtitle = ob.Description,
                    IconUrl = ob.ImageUrl,
                    NavigationKey = ob.LinkUrl ?? $"offer/{ob.Id}",
                    IsActive = ob.IsActive
                }).ToList();

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting home page response for user: {UserId}", userId);
                throw;
            }
        }
    }
}
