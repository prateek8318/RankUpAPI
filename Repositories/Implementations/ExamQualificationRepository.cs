using Microsoft.EntityFrameworkCore;
using RankUpAPI.Data;
using RankUpAPI.Models;
using RankUpAPI.Repositories.Interfaces;

namespace RankUpAPI.Repositories.Implementations
{
    public class ExamQualificationRepository : Repository<ExamQualification>, IExamQualificationRepository
    {
        public ExamQualificationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ExamQualification>> GetByExamIdAsync(int examId)
        {
            return await _dbSet.Where(eq => eq.ExamId == examId).ToListAsync();
        }

        public async Task<IEnumerable<ExamQualification>> GetByQualificationIdAsync(int qualificationId)
        {
            return await _dbSet.Where(eq => eq.QualificationId == qualificationId).ToListAsync();
        }

        public async Task<bool> ExistsAsync(int examId, int qualificationId)
        {
            return await _dbSet.AnyAsync(eq => eq.ExamId == examId && eq.QualificationId == qualificationId);
        }

        public async Task<bool> HasExamsForQualificationAsync(int qualificationId)
        {
            return await _dbSet.AnyAsync(eq => eq.QualificationId == qualificationId);
        }

        public async Task DeleteByExamIdAsync(int examId)
        {
            var examQualifications = await GetByExamIdAsync(examId);
            await DeleteRangeAsync(examQualifications);
        }
    }
}
