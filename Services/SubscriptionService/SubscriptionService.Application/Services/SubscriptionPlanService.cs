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
        private readonly IImageUploadService _imageUploadService;

        public SubscriptionPlanService(
            ISubscriptionPlanRepository subscriptionPlanRepository,
            IMapper mapper,
            ILogger<SubscriptionPlanService> logger,
            IImageUploadService imageUploadService)
        {
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _mapper = mapper;
            _logger = logger;
            _imageUploadService = imageUploadService;
        }

        public async Task<SubscriptionPlanDto> CreatePlanAsync(CreateSubscriptionPlanDto createPlanDto)
        {
            try
            {
                _logger.LogInformation("Creating new subscription plan: {PlanName}", createPlanDto.Name);

                var duplicate = await _subscriptionPlanRepository.ExistsByNameAsync(createPlanDto.Name, createPlanDto.ExamCategory, createPlanDto.Type, examId: createPlanDto.ExamId);
                if (duplicate)
                    throw new InvalidOperationException($"Subscription plan '{createPlanDto.Name}' already exists for this exam type and duration type.");

                // Handle image upload
                string? imageUrl = createPlanDto.ImageUrl;
                if (createPlanDto.ImageFile != null)
                {
                    imageUrl = await _imageUploadService.UploadImageAsync(createPlanDto.ImageFile);
                }

                var plan = _mapper.Map<SubscriptionPlan>(createPlanDto);
                plan.ImageUrl = imageUrl;
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

                // Handle image upload
                string? imageUrl = updatePlanDto.ImageUrl;
                if (updatePlanDto.ImageFile != null)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(existingPlan.ImageUrl))
                    {
                        _imageUploadService.DeleteImage(existingPlan.ImageUrl);
                    }
                    
                    // Upload new image
                    imageUrl = await _imageUploadService.UploadImageAsync(updatePlanDto.ImageFile);
                }

                _mapper.Map(updatePlanDto, existingPlan);
                existingPlan.ImageUrl = imageUrl;
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
                var result = _mapper.Map<IEnumerable<SubscriptionPlanListDto>>(plans);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active subscription plans");
                throw;
            }
        }

        public async Task<IEnumerable<SubscriptionPlanListDto>> GetActivePlansAsync(string? language = null, int? examId = null, int? userId = null)
        {
            try
            {
                // Filter by exam ID - ExamCategory contains ExamId from Master Service
                IEnumerable<SubscriptionPlan> plans;
                if (examId.HasValue)
                {
                    // Get plans where ExamCategory matches user's selected ExamId
                    plans = await _subscriptionPlanRepository.GetActivePlansAsync();
                    plans = plans.Where(p => p.ExamCategory == examId.Value.ToString());
                }
                else
                {
                    plans = await _subscriptionPlanRepository.GetActivePlansAsync();
                }

                // Filter out inactive plans
                plans = plans.Where(p => p.IsActive);

                // Filter out plans that user has already purchased
                if (userId.HasValue)
                {
                    var purchasedPlanIds = new HashSet<int>();
                    var userSubscriptions = await _subscriptionPlanRepository.GetUserActiveSubscriptionsAsync(userId.Value);
                    
                    foreach (var subscription in userSubscriptions)
                    {
                        purchasedPlanIds.Add(subscription.SubscriptionPlanId);
                    }

                    plans = plans.Where(plan => !purchasedPlanIds.Contains(plan.Id));
                    
                    _logger.LogInformation("Filtered out {Count} already purchased plans for user {UserId}", 
                        purchasedPlanIds.Count, userId);
                }

                var result = _mapper.Map<IEnumerable<SubscriptionPlanListDto>>(plans);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active subscription plans");
                throw;
            }
        }

        public async Task<IEnumerable<SubscriptionPlanListDto>> GetActivePlansByExamIdsAsync(string? language, List<int> examIds)
        {
            try
            {
                if (examIds == null || !examIds.Any())
                {
                    _logger.LogWarning("No exam IDs provided for filtering plans");
                    return new List<SubscriptionPlanListDto>();
                }

                // Get all active plans first
                var allPlans = await _subscriptionPlanRepository.GetActivePlansAsync();
                
                // Filter plans by user's selected exam IDs
                // Check if ExamCategory matches any of the user's selected exam IDs
                var filteredPlans = allPlans.Where(plan => 
                {
                    // Try to parse ExamCategory as integer to match with exam IDs
                    if (int.TryParse(plan.ExamCategory, out int planExamId))
                    {
                        return examIds.Contains(planExamId);
                    }
                    return false;
                }).ToList();

                _logger.LogInformation("Filtered {TotalCount} plans to {FilteredCount} plans based on exam IDs: {ExamIds}", 
                    allPlans.Count(), filteredPlans.Count(), string.Join(", ", examIds));

                var result = _mapper.Map<IEnumerable<SubscriptionPlanListDto>>(filteredPlans);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active subscription plans filtered by exam IDs: {ExamIds}", string.Join(", ", examIds));
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
                // Get current month start and end dates
                var now = DateTime.UtcNow;
                var monthStart = new DateTime(now.Year, now.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                // Get payments from current month
                var payments = await _subscriptionPlanRepository.GetPaymentsByDateRangeAsync(monthStart, monthEnd);
                var monthlyRevenue = payments.Where(p => p.Status == PaymentStatus.Success)
                                            .Sum(p => p.FinalAmount);

                _logger.LogInformation("Monthly revenue calculated: {Revenue} for {Month}-{Year}", 
                    monthlyRevenue, now.Month, now.Year);
                
                return monthlyRevenue;
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
                // Get subscriptions expiring in next 7 days
                var now = DateTime.UtcNow;
                var sevenDaysFromNow = now.AddDays(7);

                var expiringSubscriptions = await _subscriptionPlanRepository.GetExpiringSubscriptionsAsync(now, sevenDaysFromNow);
                var expiringSoonCount = expiringSubscriptions.Count(s => s.Status == "Active");

                _logger.LogInformation("Expiring soon subscriptions count: {Count} for next 7 days", expiringSoonCount);
                
                return expiringSoonCount;
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
                // Get new subscribers from last 30 days
                var now = DateTime.UtcNow;
                var thirtyDaysAgo = now.AddDays(-30);

                var newSubscriptions = await _subscriptionPlanRepository.GetNewSubscriptionsAsync(thirtyDaysAgo, now);
                var newSubscribersCount = newSubscriptions.Count(s => s.Status == "Active" || s.Status == "Pending");

                _logger.LogInformation("New subscribers count: {Count} for last 30 days", newSubscribersCount);
                
                return newSubscribersCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting new subscribers");
                return 0;
            }
        }

        // New methods for duration options support
        public async Task<PlanWithDurationOptionsDto> CreatePlanWithDurationsAsync(CreateSubscriptionPlanWithDurationDto createPlanDto)
        {
            try
            {
                _logger.LogInformation("Creating new subscription plan with durations: {PlanName}", createPlanDto.Name);

                // Check for duplicate plan - ExamCategory contains ExamId from Master Service
                var duplicate = await _subscriptionPlanRepository.ExistsByNameAsync(createPlanDto.Name, createPlanDto.ExamCategory, createPlanDto.Type, examId: createPlanDto.ExamId);
                if (duplicate)
                    throw new InvalidOperationException($"Subscription plan '{createPlanDto.Name}' already exists for this exam type.");

                // Handle image upload
                string? imageUrl = createPlanDto.ImageUrl;
                if (createPlanDto.ImageFile != null)
                {
                    imageUrl = await _imageUploadService.UploadImageAsync(createPlanDto.ImageFile);
                }

                // Create base plan
                var plan = new SubscriptionPlan
                {
                    Name = createPlanDto.Name,
                    Description = createPlanDto.Description,
                    Type = createPlanDto.Type,
                    Price = createPlanDto.BasePrice, // Use base price for 1 month
                    Currency = createPlanDto.Currency,
                    TestPapersCount = createPlanDto.TestPapersCount,
                    Discount = 0,
                    Duration = 1, // Default to 1 month
                    DurationType = "Monthly",
                    ValidityDays = 30, // Default to 30 days
                    ExamId = createPlanDto.ExamId,
                    ExamCategory = createPlanDto.ExamCategory,
                    Features = createPlanDto.Features ?? new List<string>(),
                    ImageUrl = imageUrl,
                    IsPopular = createPlanDto.IsPopular,
                    IsRecommended = createPlanDto.IsRecommended,
                    CardColorTheme = createPlanDto.CardColorTheme,
                    SortOrder = createPlanDto.SortOrder,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                // Prepare duration options
                var durationOptions = createPlanDto.DurationOptions.Select(dto => new PlanDurationOption
                {
                    DurationMonths = dto.DurationMonths,
                    Price = dto.Price,
                    DiscountPercentage = dto.DiscountPercentage,
                    DisplayLabel = dto.DisplayLabel,
                    IsPopular = dto.IsPopular,
                    SortOrder = dto.SortOrder,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                // Prepare translations
                var translations = createPlanDto.Translations?
                    .Where(t => !string.IsNullOrWhiteSpace(t.LanguageCode) && t.LanguageCode.Trim().ToLowerInvariant() != "en")
                    .Select(t => new SubscriptionPlanTranslation
                    {
                        LanguageCode = t.LanguageCode.Trim().ToLowerInvariant(),
                        Name = t.Name,
                        Description = t.Description,
                        Features = t.Features ?? new List<string>()
                    }).ToList();

                // Create plan with durations and translations in a single transaction
                var createdPlan = await _subscriptionPlanRepository.CreatePlanWithDurationsAsync(plan, durationOptions, translations);

                var result = await GetPlanWithDurationsAsync(createdPlan.Id, "en");
                if (result == null)
                {
                    throw new InvalidOperationException("Failed to retrieve created plan with durations");
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating plan with durations: {PlanName}", createPlanDto.Name);
                throw;
            }
        }

        public async Task<PlanWithDurationOptionsDto> UpsertPlanWithDurationsAsync(CreateSubscriptionPlanWithDurationDto dto)
        {
            if (dto.Id.HasValue && dto.Id.Value > 0)
            {
                return await UpdatePlanWithDurationsAsync(dto.Id.Value, dto);
            }

            return await CreatePlanWithDurationsAsync(dto);
        }

        private async Task<PlanWithDurationOptionsDto> UpdatePlanWithDurationsAsync(int planId, CreateSubscriptionPlanWithDurationDto dto)
        {
            try
            {
                _logger.LogInformation("Updating subscription plan with durations: {PlanId}", planId);

                // Handle image upload (keep existing if not provided)
                var existing = await _subscriptionPlanRepository.GetByIdAsync(planId);
                if (existing == null)
                    throw new KeyNotFoundException($"Subscription plan with ID {planId} not found");

                string? imageUrl = dto.ImageUrl ?? existing.ImageUrl;
                if (dto.ImageFile != null)
                {
                    imageUrl = await _imageUploadService.UploadImageAsync(dto.ImageFile);
                }

                var plan = new SubscriptionPlan
                {
                    Id = planId,
                    Name = dto.Name,
                    Description = dto.Description,
                    Type = dto.Type,
                    Price = dto.BasePrice,
                    Currency = dto.Currency,
                    TestPapersCount = dto.TestPapersCount,
                    Discount = 0,
                    Duration = 1,
                    DurationType = "Monthly",
                    ValidityDays = 30,
                    ExamId = dto.ExamId,
                    ExamCategory = dto.ExamCategory,
                    Features = dto.Features ?? new List<string>(),
                    ImageUrl = imageUrl,
                    IsPopular = dto.IsPopular,
                    IsRecommended = dto.IsRecommended,
                    CardColorTheme = dto.CardColorTheme,
                    SortOrder = dto.SortOrder,
                    IsActive = existing.IsActive,
                    CreatedAt = existing.CreatedAt,
                    UpdatedAt = DateTime.UtcNow
                };

                var durationOptions = dto.DurationOptions.Select(x => new PlanDurationOption
                {
                    DurationMonths = x.DurationMonths,
                    Price = x.Price,
                    DiscountPercentage = x.DiscountPercentage,
                    DisplayLabel = x.DisplayLabel,
                    IsPopular = x.IsPopular,
                    SortOrder = x.SortOrder,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                var translations = dto.Translations?
                    .Where(t => !string.IsNullOrWhiteSpace(t.LanguageCode) && t.LanguageCode.Trim().ToLowerInvariant() != "en")
                    .Select(t => new SubscriptionPlanTranslation
                    {
                        LanguageCode = t.LanguageCode.Trim().ToLowerInvariant(),
                        Name = t.Name,
                        Description = t.Description,
                        Features = t.Features ?? new List<string>()
                    }).ToList();

                var updatedPlan = await _subscriptionPlanRepository.UpdatePlanWithDurationsAsync(planId, plan, durationOptions, translations);

                var result = await GetPlanWithDurationsAsync(updatedPlan.Id, "en");
                if (result == null)
                    throw new InvalidOperationException("Failed to retrieve updated plan with durations");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating plan with durations: {PlanId}", planId);
                throw;
            }
        }

        public async Task<PlanWithDurationOptionsDto?> GetPlanWithDurationsAsync(int id, string? language = null, int? userId = null)
        {
            try
            {
                var plan = await _subscriptionPlanRepository.GetPlanWithDurationsAsync(id, language ?? "en");
                if (plan == null)
                    return null;

                // Hide plan if already purchased by user
                if (userId.HasValue)
                {
                    var existingSubscription = await _subscriptionPlanRepository.GetUserActiveSubscriptionAsync(userId.Value, id);
                    if (existingSubscription != null)
                    {
                        _logger.LogInformation("User {UserId} has already purchased plan {PlanId}, hiding from view", userId, id);
                        return null;
                    }
                }

                return _mapper.Map<PlanWithDurationOptionsDto>(plan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving plan with durations: {PlanId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<PlanWithDurationOptionsDto>> GetActivePlansWithDurationsAsync(string? language = null, int? examId = null, int? userId = null)
        {
            try
            {
                var plans = await _subscriptionPlanRepository.GetActivePlansWithDurationsAsync(language ?? "en", examId);
                var planDtos = _mapper.Map<IEnumerable<PlanWithDurationOptionsDto>>(plans).ToList();

                // Additional filtering by ExamCategory if examId is provided
                if (examId.HasValue)
                {
                    planDtos = planDtos.Where(plan => plan.ExamCategory == examId.Value.ToString()).ToList();
                }

                // Filter out plans that user has already purchased
                if (userId.HasValue)
                {
                    var purchasedPlanIds = new HashSet<int>();
                    var userSubscriptions = await _subscriptionPlanRepository.GetUserActiveSubscriptionsAsync(userId.Value);
                    
                    foreach (var subscription in userSubscriptions)
                    {
                        purchasedPlanIds.Add(subscription.SubscriptionPlanId);
                    }

                    planDtos = planDtos.Where(plan => !purchasedPlanIds.Contains(plan.Id)).ToList();
                    
                    _logger.LogInformation("Filtered out {Count} already purchased plans for user {UserId}", 
                        purchasedPlanIds.Count, userId);
                }

                return planDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active plans with durations");
                throw;
            }
        }

        public async Task<bool> AddDurationOptionsAsync(int planId, List<CreatePlanDurationOptionDto> durationOptions)
        {
            try
            {
                var plan = await _subscriptionPlanRepository.GetByIdAsync(planId);
                if (plan == null)
                    throw new KeyNotFoundException($"Subscription plan with ID {planId} not found");

                foreach (var dto in durationOptions)
                {
                    var option = new PlanDurationOption
                    {
                        SubscriptionPlanId = planId,
                        DurationMonths = dto.DurationMonths,
                        Price = dto.Price,
                        DiscountPercentage = dto.DiscountPercentage,
                        DisplayLabel = dto.DisplayLabel,
                        IsPopular = dto.IsPopular,
                        SortOrder = dto.SortOrder,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _subscriptionPlanRepository.AddDurationOptionAsync(option);
                }

                await _subscriptionPlanRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding duration options to plan: {PlanId}", planId);
                throw;
            }
        }

        public async Task<PlanDurationOptionDto> UpdateDurationOptionAsync(int durationOptionId, UpdatePlanDurationOptionDto updateDto)
        {
            try
            {
                var option = await _subscriptionPlanRepository.GetDurationOptionAsync(durationOptionId);
                if (option == null)
                    throw new KeyNotFoundException($"Duration option with ID {durationOptionId} not found");

                option.DurationMonths = updateDto.DurationMonths;
                option.Price = updateDto.Price;
                option.DiscountPercentage = updateDto.DiscountPercentage;
                option.DisplayLabel = updateDto.DisplayLabel;
                option.IsPopular = updateDto.IsPopular;
                option.SortOrder = updateDto.SortOrder;
                option.IsActive = updateDto.IsActive;
                option.UpdatedAt = DateTime.UtcNow;

                await _subscriptionPlanRepository.UpdateDurationOptionAsync(option);
                await _subscriptionPlanRepository.SaveChangesAsync();

                return _mapper.Map<PlanDurationOptionDto>(option);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating duration option: {DurationOptionId}", durationOptionId);
                throw;
            }
        }

        public async Task<bool> DeleteDurationOptionAsync(int durationOptionId)
        {
            try
            {
                var option = await _subscriptionPlanRepository.GetDurationOptionAsync(durationOptionId);
                if (option == null)
                    return false;

                option.IsActive = false;
                option.UpdatedAt = DateTime.UtcNow;

                await _subscriptionPlanRepository.UpdateDurationOptionAsync(option);
                await _subscriptionPlanRepository.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting duration option: {DurationOptionId}", durationOptionId);
                throw;
            }
        }

        public async Task<PurchasePlanResponseDto> PurchasePlanAsync(int userId, PurchasePlanRequestDto purchaseRequest)
        {
            try
            {
                // Get plan with duration options
                var plan = await _subscriptionPlanRepository.GetPlanWithDurationsAsync(purchaseRequest.PlanId, "en");
                if (plan == null)
                    throw new KeyNotFoundException($"Subscription plan with ID {purchaseRequest.PlanId} not found");

                var durationOption = plan.DurationOptions.FirstOrDefault(d => d.Id == purchaseRequest.DurationOptionId);
                if (durationOption == null)
                    throw new KeyNotFoundException($"Duration option with ID {purchaseRequest.DurationOptionId} not found");

                // Check if user already has active subscription for this plan
                var existingSubscription = await _subscriptionPlanRepository.GetUserActiveSubscriptionAsync(userId, purchaseRequest.PlanId);
                if (existingSubscription != null)
                    throw new InvalidOperationException("You already have an active subscription for this plan");

                // Create Razorpay order (this would integrate with payment service)
                // For now, return a mock response
                var response = new PurchasePlanResponseDto
                {
                    Success = true,
                    Message = "Order created successfully",
                    RazorpayOrderId = $"order_{Guid.NewGuid():N}",
                    Amount = durationOption.EffectivePrice * 100, // Convert to paise
                    Currency = purchaseRequest.Currency,
                    Plan = _mapper.Map<PlanWithDurationOptionsDto>(plan),
                    SelectedDuration = _mapper.Map<PlanDurationOptionDto>(durationOption)
                };

                _logger.LogInformation("Created purchase order for user {UserId}, plan {PlanId}, duration {DurationMonths}", 
                    userId, purchaseRequest.PlanId, durationOption.DurationMonths);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing plan purchase for user {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<PlanWithDurationOptionsDto>> GetAllPlansWithDurationsAsync(string? language = null, bool includeInactive = false)
        {
            try
            {
                var currentLanguage = language ?? "en";
                var plans = await _subscriptionPlanRepository.GetAllPlansWithDurationsAsync(currentLanguage, null);
                var planDtos = _mapper.Map<IEnumerable<PlanWithDurationOptionsDto>>(plans).ToList();

                // Filter out inactive plans if not requested
                if (!includeInactive)
                {
                    planDtos = planDtos.Where(plan => plan.IsActive).ToList();
                }

                return planDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all subscription plans with durations");
                throw;
            }
        }

        
        public async Task<SubscriptionPlanDto> TogglePopularAsync(int id)
        {
            try
            {
                var plan = await _subscriptionPlanRepository.GetByIdAsync(id);
                if (plan == null)
                {
                    throw new KeyNotFoundException($"Subscription plan with ID {id} not found");
                }

                plan.IsPopular = !plan.IsPopular;
                plan.UpdatedAt = DateTime.UtcNow;

                var updatedPlan = await _subscriptionPlanRepository.UpdateAsync(plan);
                var result = _mapper.Map<SubscriptionPlanDto>(updatedPlan);
                
                _logger.LogInformation("Successfully toggled popular status for plan: {PlanId}", id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling popular status for plan: {PlanId}", id);
                throw;
            }
        }

        public async Task<SubscriptionPlanDto> ToggleRecommendedAsync(int id)
        {
            try
            {
                var plan = await _subscriptionPlanRepository.GetByIdAsync(id);
                if (plan == null)
                {
                    throw new KeyNotFoundException($"Subscription plan with ID {id} not found");
                }

                plan.IsRecommended = !plan.IsRecommended;
                plan.UpdatedAt = DateTime.UtcNow;

                var updatedPlan = await _subscriptionPlanRepository.UpdateAsync(plan);
                var result = _mapper.Map<SubscriptionPlanDto>(updatedPlan);
                
                _logger.LogInformation("Successfully toggled recommended status for plan: {PlanId}", id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling recommended status for plan: {PlanId}", id);
                throw;
            }
        }

        public async Task<SubscriptionPlanDto> ToggleStatusAsync(int id)
        {
            try
            {
                var plan = await _subscriptionPlanRepository.GetByIdAsync(id);
                if (plan == null)
                {
                    throw new KeyNotFoundException($"Subscription plan with ID {id} not found");
                }

                plan.IsActive = !plan.IsActive;
                plan.UpdatedAt = DateTime.UtcNow;

                var updatedPlan = await _subscriptionPlanRepository.UpdateAsync(plan);
                var result = _mapper.Map<SubscriptionPlanDto>(updatedPlan);
                
                _logger.LogInformation("Successfully toggled status for plan: {PlanId}", id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling status for plan: {PlanId}", id);
                throw;
            }
        }

        private decimal CalculateAutomaticPrice(decimal basePrice, int durationMonths)
        {
            try
            {
                // Automatic pricing logic based on duration
                decimal discountPercentage = 0;

                // Progressive discount for longer durations
                switch (durationMonths)
                {
                    case 1:
                        discountPercentage = 0; // No discount for 1 month
                        break;
                    case 3:
                        discountPercentage = 10; // 10% discount for 3 months
                        break;
                    case 6:
                        discountPercentage = 20; // 20% discount for 6 months
                        break;
                    case 12:
                        discountPercentage = 33; // 33% discount for 12 months (1 year)
                        break;
                    default:
                        // For custom durations, calculate discount progressively
                        if (durationMonths >= 24)
                            discountPercentage = 40; // 40% discount for 2+ years
                        else if (durationMonths >= 18)
                            discountPercentage = 30; // 30% discount for 18 months
                        else if (durationMonths >= 12)
                            discountPercentage = 25; // 25% discount for 12-17 months
                        else if (durationMonths >= 6)
                            discountPercentage = 15; // 15% discount for 6-11 months
                        else if (durationMonths >= 3)
                            discountPercentage = 5; // 5% discount for 3-5 months
                        break;
                }

                // Calculate final price
                var discountAmount = basePrice * (discountPercentage / 100);
                var finalPrice = basePrice * durationMonths - discountAmount;

                _logger.LogInformation("Calculated automatic price: BasePrice={BasePrice}, Duration={Duration} months, Discount={Discount}%, FinalPrice={FinalPrice}", 
                    basePrice, durationMonths, discountPercentage, finalPrice);

                return finalPrice;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating automatic price for base price {BasePrice} and duration {Duration}", basePrice, durationMonths);
                throw;
            }
        }
    }
}
