using Microsoft.EntityFrameworkCore;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using HomeDashboardService.Infrastructure.Data;

namespace HomeDashboardService.Infrastructure.Repositories
{
    public class LeaderboardEntryRepository : GenericRepository<LeaderboardEntry>, ILeaderboardEntryRepository
    {
        public LeaderboardEntryRepository(HomeDashboardDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<LeaderboardEntry>> GetTopEntriesAsync(int quizId, int limit = 10)
        {
            return await _dbSet
                .Include(e => e.Quiz)
                .Where(e => e.QuizId == quizId && e.IsActive)
                .OrderBy(e => e.Rank)
                .ThenByDescending(e => e.Score)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<LeaderboardEntry?> GetUserRankAsync(int userId, int quizId)
        {
            return await _dbSet
                .Include(e => e.Quiz)
                .FirstOrDefaultAsync(e => e.UserId == userId && e.QuizId == quizId && e.IsActive);
        }

        public async Task<IEnumerable<LeaderboardEntry>> GetUserHistoryAsync(int userId, int limit = 10)
        {
            return await _dbSet
                .Include(e => e.Quiz)
                    .ThenInclude(q => q.Chapter)
                        .ThenInclude(c => c.Subject)
                            .ThenInclude(s => s.Exam)
                .Where(e => e.UserId == userId && e.IsActive)
                .OrderByDescending(e => e.AttemptDate)
                .Take(limit)
                .ToListAsync();
        }
    }
}
