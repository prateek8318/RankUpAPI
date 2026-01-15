using Microsoft.EntityFrameworkCore;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using HomeDashboardService.Infrastructure.Data;

namespace HomeDashboardService.Infrastructure.Repositories
{
    public class OfferBannerRepository : GenericRepository<OfferBanner>, IOfferBannerRepository
    {
        public OfferBannerRepository(HomeDashboardDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<OfferBanner>> GetActiveOffersAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(o => o.IsActive &&
                           (o.StartDate == null || o.StartDate <= now) &&
                           (o.EndDate == null || o.EndDate >= now))
                .OrderBy(o => o.DisplayOrder)
                .ToListAsync();
        }
    }
}
