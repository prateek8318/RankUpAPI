using Microsoft.EntityFrameworkCore;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using HomeDashboardService.Infrastructure.Data;

namespace HomeDashboardService.Infrastructure.Repositories
{
    public class PracticeModeRepository : GenericRepository<PracticeMode>, IPracticeModeRepository
    {
        public PracticeModeRepository(HomeDashboardDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PracticeMode>> GetActiveModesAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(m => m.IsActive)
                .OrderBy(m => m.DisplayOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<PracticeMode>> GetFeaturedModesAsync()
        {
            return await _dbSet
                .Where(m => m.IsActive && m.IsFeatured)
                .OrderBy(m => m.DisplayOrder)
                .ToListAsync();
        }
    }
}
