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

        public override async Task<SubscriptionPlan?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Translations)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public override async Task<IEnumerable<SubscriptionPlan>> GetAllAsync()
        {
            return await _dbSet
                .Include(p => p.Translations)
                .OrderBy(sp => sp.SortOrder)
                .ThenBy(sp => sp.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetByExamCategoryAsync(string examCategory)
        {
            return await _dbSet
                .Include(p => p.Translations)
                .Where(sp => sp.ExamCategory == examCategory)
                .OrderBy(sp => sp.SortOrder)
                .ThenBy(sp => sp.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetActivePlansAsync()
        {
            return await _dbSet
                .Include(p => p.Translations)
                .Where(sp => sp.IsActive)
                .OrderBy(sp => sp.SortOrder)
                .ThenBy(sp => sp.Name)
                .ToListAsync();
        }

        public async Task<SubscriptionPlan?> GetByPlanTypeAsync(PlanType planType)
        {
            return await _dbSet
                .Include(p => p.Translations)
                .FirstOrDefaultAsync(sp => sp.Type == planType && sp.IsActive);
        }

        public async Task<bool> ExistsByNameAsync(string name, string? examCategory, PlanType type, int? excludeId = null)
        {
            var query = _dbSet.AsQueryable();
            query = query.Where(p => p.Name == name && p.ExamCategory == examCategory && p.Type == type);
            if (excludeId.HasValue)
                query = query.Where(p => p.Id != excludeId.Value);

            return await query.AnyAsync();
        }
    }
}
