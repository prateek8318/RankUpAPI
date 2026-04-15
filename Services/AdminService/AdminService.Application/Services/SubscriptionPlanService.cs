using AdminService.Application.DTOs;
using AdminService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AdminService.Application.Services
{
    public class SubscriptionPlanService : ISubscriptionPlanService
    {
        private readonly ISubscriptionServiceClient _subscriptionServiceClient;
        private readonly ILogger<SubscriptionPlanService> _logger;

        public SubscriptionPlanService(
            ISubscriptionServiceClient subscriptionServiceClient,
            ILogger<SubscriptionPlanService> logger)
        {
            _subscriptionServiceClient = subscriptionServiceClient;
            _logger = logger;
        }

        public async Task<SubscriptionPlanDto> CreatePlanAsync(CreateSubscriptionPlanRequest request)
        {
            try
            {
                var result = await _subscriptionServiceClient.CreateSubscriptionPlanAsync(request);
                if (result == null)
                {
                    throw new Exception("Failed to create subscription plan");
                }

                var json = JsonSerializer.Serialize(result);
                var plan = JsonSerializer.Deserialize<SubscriptionPlanDto>(json);
                
                if (plan == null)
                {
                    throw new Exception("Failed to deserialize subscription plan response");
                }

                return plan;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription plan");
                throw;
            }
        }

        public async Task<bool> DeletePlanAsync(int id)
        {
            try
            {
                return await _subscriptionServiceClient.DeleteSubscriptionPlanAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subscription plan: {PlanId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<SubscriptionPlanDto>> GetAllPlansAsync(int page = 1, int pageSize = 50)
        {
            try
            {
                var result = await _subscriptionServiceClient.GetAllSubscriptionsAsync();
                if (result == null)
                {
                    return Enumerable.Empty<SubscriptionPlanDto>();
                }

                var json = JsonSerializer.Serialize(result);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                using var document = JsonDocument.Parse(json);
                var root = document.RootElement;
                
                if (root.TryGetProperty("data", out var dataElement))
                {
                    var plans = JsonSerializer.Deserialize<IEnumerable<SubscriptionPlanDto>>(dataElement.GetRawText(), options);
                    return plans ?? Enumerable.Empty<SubscriptionPlanDto>();
                }

                return Enumerable.Empty<SubscriptionPlanDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all subscription plans");
                throw;
            }
        }

        public async Task<SubscriptionPlanDto?> GetPlanByIdAsync(int id)
        {
            try
            {
                var result = await _subscriptionServiceClient.GetSubscriptionByIdAsync(id);
                if (result == null)
                {
                    return null;
                }

                var json = JsonSerializer.Serialize(result);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                using var document = JsonDocument.Parse(json);
                var root = document.RootElement;
                
                if (root.TryGetProperty("data", out var dataElement))
                {
                    var plan = JsonSerializer.Deserialize<SubscriptionPlanDto>(dataElement.GetRawText(), options);
                    return plan;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription plan by ID: {PlanId}", id);
                throw;
            }
        }

        public async Task<(IEnumerable<SubscriptionPlanDto> Plans, int TotalCount)> GetFilteredPlansAsync(SubscriptionPlanFilterRequest filter)
        {
            try
            {
                // For now, return all plans since filtering is not implemented in the SubscriptionService
                var plans = await GetAllPlansAsync(filter.Page, filter.PageSize);
                return (plans, plans.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting filtered subscription plans");
                throw;
            }
        }

        public async Task<SubscriptionPlanStatsDto> GetStatsAsync()
        {
            try
            {
                var result = await _subscriptionServiceClient.GetStatsAsync();
                if (result == null)
                {
                    throw new Exception("Failed to get subscription plan stats");
                }

                var json = JsonSerializer.Serialize(result);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                using var document = JsonDocument.Parse(json);
                var root = document.RootElement;
                
                if (root.TryGetProperty("data", out var dataElement))
                {
                    var stats = JsonSerializer.Deserialize<SubscriptionPlanStatsDto>(dataElement.GetRawText(), options);
                    return stats ?? throw new Exception("Failed to deserialize subscription plan stats");
                }

                throw new Exception("Invalid response format from subscription service");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription plan stats");
                throw;
            }
        }

        public async Task<SubscriptionPlanDto> UpdatePlanAsync(int id, UpdateSubscriptionPlanRequest request)
        {
            try
            {
                var result = await _subscriptionServiceClient.UpdateSubscriptionPlanAsync(id, request);
                if (result == null)
                {
                    throw new Exception("Failed to update subscription plan");
                }

                var json = JsonSerializer.Serialize(result);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                using var document = JsonDocument.Parse(json);
                var root = document.RootElement;
                
                if (root.TryGetProperty("data", out var dataElement))
                {
                    var plan = JsonSerializer.Deserialize<SubscriptionPlanDto>(dataElement.GetRawText(), options);
                    return plan ?? throw new Exception("Failed to deserialize updated subscription plan");
                }

                throw new Exception("Invalid response format from subscription service");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription plan: {PlanId}", id);
                throw;
            }
        }
    }
}
