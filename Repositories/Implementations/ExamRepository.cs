using Microsoft.EntityFrameworkCore;
using RankUpAPI.Data;
using RankUpAPI.Models;
using RankUpAPI.Repositories.Interfaces;

namespace RankUpAPI.Repositories.Implementations
{
    public class ExamRepository : Repository<Exam>, IExamRepository
    {
        public ExamRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Exam?> GetByIdWithQualificationsAsync(int id)
        {
            return await _dbSet
                .Include(e => e.ExamQualifications)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Exam>> GetActiveAsync()
        {
            return await _dbSet.Where(e => e.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Exam>> GetByQualificationIdAsync(int qualificationId)
        {
            return await _dbSet
                .Include(e => e.ExamQualifications)
                .Where(e => e.ExamQualifications.Any(eq => eq.QualificationId == qualificationId) && e.IsActive)
                .ToListAsync();
        }
    }
}
