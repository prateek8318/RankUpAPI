using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Domain.Interfaces
{
    public interface IPracticeModeRepository : IRepository<PracticeMode>
    {
        Task<IEnumerable<PracticeMode>> GetActiveModesAsync();
        Task<IEnumerable<PracticeMode>> GetFeaturedModesAsync();
    }
}
