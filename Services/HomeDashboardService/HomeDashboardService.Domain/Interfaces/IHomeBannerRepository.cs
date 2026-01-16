using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Domain.Interfaces
{
    public interface IHomeBannerRepository : IRepository<HomeBanner>
    {
        Task<IEnumerable<HomeBanner>> GetActiveBannersAsync();
    }
}
