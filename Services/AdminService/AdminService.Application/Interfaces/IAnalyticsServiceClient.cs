namespace AdminService.Application.Interfaces
{
    public interface IAnalyticsServiceClient
    {
        Task<object?> GetUserAnalyticsAsync(DateTime? startDate, DateTime? endDate);
        Task<object?> GetQuizAnalyticsAsync(int? quizId, DateTime? startDate, DateTime? endDate);
        Task<object?> GetSubscriptionAnalyticsAsync(DateTime? startDate, DateTime? endDate);
        Task<object?> GetLeaderboardDataAsync(int? quizId, int limit);
    }
}
