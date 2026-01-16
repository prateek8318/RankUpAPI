using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Domain.Interfaces
{
    public interface ISubscriptionBannerRepository : IRepository<SubscriptionBanner>
    {
        Task<IEnumerable<SubscriptionBanner>> GetActiveBannersAsync(bool forSubscribedUsers = false);
    }
}
