using Microsoft.EntityFrameworkCore;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using HomeDashboardService.Infrastructure.Data;

namespace HomeDashboardService.Infrastructure.Repositories
{
    public class QuestionOptionRepository : GenericRepository<QuestionOption>, IQuestionOptionRepository
    {
        public QuestionOptionRepository(HomeDashboardDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<QuestionOption>> GetByQuestionIdAsync(int questionId)
        {
            return await _dbSet
                .Where(o => o.QuestionId == questionId && o.IsActive)
                .OrderBy(o => o.DisplayOrder)
                .ThenBy(o => o.Id)
                .ToListAsync();
        }
    }
}
