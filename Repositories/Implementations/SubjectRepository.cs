using Microsoft.EntityFrameworkCore;
using RankUpAPI.Data;
using RankUpAPI.Models;
using RankUpAPI.Repositories.Interfaces;

namespace RankUpAPI.Repositories.Implementations
{
    public class SubjectRepository : Repository<Subject>, ISubjectRepository
    {
        public SubjectRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Subject?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(s => s.Exam)
                .Include(s => s.Chapters)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Subject>> GetActiveAsync()
        {
            return await _dbSet
                .Include(s => s.Exam)
                .Include(s => s.Chapters)
                .Where(s => s.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Subject>> GetByExamIdAsync(int examId)
        {
            return await _dbSet
                .Include(s => s.Exam)
                .Include(s => s.Chapters)
                .Where(s => s.ExamId == examId && s.IsActive)
                .ToListAsync();
        }
    }
}
