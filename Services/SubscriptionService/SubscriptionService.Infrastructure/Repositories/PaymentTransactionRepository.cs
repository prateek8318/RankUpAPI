using Microsoft.EntityFrameworkCore;
using SubscriptionService.Domain.Entities;
using SubscriptionService.Domain.Interfaces;
using SubscriptionService.Infrastructure.Data;

namespace SubscriptionService.Infrastructure.Repositories
{
    public class PaymentTransactionRepository : GenericRepository<PaymentTransaction>, IPaymentTransactionRepository
    {
        public PaymentTransactionRepository(SubscriptionDbContext context) : base(context)
        {
        }

        public async Task<PaymentTransaction?> GetByTransactionIdAsync(string transactionId)
        {
            return await _dbSet
                .Include(pt => pt.UserSubscription)
                .FirstOrDefaultAsync(pt => pt.TransactionId == transactionId);
        }

        public async Task<PaymentTransaction?> GetByRazorpayPaymentIdAsync(string razorpayPaymentId)
        {
            return await _dbSet
                .Include(pt => pt.UserSubscription)
                .FirstOrDefaultAsync(pt => pt.RazorpayPaymentId == razorpayPaymentId);
        }

        public async Task<PaymentTransaction?> GetByRazorpayOrderIdAsync(string razorpayOrderId)
        {
            return await _dbSet
                .Include(pt => pt.UserSubscription)
                .FirstOrDefaultAsync(pt => pt.RazorpayOrderId == razorpayOrderId);
        }

        public async Task<IEnumerable<PaymentTransaction>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(pt => pt.UserSubscription)
                .Where(pt => pt.UserSubscription.UserId == userId)
                .OrderByDescending(pt => pt.CreatedAt)
                .ToListAsync();
        }
    }
}
