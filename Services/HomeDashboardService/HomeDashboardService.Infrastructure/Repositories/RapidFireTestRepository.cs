using Microsoft.EntityFrameworkCore;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using HomeDashboardService.Infrastructure.Data;

namespace HomeDashboardService.Infrastructure.Repositories
{
    public class RapidFireTestRepository : GenericRepository<RapidFireTest>, IRapidFireTestRepository
    {
        public RapidFireTestRepository(HomeDashboardDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<RapidFireTest>> GetActiveTestsAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(t => t.IsActive &&
                           (t.StartDate == null || t.StartDate <= now) &&
                           (t.EndDate == null || t.EndDate >= now))
                .OrderBy(t => t.DisplayOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<RapidFireTest>> GetFeaturedTestsAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(t => t.IsActive && t.IsFeatured &&
                           (t.StartDate == null || t.StartDate <= now) &&
                           (t.EndDate == null || t.EndDate >= now))
                .OrderBy(t => t.DisplayOrder)
                .ToListAsync();
        }
    }
}
