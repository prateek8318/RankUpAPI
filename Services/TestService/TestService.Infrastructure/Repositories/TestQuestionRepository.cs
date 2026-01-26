using Microsoft.EntityFrameworkCore;
using TestService.Domain.Entities;
using TestService.Domain.Interfaces;
using TestService.Infrastructure.Data;

namespace TestService.Infrastructure.Repositories
{
    public class TestQuestionRepository : GenericRepository<TestQuestion>, ITestQuestionRepository
    {
        public TestQuestionRepository(TestDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TestQuestion>> GetByTestIdAsync(int testId)
        {
            return await _dbSet
                .Include(tq => tq.Question)
                .Where(tq => tq.TestId == testId)
                .OrderBy(tq => tq.DisplayOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<TestQuestion>> GetByQuestionIdAsync(int questionId)
        {
            return await _dbSet
                .Include(tq => tq.Test)
                .Where(tq => tq.QuestionId == questionId)
                .ToListAsync();
        }
    }
}
