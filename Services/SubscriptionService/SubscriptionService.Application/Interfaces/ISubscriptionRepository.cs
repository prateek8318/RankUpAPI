using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Application.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task<Subscription?> GetByIdAsync(int id);
        Task<IEnumerable<Subscription>> GetAllAsync();
        Task<IEnumerable<Subscription>> GetByUserIdAsync(int userId);
        Task<Subscription> AddAsync(Subscription subscription);
        Task UpdateAsync(Subscription subscription);
        Task<int> SaveChangesAsync();
    }
}
