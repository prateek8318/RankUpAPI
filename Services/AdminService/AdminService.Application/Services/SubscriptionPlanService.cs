using AdminService.Application.DTOs;
using AdminService.Application.Interfaces;
using AdminService.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace AdminService.Application.Services
{
    public class SubscriptionPlanService : ISubscriptionPlanService
    {
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly ILogger<SubscriptionPlanService> _logger;

        public SubscriptionPlanService(
            ISubscriptionPlanRepository subscriptionPlanRepository,
            ILogger<SubscriptionPlanService> logger)
        {
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _logger = logger;
        }

        public async Task<SubscriptionPlanDto> CreatePlanAsync(CreateSubscriptionPlanRequest request)
        {
            try
            {
                var subscriptionPlan = new SubscriptionPlan
                {
                    PlanName = request.PlanName,
                    ExamType = request.ExamType,
                    Price = request.Price,
                    Duration = request.Duration,
                    ColorCode = request.ColorCode,
                    IsPopular = request.IsPopular,
                    IsRecommended = request.IsRecommended,
                    IsActive = request.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                var createdPlan = await _subscriptionPlanRepository.AddAsync(subscriptionPlan);

                return MapToDto(createdPlan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription plan");
                throw;
            }
        }

        public async Task<SubscriptionPlanDto?> GetPlanByIdAsync(int id)
        {
            try
            {
                var plan = await _subscriptionPlanRepository.GetByIdAsync(id);
                return plan != null ? MapToDto(plan) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription plan by ID: {PlanId}", id);
                return null;
            }
        }

        public async Task<IEnumerable<SubscriptionPlanDto>> GetAllPlansAsync(int page = 1, int pageSize = 50)
        {
            try
            {
                var plans = await _subscriptionPlanRepository.GetAllAsync(page, pageSize);
                return plans.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all subscription plans");
                return new List<SubscriptionPlanDto>();
            }
        }

        public async Task<(IEnumerable<SubscriptionPlanDto> Plans, int TotalCount)> GetFilteredPlansAsync(SubscriptionPlanFilterRequest filter)
        {
            try
            {
                var plans = await _subscriptionPlanRepository.GetFilteredAsync(
                    filter.ExamType,
                    filter.IsPopular,
                    filter.IsRecommended,
                    filter.MinPrice,
                    filter.MaxPrice,
                    filter.Page,
                    filter.PageSize);

                var totalCount = await _subscriptionPlanRepository.GetFilteredCountAsync(
                    filter.ExamType,
                    filter.IsPopular,
                    filter.IsRecommended,
                    filter.MinPrice,
                    filter.MaxPrice);

                return (plans.Select(MapToDto), totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting filtered subscription plans");
                return (new List<SubscriptionPlanDto>(), 0);
            }
        }

        public async Task<SubscriptionPlanDto> UpdatePlanAsync(int id, UpdateSubscriptionPlanRequest request)
        {
            try
            {
                var existingPlan = await _subscriptionPlanRepository.GetByIdAsync(id);
                if (existingPlan == null)
                    throw new KeyNotFoundException($"Subscription plan with ID {id} not found");

                existingPlan.PlanName = request.PlanName;
                existingPlan.ExamType = request.ExamType;
                existingPlan.Price = request.Price;
                existingPlan.Duration = request.Duration;
                existingPlan.ColorCode = request.ColorCode;
                existingPlan.IsPopular = request.IsPopular;
                existingPlan.IsRecommended = request.IsRecommended;
                existingPlan.IsActive = request.IsActive;
                existingPlan.UpdatedAt = DateTime.UtcNow;

                await _subscriptionPlanRepository.UpdateAsync(existingPlan);

                return MapToDto(existingPlan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription plan: {PlanId}", id);
                throw;
            }
        }

        public async Task<bool> DeletePlanAsync(int id)
        {
            try
            {
                var plan = await _subscriptionPlanRepository.GetByIdAsync(id);
                if (plan == null)
                    return false;

                await _subscriptionPlanRepository.DeleteAsync(plan);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subscription plan: {PlanId}", id);
                return false;
            }
        }

        public async Task<SubscriptionPlanStatsDto> GetStatsAsync()
        {
            try
            {
                var allPlans = await _subscriptionPlanRepository.GetAllAsync(1, 10000);
                
                var activePlans = allPlans.Count(p => p.IsActive);
                var monthlyRevenue = allPlans.Where(p => p.IsActive).Sum(p => p.Price);
                var expiringSoon = 0; // TODO: Implement based on actual subscription end dates
                var newSubscribers = 0; // TODO: Implement based on actual subscription data

                return new SubscriptionPlanStatsDto
                {
                    ActivePlans = activePlans,
                    MonthlyRevenue = monthlyRevenue,
                    ExpiringSoon = expiringSoon,
                    NewSubscribers = newSubscribers
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription plan stats");
                return new SubscriptionPlanStatsDto();
            }
        }

        private static SubscriptionPlanDto MapToDto(SubscriptionPlan plan)
        {
            return new SubscriptionPlanDto
            {
                Id = plan.Id,
                PlanName = plan.PlanName,
                ExamType = plan.ExamType,
                Price = plan.Price,
                Duration = plan.Duration,
                ColorCode = plan.ColorCode,
                IsPopular = plan.IsPopular,
                IsRecommended = plan.IsRecommended,
                IsActive = plan.IsActive,
                CreatedAt = plan.CreatedAt,
                UpdatedAt = plan.UpdatedAt
            };
        }
    }
}
