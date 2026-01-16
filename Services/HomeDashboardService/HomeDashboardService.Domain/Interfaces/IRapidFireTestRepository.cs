using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Domain.Interfaces
{
    public interface IRapidFireTestRepository : IRepository<RapidFireTest>
    {
        Task<IEnumerable<RapidFireTest>> GetActiveTestsAsync();
        Task<IEnumerable<RapidFireTest>> GetFeaturedTestsAsync();
    }
}
