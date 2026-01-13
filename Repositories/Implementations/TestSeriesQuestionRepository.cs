using Microsoft.EntityFrameworkCore;
using RankUpAPI.Data;
using RankUpAPI.Models;
using RankUpAPI.Repositories.Interfaces;

namespace RankUpAPI.Repositories.Implementations
{
    public class TestSeriesQuestionRepository : Repository<TestSeriesQuestion>, ITestSeriesQuestionRepository
    {
        public TestSeriesQuestionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TestSeriesQuestion>> GetByTestSeriesIdAsync(int testSeriesId)
        {
            return await _dbSet
                .Where(tsq => tsq.TestSeriesId == testSeriesId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TestSeriesQuestion>> GetByQuestionIdAsync(int questionId)
        {
            return await _dbSet
                .Where(tsq => tsq.QuestionId == questionId)
                .ToListAsync();
        }

        public async Task<TestSeriesQuestion?> GetByTestSeriesAndQuestionIdAsync(int testSeriesId, int questionId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(tsq => tsq.TestSeriesId == testSeriesId && tsq.QuestionId == questionId);
        }

        public async Task<int> GetQuestionCountByTestSeriesIdAsync(int testSeriesId)
        {
            return await _dbSet.CountAsync(tsq => tsq.TestSeriesId == testSeriesId);
        }

        public async Task<int> GetMaxOrderByTestSeriesIdAsync(int testSeriesId)
        {
            return await _dbSet
                .Where(tsq => tsq.TestSeriesId == testSeriesId)
                .Select(tsq => tsq.QuestionOrder)
                .DefaultIfEmpty(0)
                .MaxAsync();
        }

        public async Task DeleteByTestSeriesIdAsync(int testSeriesId)
        {
            var testSeriesQuestions = await GetByTestSeriesIdAsync(testSeriesId);
            await DeleteRangeAsync(testSeriesQuestions);
        }
    }
}
