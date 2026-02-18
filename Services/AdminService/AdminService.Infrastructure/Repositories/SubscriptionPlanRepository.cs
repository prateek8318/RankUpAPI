using AdminService.Application.DTOs;
using AdminService.Application.Interfaces;
using AdminService.Domain.Entities;
using AdminService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AdminService.Infrastructure.Repositories
{
    public class SubscriptionPlanRepository : ISubscriptionPlanRepository
    {
        private readonly AdminDbContext _context;
        private readonly ILogger<SubscriptionPlanRepository> _logger;

        public SubscriptionPlanRepository(AdminDbContext context, ILogger<SubscriptionPlanRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<SubscriptionPlan?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.SubscriptionPlans
                    .FirstOrDefaultAsync(sp => sp.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription plan by ID: {PlanId}", id);
                return null;
            }
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetAllAsync(int page = 1, int pageSize = 50)
        {
            try
            {
                return await _context.SubscriptionPlans
                    .OrderByDescending(sp => sp.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all subscription plans");
                return new List<SubscriptionPlan>();
            }
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetFilteredAsync(
            string? examType = null,
            bool? isPopular = null,
            bool? isRecommended = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int page = 1,
            int pageSize = 50)
        {
            try
            {
                var query = _context.SubscriptionPlans.AsQueryable();

                if (!string.IsNullOrEmpty(examType))
                    query = query.Where(sp => sp.ExamType.ToLower() == examType.ToLower());

                if (isPopular.HasValue)
                    query = query.Where(sp => sp.IsPopular == isPopular.Value);

                if (isRecommended.HasValue)
                    query = query.Where(sp => sp.IsRecommended == isRecommended.Value);

                if (minPrice.HasValue)
                    query = query.Where(sp => sp.Price >= minPrice.Value);

                if (maxPrice.HasValue)
                    query = query.Where(sp => sp.Price <= maxPrice.Value);

                return await query
                    .OrderByDescending(sp => sp.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting filtered subscription plans");
                return new List<SubscriptionPlan>();
            }
        }

        public async Task<int> GetTotalCountAsync()
        {
            try
            {
                return await _context.SubscriptionPlans.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total subscription plans count");
                return 0;
            }
        }

        public async Task<int> GetFilteredCountAsync(
            string? examType = null,
            bool? isPopular = null,
            bool? isRecommended = null,
            decimal? minPrice = null,
            decimal? maxPrice = null)
        {
            try
            {
                var query = _context.SubscriptionPlans.AsQueryable();

                if (!string.IsNullOrEmpty(examType))
                    query = query.Where(sp => sp.ExamType.ToLower() == examType.ToLower());

                if (isPopular.HasValue)
                    query = query.Where(sp => sp.IsPopular == isPopular.Value);

                if (isRecommended.HasValue)
                    query = query.Where(sp => sp.IsRecommended == isRecommended.Value);

                if (minPrice.HasValue)
                    query = query.Where(sp => sp.Price >= minPrice.Value);

                if (maxPrice.HasValue)
                    query = query.Where(sp => sp.Price <= maxPrice.Value);

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting filtered subscription plans count");
                return 0;
            }
        }

        public async Task<SubscriptionPlan> AddAsync(SubscriptionPlan subscriptionPlan)
        {
            try
            {
                await _context.SubscriptionPlans.AddAsync(subscriptionPlan);
                await _context.SaveChangesAsync();
                return subscriptionPlan;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding subscription plan");
                throw;
            }
        }

        public async Task UpdateAsync(SubscriptionPlan subscriptionPlan)
        {
            try
            {
                _context.SubscriptionPlans.Update(subscriptionPlan);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription plan: {PlanId}", subscriptionPlan.Id);
                throw;
            }
        }

        public async Task DeleteAsync(SubscriptionPlan subscriptionPlan)
        {
            try
            {
                _context.SubscriptionPlans.Remove(subscriptionPlan);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subscription plan: {PlanId}", subscriptionPlan.Id);
                throw;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
