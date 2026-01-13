using Microsoft.EntityFrameworkCore;
using RankUpAPI.Data;
using RankUpAPI.Models;
using RankUpAPI.Repositories.Interfaces;

namespace RankUpAPI.Repositories.Implementations
{
    public class TestSeriesRepository : Repository<TestSeries>, ITestSeriesRepository
    {
        public TestSeriesRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<TestSeries?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(ts => ts.Exam)
                .Include(ts => ts.TestSeriesQuestions)
                .FirstOrDefaultAsync(ts => ts.Id == id);
        }

        public async Task<IEnumerable<TestSeries>> GetActiveAsync()
        {
            return await _dbSet
                .Include(ts => ts.Exam)
                .Where(ts => ts.IsActive)
                .OrderBy(ts => ts.DisplayOrder)
                .ThenBy(ts => ts.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<TestSeries>> GetByExamIdAsync(int examId)
        {
            return await _dbSet
                .Include(ts => ts.Exam)
                .Where(ts => ts.ExamId == examId && ts.IsActive)
                .OrderBy(ts => ts.DisplayOrder)
                .ThenBy(ts => ts.Name)
                .ToListAsync();
        }
    }
}
