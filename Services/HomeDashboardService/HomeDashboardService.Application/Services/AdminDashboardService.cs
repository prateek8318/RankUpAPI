using HomeDashboardService.Application.DTOs;
using HomeDashboardService.Application.Interfaces;
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
    }
}
