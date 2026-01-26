using Microsoft.EntityFrameworkCore;
using TestService.Domain.Entities;
using TestService.Domain.Interfaces;
using TestService.Infrastructure.Data;

namespace TestService.Infrastructure.Repositories
{
    public class TestSeriesRepository : GenericRepository<TestSeries>, ITestSeriesRepository
    {
        public TestSeriesRepository(TestDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TestSeries>> GetByExamIdAsync(int examId)
        {
            return await _dbSet
                .Include(ts => ts.Exam)
                .Include(ts => ts.Tests)
                    .ThenInclude(t => t.PracticeMode)
                .Where(ts => ts.ExamId == examId)
                .OrderBy(ts => ts.DisplayOrder)
                .ToListAsync();
        }

        public async Task<TestSeries?> GetByIdWithTestsAsync(int id)
        {
            return await _dbSet
                .Include(ts => ts.Exam)
                .Include(ts => ts.Tests)
                    .ThenInclude(t => t.PracticeMode)
                .Include(ts => ts.Tests)
                    .ThenInclude(t => t.Subject)
                .FirstOrDefaultAsync(ts => ts.Id == id);
        }

        public async Task<IEnumerable<TestSeries>> GetActiveSeriesAsync()
        {
            return await _dbSet
                .Include(ts => ts.Exam)
                .Include(ts => ts.Tests)
                    .ThenInclude(t => t.PracticeMode)
                .Where(ts => ts.IsActive)
                .OrderBy(ts => ts.DisplayOrder)
                .ToListAsync();
        }
    }
}
