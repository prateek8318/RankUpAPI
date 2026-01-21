using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Domain.Interfaces
{
    public interface IDailyTargetRepository : IRepository<DailyTarget>
    {
        Task<DailyTarget?> GetUserTargetForDateAsync(int userId, DateTime targetDate);
        Task<IEnumerable<DailyTarget>> GetUserTargetsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<DailyTarget>> GetTargetsAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}
