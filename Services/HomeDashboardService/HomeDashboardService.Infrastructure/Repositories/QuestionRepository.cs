using Microsoft.EntityFrameworkCore;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using HomeDashboardService.Infrastructure.Data;

namespace HomeDashboardService.Infrastructure.Repositories
{
    public class QuestionRepository : GenericRepository<Question>, IQuestionRepository
    {
        public QuestionRepository(HomeDashboardDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Question>> GetByQuizIdAsync(int quizId)
        {
            return await _dbSet
                .Include(q => q.Options.Where(o => o.IsActive))
                .Where(q => q.QuizId == quizId && q.IsActive)
                .OrderBy(q => q.DisplayOrder)
                .ThenBy(q => q.Id)
                .ToListAsync();
        }

        public async Task<Question?> GetByIdWithOptionsAsync(int id)
        {
            return await _dbSet
                .Include(q => q.Options.Where(o => o.IsActive))
                .Include(q => q.Quiz)
                .FirstOrDefaultAsync(q => q.Id == id && q.IsActive);
        }

        public async Task<int> BulkInsertAsync(IEnumerable<Question> questions)
        {
            await _dbSet.AddRangeAsync(questions);
            return questions.Count();
        }
    }
}
