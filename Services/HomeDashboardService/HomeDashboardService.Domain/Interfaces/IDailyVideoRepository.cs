using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Domain.Interfaces
{
    public interface IDailyVideoRepository : IRepository<DailyVideo>
    {
        Task<DailyVideo?> GetTodayVideoAsync();
        Task<DailyVideo?> GetByDateAsync(DateTime date);
        Task<IEnumerable<DailyVideo>> GetRecentVideosAsync(int limit = 10);
    }
}
