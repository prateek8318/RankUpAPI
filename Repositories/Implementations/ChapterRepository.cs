using Microsoft.EntityFrameworkCore;
using RankUpAPI.Data;
using RankUpAPI.Models;
using RankUpAPI.Repositories.Interfaces;

namespace RankUpAPI.Repositories.Implementations
{
    public class ChapterRepository : Repository<Chapter>, IChapterRepository
    {
        public ChapterRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Chapter?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Subject)
                    .ThenInclude(s => s.Exam)
                .Include(c => c.Questions)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Chapter>> GetActiveAsync()
        {
            return await _dbSet
                .Include(c => c.Subject)
                    .ThenInclude(s => s.Exam)
                .Include(c => c.Questions)
                .Where(c => c.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Chapter>> GetBySubjectIdAsync(int subjectId)
        {
            return await _dbSet
                .Include(c => c.Subject)
                    .ThenInclude(s => s.Exam)
                .Include(c => c.Questions)
                .Where(c => c.SubjectId == subjectId && c.IsActive)
                .ToListAsync();
        }
    }
}
