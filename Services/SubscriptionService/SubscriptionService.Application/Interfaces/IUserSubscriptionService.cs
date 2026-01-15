using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Application.Interfaces
{
    public interface IUserSubscriptionService
    {
        Task<UserSubscriptionDto> CreateSubscriptionAsync(CreateUserSubscriptionDto createSubscriptionDto);
        Task<PaymentVerificationResultDto> ActivateSubscriptionAsync(ActivateSubscriptionDto activateSubscriptionDto);
        Task<UserSubscriptionDto> RenewSubscriptionAsync(RenewSubscriptionDto renewSubscriptionDto);
        Task<bool> CancelSubscriptionAsync(CancelSubscriptionDto cancelSubscriptionDto);
        Task<UserSubscriptionDto?> GetSubscriptionByIdAsync(int id);
        Task<UserSubscriptionDto?> GetMySubscriptionAsync(int userId);
        Task<SubscriptionHistoryDto> GetUserSubscriptionHistoryAsync(int userId);
        Task<IEnumerable<UserSubscriptionDto>> GetAllUserSubscriptionsAsync();
        Task<IEnumerable<UserSubscriptionDto>> GetActiveSubscriptionsAsync();
        Task<IEnumerable<UserSubscriptionDto>> GetExpiringSubscriptionsAsync(int daysBeforeExpiry);
    }
}
