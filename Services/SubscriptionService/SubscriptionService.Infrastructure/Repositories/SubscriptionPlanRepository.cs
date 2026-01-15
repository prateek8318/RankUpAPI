using Microsoft.EntityFrameworkCore;
using SubscriptionService.Domain.Entities;
using SubscriptionService.Domain.Interfaces;
using SubscriptionService.Infrastructure.Data;

namespace SubscriptionService.Infrastructure.Repositories
{
    public class SubscriptionPlanRepository : GenericRepository<SubscriptionPlan>, ISubscriptionPlanRepository
    {
        public SubscriptionPlanRepository(SubscriptionDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetByExamCategoryAsync(string examCategory)
        {
            return await _dbSet
                .Where(sp => sp.ExamCategory == examCategory)
                .OrderBy(sp => sp.SortOrder)
                .ThenBy(sp => sp.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetActivePlansAsync()
        {
            return await _dbSet
                .Where(sp => sp.IsActive)
                .OrderBy(sp => sp.SortOrder)
                .ThenBy(sp => sp.Name)
                .ToListAsync();
        }

        public async Task<SubscriptionPlan?> GetByPlanTypeAsync(PlanType planType)
        {
            return await _dbSet
                .FirstOrDefaultAsync(sp => sp.Type == planType && sp.IsActive);
        }
    }
}
