using Microsoft.EntityFrameworkCore;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using HomeDashboardService.Infrastructure.Data;

namespace HomeDashboardService.Infrastructure.Repositories
{
    public class DailyVideoRepository : GenericRepository<DailyVideo>, IDailyVideoRepository
    {
        public DailyVideoRepository(HomeDashboardDbContext context) : base(context)
        {
        }

        public async Task<DailyVideo?> GetTodayVideoAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _dbSet
                .Where(v => v.VideoDate.Date == today && v.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task<DailyVideo?> GetByDateAsync(DateTime date)
        {
            var dateOnly = date.Date;
            return await _dbSet
                .Where(v => v.VideoDate.Date == dateOnly && v.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<DailyVideo>> GetRecentVideosAsync(int limit = 10)
        {
            return await _dbSet
                .Where(v => v.IsActive)
                .OrderByDescending(v => v.VideoDate)
                .Take(limit)
                .ToListAsync();
        }
    }
}
