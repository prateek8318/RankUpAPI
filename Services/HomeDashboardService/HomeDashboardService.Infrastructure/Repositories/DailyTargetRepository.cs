using Microsoft.EntityFrameworkCore;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using HomeDashboardService.Infrastructure.Data;

namespace HomeDashboardService.Infrastructure.Repositories
{
    public class DailyTargetRepository : GenericRepository<DailyTarget>, IDailyTargetRepository
    {
        public DailyTargetRepository(HomeDashboardDbContext context) : base(context)
        {
        }

        public async Task<DailyTarget?> GetUserTargetForDateAsync(int userId, DateTime targetDate)
        {
            return await _dbSet
                .FirstOrDefaultAsync(t => t.UserId == userId && 
                                         t.TargetDate.Date == targetDate.Date && 
                                         t.IsActive);
        }

        public async Task<IEnumerable<DailyTarget>> GetUserTargetsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbSet.Where(t => t.UserId == userId && t.IsActive);

            if (startDate.HasValue)
                query = query.Where(t => t.TargetDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.TargetDate <= endDate.Value);

            return await query
                .OrderByDescending(t => t.TargetDate)
                .ToListAsync();
        }
    }
}
