using AdminService.Application.DTOs;
using AdminService.Application.Interfaces;
using AdminService.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AdminService.Application.Services
{
    public class SubscriptionPlanService : ISubscriptionPlanService
    {
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly ISubscriptionServiceClient _subscriptionServiceClient;
        private readonly ILogger<SubscriptionPlanService> _logger;

        public SubscriptionPlanService(
            ISubscriptionPlanRepository subscriptionPlanRepository,
            ISubscriptionServiceClient subscriptionServiceClient,
            ILogger<SubscriptionPlanService> logger)
        {
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _subscriptionServiceClient = subscriptionServiceClient;
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
                
                // Get all user subscriptions from SubscriptionService
                var allUserSubscriptions = await _subscriptionServiceClient.GetAllUserSubscriptionsAsync();
                var expiringSubscriptions = await _subscriptionServiceClient.GetExpiringUserSubscriptionsAsync(30);
                
                var now = DateTime.UtcNow;
                var thirtyDaysAgo = now.AddDays(-30);
                var thirtyDaysFromNow = now.AddDays(30);
                
                // Calculate monthly revenue from active subscriptions created this month
                decimal monthlyRevenue = 0;
                int newSubscribers = 0;
                int expiringSoon = 0;
                
                if (allUserSubscriptions != null)
                {
                    foreach (var subscription in allUserSubscriptions)
                    {
                        // Parse subscription data - System.Text.Json uses PascalCase by default
                        // Try both PascalCase and camelCase for compatibility
                        var createdAtKey = subscription.ContainsKey("CreatedAt") ? "CreatedAt" : 
                                         subscription.ContainsKey("createdAt") ? "createdAt" : null;
                        
                        if (createdAtKey != null && subscription.TryGetValue(createdAtKey, out var createdAtObj))
                        {
                            DateTime createdAt;
                            if (createdAtObj is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.String)
                            {
                                if (DateTime.TryParse(jsonElement.GetString(), out createdAt))
                                {
                                    // Count new subscribers (created in last 30 days)
                                    if (createdAt >= thirtyDaysAgo)
                                    {
                                        newSubscribers++;
                                    }
                                }
                            }
                            else if (DateTime.TryParse(createdAtObj?.ToString(), out createdAt))
                            {
                                // Count new subscribers (created in last 30 days)
                                if (createdAt >= thirtyDaysAgo)
                                {
                                    newSubscribers++;
                                }
                            }
                        }
                        
                        // Calculate monthly revenue from active subscriptions created this month
                        var statusKey = subscription.ContainsKey("Status") ? "Status" : 
                                      subscription.ContainsKey("status") ? "status" : null;
                        
                        if (statusKey != null && subscription.TryGetValue(statusKey, out var statusObj))
                        {
                            var statusValue = statusObj?.ToString();
                            if (statusValue == "Active" || statusValue == "0") // 0 might be enum value
                            {
                                if (createdAtKey != null && subscription.TryGetValue(createdAtKey, out var subCreatedAtObj))
                                {
                                    DateTime subCreatedAt;
                                    bool dateParsed = false;
                                    
                                    if (subCreatedAtObj is JsonElement dateJsonElement && dateJsonElement.ValueKind == JsonValueKind.String)
                                    {
                                        dateParsed = DateTime.TryParse(dateJsonElement.GetString(), out subCreatedAt);
                                    }
                                    else
                                    {
                                        dateParsed = DateTime.TryParse(subCreatedAtObj?.ToString(), out subCreatedAt);
                                    }
                                    
                                    if (dateParsed && subCreatedAt >= thirtyDaysAgo)
                                    {
                                        var finalAmountKey = subscription.ContainsKey("FinalAmount") ? "FinalAmount" : 
                                                           subscription.ContainsKey("finalAmount") ? "finalAmount" : null;
                                        
                                        if (finalAmountKey != null && subscription.TryGetValue(finalAmountKey, out var finalAmountObj))
                                        {
                                            decimal finalAmount;
                                            if (finalAmountObj is JsonElement amountJsonElement)
                                            {
                                                if (amountJsonElement.ValueKind == JsonValueKind.Number && 
                                                    amountJsonElement.TryGetDecimal(out finalAmount))
                                                {
                                                    monthlyRevenue += finalAmount;
                                                }
                                            }
                                            else if (decimal.TryParse(finalAmountObj?.ToString(), out finalAmount))
                                            {
                                                monthlyRevenue += finalAmount;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                
                // Count expiring subscriptions
                if (expiringSubscriptions != null)
                {
                    expiringSoon = expiringSubscriptions.Count;
                }

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
