using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Domain.Interfaces
{
    public interface IOfferBannerRepository : IRepository<OfferBanner>
    {
        Task<IEnumerable<OfferBanner>> GetActiveOffersAsync();
    }
}
