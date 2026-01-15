using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Domain.Interfaces
{
    public interface IDashboardBannerRepository : IRepository<DashboardBanner>
    {
        Task<IEnumerable<DashboardBanner>> GetActiveBannersAsync();
    }
}
