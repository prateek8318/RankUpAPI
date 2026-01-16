using Microsoft.EntityFrameworkCore;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using HomeDashboardService.Infrastructure.Data;

namespace HomeDashboardService.Infrastructure.Repositories
{
    public class SubscriptionBannerRepository : GenericRepository<SubscriptionBanner>, ISubscriptionBannerRepository
    {
        public SubscriptionBannerRepository(HomeDashboardDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SubscriptionBanner>> GetActiveBannersAsync(bool forSubscribedUsers = false)
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(b => b.IsActive &&
                           (b.StartDate == null || b.StartDate <= now) &&
                           (b.EndDate == null || b.EndDate >= now) &&
                           (forSubscribedUsers ? b.ShowToSubscribedUsers : b.ShowToNonSubscribedUsers))
                .OrderBy(b => b.DisplayOrder)
                .ToListAsync();
        }
    }
}
