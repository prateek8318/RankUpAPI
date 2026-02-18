using AdminService.Domain.Entities;

namespace AdminService.Application.Interfaces
{
    public interface ISubscriptionPlanRepository
    {
        Task<SubscriptionPlan?> GetByIdAsync(int id);
        Task<IEnumerable<SubscriptionPlan>> GetAllAsync(int page = 1, int pageSize = 50);
        Task<IEnumerable<SubscriptionPlan>> GetFilteredAsync(string? examType = null, bool? isPopular = null, bool? isRecommended = null, decimal? minPrice = null, decimal? maxPrice = null, int page = 1, int pageSize = 50);
        Task<int> GetTotalCountAsync();
        Task<int> GetFilteredCountAsync(string? examType = null, bool? isPopular = null, bool? isRecommended = null, decimal? minPrice = null, decimal? maxPrice = null);
        Task<SubscriptionPlan> AddAsync(SubscriptionPlan subscriptionPlan);
        Task UpdateAsync(SubscriptionPlan subscriptionPlan);
        Task DeleteAsync(SubscriptionPlan subscriptionPlan);
        Task<int> SaveChangesAsync();
    }
}
