using AutoMapper;

using Microsoft.Extensions.Logging;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Interfaces;
using SubscriptionService.Domain.Entities;
using SubscriptionService.Domain.Interfaces;
using System.Reflection;


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

                var duplicate = await _subscriptionPlanRepository.ExistsByNameAsync(createPlanDto.Name, createPlanDto.ExamCategory, createPlanDto.Type, examId: createPlanDto.ExamId);
                if (duplicate)
                    throw new InvalidOperationException($"Subscription plan '{createPlanDto.Name}' already exists for this exam type and duration type.");

                var plan = _mapper.Map<SubscriptionPlan>(createPlanDto);
                plan.CreatedAt = DateTime.UtcNow;
                plan.IsActive = true;
                if (plan.ValidityDays <= 0)
                    plan.ValidityDays = ComputeValidityDays(plan.Duration, plan.DurationType);

                // Save non-English translations (English comes from base fields)
                if (createPlanDto.Translations != null && createPlanDto.Translations.Count > 0)
                {
                    foreach (var t in createPlanDto.Translations)
                    {
                        if (string.IsNullOrWhiteSpace(t.LanguageCode) || t.LanguageCode.Trim().ToLowerInvariant() == "en")
                            continue;

                        plan.Translations.Add(new SubscriptionPlanTranslation
                        {
                            LanguageCode = t.LanguageCode.Trim().ToLowerInvariant(),
                            Name = t.Name,
                            Description = t.Description,
                            Features = t.Features ?? new List<string>()
                        });
                    }
                }

                var createdPlan = await _subscriptionPlanRepository.AddAsync(plan);

                var result = _mapper.Map<SubscriptionPlanDto>(createdPlan);
                result.ExamType = result.ExamCategory;
                result.Translations = createdPlan.Translations
                    .Select(t => new SubscriptionPlanTranslationDto
                    {
                        LanguageCode = t.LanguageCode,
                        Name = t.Name,
                        Description = t.Description,
                        Features = t.Features
                    }).ToList();
                
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

                var duplicate = await _subscriptionPlanRepository.ExistsByNameAsync(updatePlanDto.Name, updatePlanDto.ExamCategory, updatePlanDto.Type, excludeId: id, examId: updatePlanDto.ExamId);
                if (duplicate)
                    throw new InvalidOperationException($"Subscription plan '{updatePlanDto.Name}' already exists for this exam type and duration type.");

                _mapper.Map(updatePlanDto, existingPlan);
                existingPlan.UpdatedAt = DateTime.UtcNow;
                if (existingPlan.ValidityDays <= 0)
                    existingPlan.ValidityDays = ComputeValidityDays(existingPlan.Duration, existingPlan.DurationType);

                // Replace translations
                existingPlan.Translations.Clear();
                if (updatePlanDto.Translations != null && updatePlanDto.Translations.Count > 0)
                {
                    foreach (var t in updatePlanDto.Translations)
                    {
                        if (string.IsNullOrWhiteSpace(t.LanguageCode) || t.LanguageCode.Trim().ToLowerInvariant() == "en")
                            continue;

                        existingPlan.Translations.Add(new SubscriptionPlanTranslation
                        {
                            LanguageCode = t.LanguageCode.Trim().ToLowerInvariant(),
                            Name = t.Name,
                            Description = t.Description,
                            Features = t.Features ?? new List<string>()
                        });
                    }
                }

                var updatedPlan = await _subscriptionPlanRepository.UpdateAsync(existingPlan);

                var result = _mapper.Map<SubscriptionPlanDto>(updatedPlan);
                result.ExamType = result.ExamCategory;
                result.Translations = updatedPlan.Translations
                    .Select(t => new SubscriptionPlanTranslationDto
                    {
                        LanguageCode = t.LanguageCode,
                        Name = t.Name,
                        Description = t.Description,
                        Features = t.Features
                    }).ToList();
                
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

                _logger.LogInformation("Successfully deleted subscription plan with ID: {PlanId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subscription plan with ID: {PlanId}", id);
                throw;
            }
        }

        public async Task<SubscriptionPlanDto?> GetPlanByIdAsync(int id, string? language = null)
        {
            try
            {
                var plan = await _subscriptionPlanRepository.GetByIdAsync(id);
                if (plan == null) return null;

                var dto = _mapper.Map<SubscriptionPlanDto>(plan);
                ApplyLocalization(dto, plan, language);
                dto.Translations = plan.Translations.Select(t => new SubscriptionPlanTranslationDto
                {
                    LanguageCode = t.LanguageCode,
                    Name = t.Name,
                    Description = t.Description,
                    Features = t.Features
                }).ToList();
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plan with ID: {PlanId}", id);
                throw;
            }
        }

        public async Task<SubscriptionPlanPagedResponseDto> GetPlansPagedAsync(SubscriptionPlanPagedRequestDto request)
        {
            try
            {
                var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
                var pageSize = request.PageSize < 1 ? 20 : request.PageSize;

                var (plans, totalCount) = await _subscriptionPlanRepository.GetPagedAsync(
                    pageNumber,
                    pageSize,
                    request.IncludeInactive,
                    request.ExamId);

                var list = _mapper.Map<List<SubscriptionPlanListDto>>(plans);
                ApplyLocalization(list, plans, request.Language);

                var totalPages = totalCount <= 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

                return new SubscriptionPlanPagedResponseDto
                {
                    Items = list,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paginated subscription plans");
                throw;
            }
        }

        public async Task<IEnumerable<SubscriptionPlanListDto>> GetAllPlansAsync(string? language = null)
        {
            try
            {
                var plans = await _subscriptionPlanRepository.GetAllAsync();
                var list = _mapper.Map<IEnumerable<SubscriptionPlanListDto>>(plans).ToList();
                ApplyLocalization(list, plans, language);
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all subscription plans");
                throw;
            }
        }

        public async Task<IEnumerable<SubscriptionPlanListDto>> GetPlansByExamCategoryAsync(string examCategory, string? language = null)
        {
            try
            {
                var plans = await _subscriptionPlanRepository.GetByExamCategoryAsync(examCategory);
                var list = _mapper.Map<IEnumerable<SubscriptionPlanListDto>>(plans).ToList();
                ApplyLocalization(list, plans, language);
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plans for exam category: {ExamCategory}", examCategory);
                throw;
            }
        }

        public async Task<IEnumerable<SubscriptionPlanListDto>> GetPlansByExamIdAsync(int examId, string? language = null)
        {
            try
            {
                var plans = await _subscriptionPlanRepository.GetByExamIdAsync(examId);
                var list = _mapper.Map<IEnumerable<SubscriptionPlanListDto>>(plans).ToList();
                ApplyLocalization(list, plans, language);
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plans for exam id: {ExamId}", examId);
                throw;
            }
        }

        public async Task<IEnumerable<SubscriptionPlanListDto>> GetActivePlansAsync(string? language = null)
        {
            try
            {
                var plans = await _subscriptionPlanRepository.GetActivePlansAsync();
                var list = _mapper.Map<IEnumerable<SubscriptionPlanListDto>>(plans).ToList();
                ApplyLocalization(list, plans, language);
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active subscription plans");
                throw;
            }
        }

        private static string NormalizeLanguage(string? language)
        {
            if (string.IsNullOrWhiteSpace(language)) return "en";
            return language.Trim().ToLowerInvariant();
        }

        private static int ComputeValidityDays(int duration, string durationType)
        {
            if (duration <= 0) duration = 1;
            var dt = (durationType ?? "Monthly").Trim().ToLowerInvariant();
            return dt switch
            {
                "daily" => duration,
                "weekly" => duration * 7,
                "yearly" => duration * 365,
                _ => duration * 30 // monthly default
            };
        }

        private static void ApplyLocalization(SubscriptionPlanDto dto, SubscriptionPlan entity, string? language)
        {
            var lang = NormalizeLanguage(language);
            dto.ExamType = dto.ExamCategory;

            if (lang == "en") return; // English is base fields

            var t = entity.Translations.FirstOrDefault(x => x.LanguageCode == lang);
            if (t == null) return; // fallback to English

            dto.Name = t.Name;
            dto.Description = t.Description;
            dto.Features = t.Features ?? new List<string>();
        }

        private static void ApplyLocalization(List<SubscriptionPlanListDto> dtos, IEnumerable<SubscriptionPlan> entities, string? language)
        {
            var lang = NormalizeLanguage(language);
            foreach (var dto in dtos)
            {
                dto.ExamType = dto.ExamCategory;
                if (lang == "en") continue;

                var entity = entities.FirstOrDefault(e => e.Id == dto.Id);
                if (entity == null) continue;

                var t = entity.Translations.FirstOrDefault(x => x.LanguageCode == lang);
                if (t == null) continue;

                dto.Name = t.Name;
                dto.Description = t.Description;
                dto.Features = t.Features ?? new List<string>();
            }
        }

        public async Task<SubscriptionPlanStatsDto> GetStatsAsync()
        {
            try
            {
                var stats = new SubscriptionPlanStatsDto();

                // Get active plans count
                var activePlans = await _subscriptionPlanRepository.GetActivePlansAsync();
                stats.ActivePlans = activePlans.Count();

                // Get monthly revenue (current month)
                var monthlyRevenue = await GetMonthlyRevenueAsync();
                stats.MonthlyRevenue = monthlyRevenue;

                // Get expiring soon count (next 7 days)
                var expiringSoon = await GetExpiringSoonCountAsync();
                stats.ExpiringSoon = expiringSoon;

                // Get new subscribers count (last 30 days)
                var newSubscribers = await GetNewSubscribersCountAsync();
                stats.NewSubscribers = newSubscribers;

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plan statistics");
                throw;
            }
        }

        private async Task<decimal> GetMonthlyRevenueAsync()
        {
            try
            {
                // For now, return 0 as we need to implement payment repository
                // TODO: Implement payment repository to get actual revenue data
                _logger.LogInformation("Monthly revenue calculation not yet implemented - returning 0");
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating monthly revenue");
                return 0;
            }
        }

        private async Task<int> GetExpiringSoonCountAsync()
        {
            try
            {
                // For now, return 0 as we need to implement user subscription repository
                // TODO: Implement user subscription repository to get actual expiring data
                _logger.LogInformation("Expiring soon calculation not yet implemented - returning 0");
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting expiring soon subscriptions");
                return 0;
            }
        }

        private async Task<int> GetNewSubscribersCountAsync()
        {
            try
            {
                // For now, return 0 as we need to implement user subscription repository
                // TODO: Implement user subscription repository to get actual subscriber data
                _logger.LogInformation("New subscribers calculation not yet implemented - returning 0");
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting new subscribers");
                return 0;
            }
        }
    }
}
