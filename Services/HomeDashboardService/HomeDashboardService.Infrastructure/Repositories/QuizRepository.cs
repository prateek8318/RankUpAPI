using Microsoft.EntityFrameworkCore;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using HomeDashboardService.Infrastructure.Data;

namespace HomeDashboardService.Infrastructure.Repositories
{
    public class QuizRepository : GenericRepository<Quiz>, IQuizRepository
    {
        public QuizRepository(HomeDashboardDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Quiz>> GetByChapterIdAsync(int chapterId)
        {
            return await _dbSet
                .Where(q => q.ChapterId == chapterId && q.IsActive)
                .OrderBy(q => q.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Quiz>> GetActiveQuizzesAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(q => q.IsActive && 
                           (q.StartDate == null || q.StartDate <= now) &&
                           (q.EndDate == null || q.EndDate >= now))
                .OrderBy(q => q.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Quiz>> GetByTypeAsync(QuizType type)
        {
            return await _dbSet
                .Where(q => q.Type == type && q.IsActive)
                .OrderBy(q => q.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Quiz>> GetTrendingQuizzesAsync(int limit = 10)
        {
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            return await _dbSet
                .Include(q => q.QuizAttempts)
                .Where(q => q.IsActive && 
                           q.QuizAttempts.Any(a => a.StartedAt >= thirtyDaysAgo))
                .OrderByDescending(q => q.QuizAttempts.Count(a => a.StartedAt >= thirtyDaysAgo))
                .Take(limit)
                .ToListAsync();
        }

        public async Task<Quiz?> GetByIdWithQuestionsAsync(int id)
        {
            return await _dbSet
                .Include(q => q.Questions.Where(qu => qu.IsActive))
                    .ThenInclude(qu => qu.Options.Where(o => o.IsActive))
                .Include(q => q.Chapter)
                    .ThenInclude(c => c.Subject)
                        .ThenInclude(s => s.Exam)
                .FirstOrDefaultAsync(q => q.Id == id && q.IsActive);
        }
    }
}
