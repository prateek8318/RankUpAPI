using HomeDashboardService.Application.DTOs;
using HomeDashboardService.Application.Interfaces;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace HomeDashboardService.Application.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IQuizAttemptRepository _quizAttemptRepository;
        private readonly ILogger<AdminDashboardService> _logger;

        public AdminDashboardService(
            IQuizAttemptRepository quizAttemptRepository,
            ILogger<AdminDashboardService> logger)
        {
            _quizAttemptRepository = quizAttemptRepository;
            _logger = logger;
        }

        public async Task<AdminDashboardMetricsDto> GetAdminDashboardMetricsAsync()
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var yesterday = today.AddDays(-1);
                var lastWeek = today.AddDays(-7);

                // Total Users (would need to call UserService)
                int totalUsers = 0; // TODO: Get from UserService

                // Active Subscriptions (would need to call SubscriptionService)
                int activeSubscriptions = 0; // TODO: Get from SubscriptionService

                // Most Attempted Quiz
                var mostAttemptedQuiz = await GetMostAttemptedQuizAsync();

                // Daily Revenue (would need to call PaymentService)
                decimal dailyRevenue = 0; // TODO: Get from PaymentService

                // Daily Active Users
                var todayAttempts = await _quizAttemptRepository.FindAsync(a => a.StartedAt.HasValue && a.StartedAt.Value.Date == today);
                var dau = todayAttempts.Select(a => a.UserId).Distinct().Count();

                // Trends
                var trends = await GetMetricTrendsAsync();

                return new AdminDashboardMetricsDto
                {
                    TotalUsers = totalUsers,
                    ActiveSubscriptions = activeSubscriptions,
                    MostAttemptedQuizId = mostAttemptedQuiz.QuizId,
                    MostAttemptedQuizTitle = mostAttemptedQuiz.Title,
                    DailyRevenue = dailyRevenue,
                    DailyActiveUsers = dau,
                    Trends = trends
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin dashboard metrics");
                throw;
            }
        }

        public async Task<MetricTrendsDto> GetMetricTrendsAsync()
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var yesterday = today.AddDays(-1);
                var lastWeek = today.AddDays(-7);

                // Users trends (would need UserService)
                decimal usersChange = 0;
                bool isUsersUp = true;

                // Subscriptions trends (would need SubscriptionService)
                decimal subscriptionsChange = 0;
                bool isSubscriptionsUp = true;

                // Revenue trends (would need PaymentService)
                decimal revenueChange = 0;
                bool isRevenueUp = true;

                // DAU trends
                var todayAttempts = await _quizAttemptRepository.FindAsync(a => a.StartedAt.HasValue && a.StartedAt.Value.Date == today);
                var yesterdayAttempts = await _quizAttemptRepository.FindAsync(a => a.StartedAt.HasValue && a.StartedAt.Value.Date == yesterday);
                
                var todayDAU = todayAttempts.Select(a => a.UserId).Distinct().Count();
                var yesterdayDAU = yesterdayAttempts.Select(a => a.UserId).Distinct().Count();
                
                decimal dauChange = yesterdayDAU > 0 
                    ? ((todayDAU - yesterdayDAU) * 100.0m / yesterdayDAU) 
                    : 0;
                bool isDAUUp = todayDAU >= yesterdayDAU;

                return new MetricTrendsDto
                {
                    UsersChangePercentage = usersChange,
                    SubscriptionsChangePercentage = subscriptionsChange,
                    RevenueChangePercentage = revenueChange,
                    DailyActiveUsersChangePercentage = dauChange,
                    IsUsersUp = isUsersUp,
                    IsSubscriptionsUp = isSubscriptionsUp,
                    IsRevenueUp = isRevenueUp,
                    IsDAUUp = isDAUUp
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting metric trends");
                throw;
            }
        }

        private async Task<(int QuizId, string Title)> GetMostAttemptedQuizAsync()
        {
            var lastWeek = DateTime.UtcNow.AddDays(-7);
            var attempts = await _quizAttemptRepository.FindAsync(a => a.StartedAt >= lastWeek);
            
            var mostAttempted = attempts
                .GroupBy(a => a.QuizId)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            if (mostAttempted != null)
            {
                var firstAttempt = mostAttempted.FirstOrDefault();
                if (firstAttempt?.Quiz != null)
                {
                    return (mostAttempted.Key, firstAttempt.Quiz.Title);
                }
            }

            return (0, "No quizzes attempted");
        }

        // Banner Management
        public async Task<DashboardBannerDto> CreateBannerAsync(CreateBannerDto banner)
        {
            try
            {
                var entity = new HomeBanner
                {
                    Title = banner.Title,
                    Description = banner.Description,
                    ImageUrl = banner.ImageUrl,
                    LinkUrl = banner.LinkUrl,
                    DisplayOrder = banner.DisplayOrder,
                    IsActive = banner.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                // TODO: Save to database using repository
                // var savedEntity = await _bannerRepository.AddAsync(entity);
                
                return new DashboardBannerDto
                {
                    Id = 1, // TODO: Return actual ID
                    Title = entity.Title,
                    Description = entity.Description,
                    ImageUrl = entity.ImageUrl,
                    LinkUrl = entity.LinkUrl,
                    DisplayOrder = entity.DisplayOrder,
                    IsActive = entity.IsActive
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating banner");
                throw;
            }
        }

        public async Task<DashboardBannerDto?> UpdateBannerAsync(int id, UpdateBannerDto banner)
        {
            try
            {
                // TODO: Get existing entity from repository
                // var entity = await _bannerRepository.GetByIdAsync(id);
                // if (entity == null) return null;

                // Update properties
                // entity.Title = banner.Title ?? entity.Title;
                // entity.Description = banner.Description ?? entity.Description;
                // entity.ImageUrl = banner.ImageUrl ?? entity.ImageUrl;
                // entity.LinkUrl = banner.LinkUrl ?? entity.LinkUrl;
                // entity.DisplayOrder = banner.DisplayOrder ?? entity.DisplayOrder;
                // entity.IsActive = banner.IsActive ?? entity.IsActive;
                // entity.UpdatedAt = DateTime.UtcNow;

                // await _bannerRepository.UpdateAsync(entity);

                return new DashboardBannerDto
                {
                    Id = id,
                    Title = banner.Title ?? "Updated Banner",
                    Description = banner.Description,
                    ImageUrl = banner.ImageUrl,
                    LinkUrl = banner.LinkUrl,
                    DisplayOrder = banner.DisplayOrder ?? 1,
                    IsActive = banner.IsActive ?? true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating banner");
                throw;
            }
        }

        // Free Test Management
        public async Task<FreeTestDto> CreateFreeTestAsync(CreateFreeTestDto test)
        {
            try
            {
                var entity = new FreeTest
                {
                    Title = test.Title,
                    Description = test.Description,
                    ImageUrl = test.ImageUrl,
                    ThumbnailUrl = test.ThumbnailUrl,
                    QuizId = test.QuizId,
                    ExamId = test.ExamId,
                    DurationMinutes = test.DurationMinutes,
                    TotalQuestions = test.TotalQuestions,
                    TotalMarks = test.TotalMarks,
                    IsFeatured = test.IsFeatured,
                    LinkUrl = test.LinkUrl,
                    CreatedAt = DateTime.UtcNow
                };

                // TODO: Save to database using repository
                // var savedEntity = await _freeTestRepository.AddAsync(entity);

                return new FreeTestDto
                {
                    Id = 1, // TODO: Return actual ID
                    Title = entity.Title,
                    Description = entity.Description,
                    ImageUrl = entity.ImageUrl,
                    ThumbnailUrl = entity.ThumbnailUrl,
                    QuizId = entity.QuizId,
                    ExamId = entity.ExamId,
                    DurationMinutes = entity.DurationMinutes,
                    TotalQuestions = entity.TotalQuestions,
                    TotalMarks = entity.TotalMarks,
                    IsFeatured = entity.IsFeatured,
                    LinkUrl = entity.LinkUrl
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating free test");
                throw;
            }
        }

        public async Task<FreeTestDto?> UpdateFreeTestAsync(int id, UpdateFreeTestDto test)
        {
            try
            {
                // TODO: Get existing entity from repository
                // var entity = await _freeTestRepository.GetByIdAsync(id);
                // if (entity == null) return null;

                // Update properties
                // entity.Title = test.Title ?? entity.Title;
                // entity.Description = test.Description ?? entity.Description;
                // entity.ImageUrl = test.ImageUrl ?? entity.ImageUrl;
                // entity.ThumbnailUrl = test.ThumbnailUrl ?? entity.ThumbnailUrl;
                // entity.QuizId = test.QuizId ?? entity.QuizId;
                // entity.ExamId = test.ExamId ?? entity.ExamId;
                // entity.DurationMinutes = test.DurationMinutes ?? entity.DurationMinutes;
                // entity.TotalQuestions = test.TotalQuestions ?? entity.TotalQuestions;
                // entity.TotalMarks = test.TotalMarks ?? entity.TotalMarks;
                // entity.IsFeatured = test.IsFeatured ?? entity.IsFeatured;
                // entity.LinkUrl = test.LinkUrl ?? entity.LinkUrl;
                // entity.UpdatedAt = DateTime.UtcNow;

                // await _freeTestRepository.UpdateAsync(entity);

                return new FreeTestDto
                {
                    Id = id,
                    Title = test.Title ?? "Updated Free Test",
                    Description = test.Description,
                    ImageUrl = test.ImageUrl,
                    ThumbnailUrl = test.ThumbnailUrl,
                    QuizId = test.QuizId != 0 ? test.QuizId : 1,
                    ExamId = test.ExamId != 0 ? test.ExamId : 1,
                    DurationMinutes = test.DurationMinutes != 0 ? test.DurationMinutes : 60,
                    TotalQuestions = test.TotalQuestions != 0 ? test.TotalQuestions : 50,
                    TotalMarks = test.TotalMarks != 0 ? test.TotalMarks : 100,
                    IsFeatured = test.IsFeatured,
                    LinkUrl = test.LinkUrl
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating free test");
                throw;
            }
        }

        // Daily Target Management
        public async Task<DailyTargetDto> CreateDailyTargetAsync(CreateDailyTargetDto target)
        {
            try
            {
                var entity = new DailyTarget
                {
                    Title = target.Title,
                    Description = target.Description,
                    TargetQuizzes = target.TargetQuizzes,
                    TargetMinutes = target.TargetMinutes,
                    TargetScore = target.TargetScore,
                    TargetDate = target.TargetDate,
                    CreatedAt = DateTime.UtcNow
                };

                // TODO: Save to database using repository
                // var savedEntity = await _dailyTargetRepository.AddAsync(entity);

                return new DailyTargetDto
                {
                    Id = 1, // TODO: Return actual ID
                    Title = entity.Title,
                    Description = entity.Description,
                    TargetQuizzes = entity.TargetQuizzes,
                    TargetMinutes = entity.TargetMinutes,
                    TargetScore = entity.TargetScore,
                    TargetDate = entity.TargetDate,
                    CompletedQuizzes = 0,
                    CompletedMinutes = 0,
                    AchievedScore = 0,
                    IsCompleted = false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating daily target");
                throw;
            }
        }

        public async Task<DailyTargetDto?> UpdateDailyTargetAsync(int id, UpdateDailyTargetDto target)
        {
            try
            {
                // TODO: Get existing entity from repository
                // var entity = await _dailyTargetRepository.GetByIdAsync(id);
                // if (entity == null) return null;

                // Update properties
                // entity.Title = target.Title ?? entity.Title;
                // entity.Description = target.Description ?? entity.Description;
                // entity.TargetQuizzes = target.TargetQuizzes ?? entity.TargetQuizzes;
                // entity.TargetMinutes = target.TargetMinutes ?? entity.TargetMinutes;
                // entity.TargetScore = target.TargetScore ?? entity.TargetScore;
                // entity.TargetDate = target.TargetDate ?? entity.TargetDate;
                // entity.UpdatedAt = DateTime.UtcNow;

                // await _dailyTargetRepository.UpdateAsync(entity);

                return new DailyTargetDto
                {
                    Id = id,
                    Title = target.Title ?? "Updated Target",
                    Description = target.Description,
                    TargetQuizzes = target.TargetQuizzes ?? 5,
                    TargetMinutes = target.TargetMinutes ?? 30,
                    TargetScore = target.TargetScore ?? 80,
                    TargetDate = target.TargetDate ?? DateTime.UtcNow.AddDays(1),
                    CompletedQuizzes = 0,
                    CompletedMinutes = 0,
                    AchievedScore = 0,
                    IsCompleted = false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating daily target");
                throw;
            }
        }

        // Trending Test Management
        public async Task<TrendingTestDto> CreateTrendingTestAsync(CreateTrendingTestDto test)
        {
            try
            {
                var entity = new TrendingTest
                {
                    Title = test.Title,
                    Description = test.Description,
                    ImageUrl = test.ImageUrl,
                    QuizId = test.QuizId,
                    ExamId = test.ExamId,
                    DurationMinutes = test.DurationMinutes,
                    TotalQuestions = test.TotalQuestions,
                    TotalMarks = test.TotalMarks,
                    IsFeatured = test.IsFeatured,
                    AttemptCount = test.AttemptCount,
                    AverageScore = test.AverageScore,
                    CreatedAt = DateTime.UtcNow
                };

                // TODO: Save to database using repository
                // var savedEntity = await _trendingTestRepository.AddAsync(entity);

                return new TrendingTestDto
                {
                    Id = 1, // TODO: Return actual ID
                    Title = entity.Title,
                    Description = entity.Description,
                    ImageUrl = entity.ImageUrl,
                    QuizId = entity.QuizId,
                    QuizTitle = entity.Title,
                    ExamId = entity.ExamId,
                    ExamName = "", // TODO: Get from quiz
                    DurationMinutes = entity.DurationMinutes,
                    TotalQuestions = entity.TotalQuestions,
                    TotalMarks = entity.TotalMarks,
                    IsFeatured = entity.IsFeatured,
                    AttemptCount = 0, // TODO: Calculate from data
                    TotalAttempts = 0, // TODO: Calculate from data
                    AverageScore = 0, // TODO: Calculate from data
                    Type = "" // TODO: Get from quiz
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating trending test");
                throw;
            }
        }

        public async Task<TrendingTestDto?> UpdateTrendingTestAsync(int id, UpdateTrendingTestDto test)
        {
            try
            {
                // TODO: Get existing entity from repository
                // var entity = await _trendingTestRepository.GetByIdAsync(id);
                // if (entity == null) return null;

                // Update properties
                // entity.Title = test.Title ?? entity.Title;
                // entity.Description = test.Description ?? entity.Description;
                // entity.ImageUrl = test.ImageUrl ?? entity.ImageUrl;
                // entity.QuizId = test.QuizId ?? entity.QuizId;
                // entity.ExamId = test.ExamId ?? entity.ExamId;
                // entity.DurationMinutes = test.DurationMinutes ?? entity.DurationMinutes;
                // entity.TotalQuestions = test.TotalQuestions ?? entity.TotalQuestions;
                // entity.TotalMarks = test.TotalMarks ?? entity.TotalMarks;
                // entity.IsFeatured = test.IsFeatured ?? entity.IsFeatured;
                // entity.AttemptCount = test.AttemptCount ?? entity.AttemptCount;
                // entity.AverageScore = test.AverageScore ?? entity.AverageScore;
                // entity.UpdatedAt = DateTime.UtcNow;

                // await _trendingTestRepository.UpdateAsync(entity);

                return new TrendingTestDto
                {
                    Id = id,
                    Title = test.Title ?? "Updated Trending Test",
                    Description = test.Description,
                    ImageUrl = test.ImageUrl,
                    QuizId = test.QuizId ?? 1,
                    QuizTitle = test.Title ?? "Updated Trending Test",
                    ExamId = test.ExamId ?? 1,
                    ExamName = "", // TODO: Get from quiz
                    DurationMinutes = test.DurationMinutes ?? 60,
                    TotalQuestions = test.TotalQuestions ?? 50,
                    TotalMarks = test.TotalMarks ?? 100,
                    IsFeatured = test.IsFeatured ?? false,
                    AttemptCount = test.AttemptCount ?? 100,
                    TotalAttempts = test.AttemptCount ?? 100,
                    AverageScore = test.AverageScore ?? 75.5m,
                    Type = "" // TODO: Get from quiz
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating trending test");
                throw;
            }
        }
    }
}
