using AutoMapper;
using Microsoft.Extensions.Logging;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Interfaces;
using SubscriptionService.Domain.Entities;
using SubscriptionService.Domain.Interfaces;

namespace SubscriptionService.Application.Services
{
    public class SubscriptionPlanService : ISubscriptionPlanService
    {
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SubscriptionPlanService> _logger;

        public SubscriptionPlanService(
            ISubscriptionPlanRepository subscriptionPlanRepository,
            IMapper mapper,
            ILogger<SubscriptionPlanService> logger)
        {
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<SubscriptionPlanDto> CreatePlanAsync(CreateSubscriptionPlanDto createPlanDto)
        {
            try
            {
                _logger.LogInformation("Creating new subscription plan: {PlanName}", createPlanDto.Name);

                var plan = _mapper.Map<SubscriptionPlan>(createPlanDto);
                var createdPlan = await _subscriptionPlanRepository.AddAsync(plan);
                await _subscriptionPlanRepository.SaveChangesAsync();

                var result = _mapper.Map<SubscriptionPlanDto>(createdPlan);
                
                _logger.LogInformation("Successfully created subscription plan with ID: {PlanId}", result.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription plan: {PlanName}", createPlanDto.Name);
                throw;
            }
        }

        public async Task<SubscriptionPlanDto> UpdatePlanAsync(int id, UpdateSubscriptionPlanDto updatePlanDto)
        {
            try
            {
                _logger.LogInformation("Updating subscription plan with ID: {PlanId}", id);

                var existingPlan = await _subscriptionPlanRepository.GetByIdAsync(id);
                if (existingPlan == null)
                {
                    throw new KeyNotFoundException($"Subscription plan with ID {id} not found");
                }

                _mapper.Map(updatePlanDto, existingPlan);
                existingPlan.UpdatedAt = DateTime.UtcNow;

                var updatedPlan = await _subscriptionPlanRepository.UpdateAsync(existingPlan);
                await _subscriptionPlanRepository.SaveChangesAsync();

                var result = _mapper.Map<SubscriptionPlanDto>(updatedPlan);
                
                _logger.LogInformation("Successfully updated subscription plan with ID: {PlanId}", result.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription plan with ID: {PlanId}", id);
                throw;
            }
        }

        public async Task<bool> DeletePlanAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting subscription plan with ID: {PlanId}", id);

                var existingPlan = await _subscriptionPlanRepository.GetByIdAsync(id);
                if (existingPlan == null)
                {
                    return false;
                }

                // Soft delete by setting IsActive to false
                existingPlan.IsActive = false;
                existingPlan.UpdatedAt = DateTime.UtcNow;

                await _subscriptionPlanRepository.UpdateAsync(existingPlan);
                await _subscriptionPlanRepository.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted subscription plan with ID: {PlanId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subscription plan with ID: {PlanId}", id);
                throw;
            }
        }

        public async Task<SubscriptionPlanDto?> GetPlanByIdAsync(int id)
        {
            try
            {
                var plan = await _subscriptionPlanRepository.GetByIdAsync(id);
                return plan != null ? _mapper.Map<SubscriptionPlanDto>(plan) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plan with ID: {PlanId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<SubscriptionPlanListDto>> GetAllPlansAsync()
        {
            try
            {
                var plans = await _subscriptionPlanRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<SubscriptionPlanListDto>>(plans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all subscription plans");
                throw;
            }
        }

        public async Task<IEnumerable<SubscriptionPlanListDto>> GetPlansByExamCategoryAsync(string examCategory)
        {
            try
            {
                var plans = await _subscriptionPlanRepository.GetByExamCategoryAsync(examCategory);
                return _mapper.Map<IEnumerable<SubscriptionPlanListDto>>(plans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plans for exam category: {ExamCategory}", examCategory);
                throw;
            }
        }

        public async Task<IEnumerable<SubscriptionPlanListDto>> GetActivePlansAsync()
        {
            try
            {
                var plans = await _subscriptionPlanRepository.GetActivePlansAsync();
                return _mapper.Map<IEnumerable<SubscriptionPlanListDto>>(plans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active subscription plans");
                throw;
            }
        }
    }
}
