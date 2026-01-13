using Microsoft.EntityFrameworkCore;
using SubscriptionService.Application.Interfaces;
using SubscriptionService.Domain.Entities;
using SubscriptionService.Infrastructure.Data;

namespace SubscriptionService.Infrastructure.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly SubscriptionDbContext _context;

        public SubscriptionRepository(SubscriptionDbContext context)
        {
            _context = context;
        }

        public async Task<Subscription?> GetByIdAsync(int id)
        {
            return await _context.Subscriptions.FindAsync(id);
        }

        public async Task<IEnumerable<Subscription>> GetAllAsync()
        {
            return await _context.Subscriptions.Where(s => s.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Subscription>> GetByUserIdAsync(int userId)
        {
            return await _context.Subscriptions
                .Where(s => s.UserId == userId && s.IsActive)
                .ToListAsync();
        }

        public async Task<Subscription> AddAsync(Subscription subscription)
        {
            await _context.Subscriptions.AddAsync(subscription);
            return subscription;
        }

        public Task UpdateAsync(Subscription subscription)
        {
            _context.Subscriptions.Update(subscription);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
