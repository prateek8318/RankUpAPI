namespace AdminService.Application.Interfaces
{
    public interface ISubscriptionServiceClient
    {
        Task<object?> GetAllSubscriptionsAsync();
        Task<object?> GetSubscriptionByIdAsync(int id);
        Task<object?> GetActiveSubscriptionsAsync();
        Task<object?> CreateSubscriptionPlanAsync(object createDto);
        Task<object?> UpdateSubscriptionPlanAsync(int id, object updateDto);
        Task<bool> DeleteSubscriptionPlanAsync(int id);
        Task<List<Dictionary<string, object>>?> GetAllUserSubscriptionsAsync();
        Task<List<Dictionary<string, object>>?> GetExpiringUserSubscriptionsAsync(int daysBeforeExpiry = 30);
    }
}
