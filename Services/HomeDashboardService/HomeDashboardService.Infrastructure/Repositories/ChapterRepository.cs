using Microsoft.EntityFrameworkCore;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using HomeDashboardService.Infrastructure.Data;

namespace HomeDashboardService.Infrastructure.Repositories
{
    public class ChapterRepository : GenericRepository<Chapter>, IChapterRepository
    {
        public ChapterRepository(HomeDashboardDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Chapter>> GetBySubjectIdAsync(int subjectId)
        {
            return await _dbSet
                .Where(c => c.SubjectId == subjectId && c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Chapter?> GetByIdWithQuizzesAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Quizzes.Where(q => q.IsActive))
                .Include(c => c.Subject)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
        }
    }
}
