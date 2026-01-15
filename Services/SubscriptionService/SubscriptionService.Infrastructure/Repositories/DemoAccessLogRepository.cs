using Microsoft.EntityFrameworkCore;
using SubscriptionService.Domain.Entities;
using SubscriptionService.Domain.Interfaces;
using SubscriptionService.Infrastructure.Data;

namespace SubscriptionService.Infrastructure.Repositories
{
    public class DemoAccessLogRepository : GenericRepository<DemoAccessLog>, IDemoAccessLogRepository
    {
        public DemoAccessLogRepository(SubscriptionDbContext context) : base(context)
        {
        }

        public async Task<DemoAccessLog?> GetLastDemoAccessAsync(int userId, string examCategory)
        {
            return await _dbSet
                .Where(dal => dal.UserId == userId && dal.ExamCategory == examCategory)
                .OrderByDescending(dal => dal.AccessDate)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<DemoAccessLog>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(dal => dal.UserId == userId)
                .OrderByDescending(dal => dal.AccessDate)
                .ToListAsync();
        }

        public async Task<bool> HasUsedDemoAccessAsync(int userId, string examCategory)
        {
            return await _dbSet
                .AnyAsync(dal => dal.UserId == userId && dal.ExamCategory == examCategory);
        }
    }
}
