using Microsoft.EntityFrameworkCore;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using HomeDashboardService.Infrastructure.Data;

namespace HomeDashboardService.Infrastructure.Repositories
{
    public class HomeBannerRepository : GenericRepository<HomeBanner>, IHomeBannerRepository
    {
        public HomeBannerRepository(HomeDashboardDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<HomeBanner>> GetActiveBannersAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(b => b.IsActive &&
                           (b.StartDate == null || b.StartDate <= now) &&
                           (b.EndDate == null || b.EndDate >= now))
                .OrderBy(b => b.DisplayOrder)
                .ThenByDescending(b => b.IsPromoted)
                .ToListAsync();
        }
    }
}
