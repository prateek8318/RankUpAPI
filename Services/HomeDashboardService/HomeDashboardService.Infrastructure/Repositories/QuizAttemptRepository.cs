using Microsoft.EntityFrameworkCore;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using HomeDashboardService.Infrastructure.Data;

namespace HomeDashboardService.Infrastructure.Repositories
{
    public class QuizAttemptRepository : GenericRepository<QuizAttempt>, IQuizAttemptRepository
    {
        public QuizAttemptRepository(HomeDashboardDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<QuizAttempt>> GetOngoingByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(a => a.Quiz)
                    .ThenInclude(q => q.Chapter)
                        .ThenInclude(c => c.Subject)
                            .ThenInclude(s => s.Exam)
                .Where(a => a.UserId == userId && 
                           a.Status == QuizAttemptStatus.InProgress && 
                           a.IsActive)
                .OrderByDescending(a => a.StartedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<QuizAttempt>> GetRecentByUserIdAsync(int userId, int limit = 10)
        {
            return await _dbSet
                .Include(a => a.Quiz)
                    .ThenInclude(q => q.Chapter)
                        .ThenInclude(c => c.Subject)
                            .ThenInclude(s => s.Exam)
                .Where(a => a.UserId == userId && a.IsActive)
                .OrderByDescending(a => a.CompletedAt ?? a.StartedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<QuizAttempt?> GetByIdWithQuizAsync(int id)
        {
            return await _dbSet
                .Include(a => a.Quiz)
                    .ThenInclude(q => q.Chapter)
                        .ThenInclude(c => c.Subject)
                            .ThenInclude(s => s.Exam)
                .FirstOrDefaultAsync(a => a.Id == id && a.IsActive);
        }

        public async Task<int> GetTotalAttemptsCountAsync(int quizId)
        {
            return await _dbSet
                .CountAsync(a => a.QuizId == quizId && a.IsActive);
        }

        public async Task<IEnumerable<QuizAttempt>> GetCompletedByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(a => a.Quiz)
                    .ThenInclude(q => q.Chapter)
                        .ThenInclude(c => c.Subject)
                            .ThenInclude(s => s.Exam)
                .Where(a => a.UserId == userId && 
                           (a.Status == QuizAttemptStatus.Completed || a.Status == QuizAttemptStatus.TimeUp) && 
                           a.IsActive)
                .OrderByDescending(a => a.CompletedAt)
                .ToListAsync();
        }
    }
}
