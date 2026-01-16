using Microsoft.EntityFrameworkCore;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using HomeDashboardService.Infrastructure.Data;

namespace HomeDashboardService.Infrastructure.Repositories
{
    public class ContinuePracticeItemRepository : GenericRepository<ContinuePracticeItem>, IContinuePracticeItemRepository
    {
        public ContinuePracticeItemRepository(HomeDashboardDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ContinuePracticeItem>> GetUserActiveItemsAsync(int userId)
        {
            return await _dbSet
                .Where(i => i.UserId == userId && i.IsActive)
                .OrderByDescending(i => i.LastAccessedAt ?? i.StartedAt)
                .ToListAsync();
        }

        public async Task<ContinuePracticeItem?> GetByQuizAttemptIdAsync(int userId, int quizAttemptId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(i => i.UserId == userId && 
                                        i.QuizAttemptId == quizAttemptId && 
                                        i.IsActive);
        }
    }
}
