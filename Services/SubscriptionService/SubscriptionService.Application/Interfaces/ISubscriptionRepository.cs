using SubscriptionService.Domain.Entities;
using Common.DTOs;

namespace SubscriptionService.Application.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task<Subscription?> GetByIdAsync(int id);
        Task<IEnumerable<Subscription>> GetAllAsync();
        Task<PaginatedResponse<Subscription>> GetAllAsync(PaginationRequest pagination);
        Task<IEnumerable<Subscription>> GetByUserIdAsync(int userId);
        Task<PaginatedResponse<Subscription>> GetByUserIdAsync(int userId, PaginationRequest pagination);
        Task<Subscription> AddAsync(Subscription subscription);
        Task UpdateAsync(Subscription subscription);
        Task<int> SaveChangesAsync();
    }
}
