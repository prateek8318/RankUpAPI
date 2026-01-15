using Microsoft.EntityFrameworkCore;
using SubscriptionService.Domain.Entities;
using SubscriptionService.Domain.Interfaces;
using SubscriptionService.Infrastructure.Data;

namespace SubscriptionService.Infrastructure.Repositories
{
    public class UserSubscriptionRepository : GenericRepository<UserSubscription>, IUserSubscriptionRepository
    {
        public UserSubscriptionRepository(SubscriptionDbContext context) : base(context)
        {
        }

        public async Task<UserSubscription?> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(us => us.SubscriptionPlan)
                .Include(us => us.PaymentTransactions)
                .Include(us => us.Invoice)
                .Where(us => us.UserId == userId && us.Status == SubscriptionStatus.Active)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<UserSubscription>> GetByUserIdWithHistoryAsync(int userId)
        {
            return await _dbSet
                .Include(us => us.SubscriptionPlan)
                .Include(us => us.PaymentTransactions)
                .Include(us => us.Invoice)
                .Where(us => us.UserId == userId)
                .OrderByDescending(us => us.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserSubscription>> GetActiveSubscriptionsAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Include(us => us.SubscriptionPlan)
                .Where(us => us.Status == SubscriptionStatus.Active && us.EndDate > now)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserSubscription>> GetExpiringSubscriptionsAsync(int daysBeforeExpiry)
        {
            var targetDate = DateTime.UtcNow.AddDays(daysBeforeExpiry);
            return await _dbSet
                .Include(us => us.SubscriptionPlan)
                .Where(us => us.Status == SubscriptionStatus.Active && 
                           us.EndDate <= targetDate && 
                           us.EndDate > DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<UserSubscription?> GetByRazorpayOrderIdAsync(string orderId)
        {
            return await _dbSet
                .Include(us => us.SubscriptionPlan)
                .Include(us => us.PaymentTransactions)
                .Include(us => us.Invoice)
                .FirstOrDefaultAsync(us => us.RazorpayOrderId == orderId);
        }

        public async Task<UserSubscription?> GetByRazorpayPaymentIdAsync(string paymentId)
        {
            return await _dbSet
                .Include(us => us.SubscriptionPlan)
                .Include(us => us.PaymentTransactions)
                .Include(us => us.Invoice)
                .FirstOrDefaultAsync(us => us.RazorpayPaymentId == paymentId);
        }
    }
}
