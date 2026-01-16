using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Domain.Interfaces
{
    public interface IFreeTestRepository : IRepository<FreeTest>
    {
        Task<IEnumerable<FreeTest>> GetActiveTestsAsync();
        Task<IEnumerable<FreeTest>> GetFeaturedTestsAsync();
    }
}
