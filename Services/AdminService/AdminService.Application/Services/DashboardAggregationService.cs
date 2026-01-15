using AdminService.Application.DTOs;
using AdminService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace AdminService.Application.Services
{
    public class DashboardAggregationService : IDashboardAggregationService
    {
        private readonly IUserServiceClient _userServiceClient;
        private readonly ISubscriptionServiceClient _subscriptionServiceClient;
        private readonly IQuizServiceClient _quizServiceClient;
        private readonly IAnalyticsServiceClient _analyticsServiceClient;
        private readonly ILogger<DashboardAggregationService> _logger;

        public DashboardAggregationService(
            IUserServiceClient userServiceClient,
            ISubscriptionServiceClient subscriptionServiceClient,
            IQuizServiceClient quizServiceClient,
            IAnalyticsServiceClient analyticsServiceClient,
            ILogger<DashboardAggregationService> logger)
        {
            _userServiceClient = userServiceClient;
            _subscriptionServiceClient = subscriptionServiceClient;
            _quizServiceClient = quizServiceClient;
            _analyticsServiceClient = analyticsServiceClient;
            _logger = logger;
        }

        public async Task<AdminDashboardMetricsDto> GetAggregatedDashboardMetricsAsync()
        {
            try
            {
                var tasks = new List<Task>
                {
                    Task.Run(async () => await _userServiceClient.GetTotalUsersCountAsync()),
                    Task.Run(async () => await _userServiceClient.GetDailyActiveUsersCountAsync()),
                    Task.Run(async () => await _subscriptionServiceClient.GetActiveSubscriptionsAsync()),
                    Task.Run(async () => await _quizServiceClient.GetAllQuizzesAsync())
                };

                await Task.WhenAll(tasks);

                var totalUsers = await _userServiceClient.GetTotalUsersCountAsync();
                var dau = await _userServiceClient.GetDailyActiveUsersCountAsync();
                var activeSubscriptions = await _subscriptionServiceClient.GetActiveSubscriptionsAsync();
                var quizzes = await _quizServiceClient.GetAllQuizzesAsync();

                var metrics = new AdminDashboardMetricsDto
                {
                    TotalUsers = totalUsers,
                    DailyActiveUsers = dau,
                    ActiveSubscriptions = activeSubscriptions != null ? ((System.Collections.ICollection)activeSubscriptions).Count : 0,
                    TotalQuizzes = quizzes != null ? ((System.Collections.ICollection)quizzes).Count : 0,
                    TotalQuestions = 0, // Would need QuestionService client
                    TotalExams = 0, // Would need ExamService client
                    DailyRevenue = 0, // Would need PaymentService client
                    Trends = await GetMetricTrendsAsync()
                };

                return metrics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error aggregating dashboard metrics");
                throw;
            }
        }

        public async Task<MetricTrendsDto> GetMetricTrendsAsync()
        {
            try
            {
                // This would typically compare current metrics with previous period
                // For now, returning default trends
                return new MetricTrendsDto
                {
                    UsersChangePercentage = 0,
                    SubscriptionsChangePercentage = 0,
                    RevenueChangePercentage = 0,
                    DailyActiveUsersChangePercentage = 0,
                    IsUsersUp = true,
                    IsSubscriptionsUp = true,
                    IsRevenueUp = true,
                    IsDAUUp = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting metric trends");
                throw;
            }
        }
    }
}
