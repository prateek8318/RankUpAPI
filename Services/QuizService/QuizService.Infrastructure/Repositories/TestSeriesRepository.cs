using Microsoft.EntityFrameworkCore;
using QuizService.Application.Interfaces;
using QuizService.Domain.Entities;
using QuizService.Infrastructure.Data;

namespace QuizService.Infrastructure.Repositories
{
    public class TestSeriesRepository : ITestSeriesRepository
    {
        private readonly QuizDbContext _context;

        public TestSeriesRepository(QuizDbContext context)
        {
            _context = context;
        }

        public async Task<TestSeries?> GetByIdAsync(int id)
        {
            return await _context.TestSeries.FindAsync(id);
        }

        public async Task<IEnumerable<TestSeries>> GetAllAsync()
        {
            return await _context.TestSeries.Where(ts => ts.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<TestSeries>> GetByExamIdAsync(int examId)
        {
            return await _context.TestSeries
                .Where(ts => ts.ExamId == examId && ts.IsActive)
                .OrderBy(ts => ts.DisplayOrder)
                .ToListAsync();
        }

        public async Task<TestSeries> AddAsync(TestSeries testSeries)
        {
            await _context.TestSeries.AddAsync(testSeries);
            return testSeries;
        }

        public Task UpdateAsync(TestSeries testSeries)
        {
            _context.TestSeries.Update(testSeries);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(TestSeries testSeries)
        {
            _context.TestSeries.Remove(testSeries);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
