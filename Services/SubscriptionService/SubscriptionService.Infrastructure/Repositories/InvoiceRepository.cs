using Microsoft.EntityFrameworkCore;
using SubscriptionService.Domain.Entities;
using SubscriptionService.Domain.Interfaces;
using SubscriptionService.Infrastructure.Data;

namespace SubscriptionService.Infrastructure.Repositories
{
    public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(SubscriptionDbContext context) : base(context)
        {
        }

        public async Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber)
        {
            return await _dbSet
                .Include(i => i.UserSubscription)
                .ThenInclude(us => us.SubscriptionPlan)
                .FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber);
        }

        public async Task<IEnumerable<Invoice>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(i => i.UserSubscription)
                .ThenInclude(us => us.SubscriptionPlan)
                .Where(i => i.UserSubscription.UserId == userId)
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }

        public async Task<Invoice?> GetByUserSubscriptionIdAsync(int userSubscriptionId)
        {
            return await _dbSet
                .Include(i => i.UserSubscription)
                .ThenInclude(us => us.SubscriptionPlan)
                .FirstOrDefaultAsync(i => i.UserSubscriptionId == userSubscriptionId);
        }

        public async Task<string> GenerateInvoiceNumberAsync()
        {
            var year = DateTime.UtcNow.Year;
            var month = DateTime.UtcNow.Month;
            
            // Format: INV-YYYYMM-XXXXX (e.g., INV-202601-00001)
            var prefix = $"INV-{year}{month:D2}-";
            
            // Get the last invoice number for this month
            var lastInvoice = await _dbSet
                .Where(i => i.InvoiceNumber.StartsWith(prefix))
                .OrderByDescending(i => i.InvoiceNumber)
                .FirstOrDefaultAsync();

            int sequence = 1;
            if (lastInvoice != null)
            {
                var lastSequence = lastInvoice.InvoiceNumber.Substring(prefix.Length);
                if (int.TryParse(lastSequence, out int lastSeq))
                {
                    sequence = lastSeq + 1;
                }
            }

            return $"{prefix}{sequence:D5}";
        }
    }
}
