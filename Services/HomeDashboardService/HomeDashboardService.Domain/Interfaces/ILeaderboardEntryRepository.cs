using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Domain.Interfaces
{
    public interface ILeaderboardEntryRepository : IRepository<LeaderboardEntry>
    {
        Task<IEnumerable<LeaderboardEntry>> GetTopEntriesAsync(int quizId, int limit = 10);
        Task<LeaderboardEntry?> GetUserRankAsync(int userId, int quizId);
        Task<IEnumerable<LeaderboardEntry>> GetUserHistoryAsync(int userId, int limit = 10);
    }
}
