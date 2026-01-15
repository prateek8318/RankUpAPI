using AutoMapper;
using Microsoft.Extensions.Logging;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Interfaces;
using SubscriptionService.Domain.Entities;
using SubscriptionService.Domain.Interfaces;

namespace SubscriptionService.Application.Services
{
    public class SubscriptionValidationAppService : ISubscriptionValidationAppService
    {
        private readonly ISubscriptionValidationService _subscriptionValidationService;
        private readonly IDemoAccessLogRepository _demoAccessLogRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SubscriptionValidationAppService> _logger;

        public SubscriptionValidationAppService(
            ISubscriptionValidationService subscriptionValidationService,
            IDemoAccessLogRepository demoAccessLogRepository,
            IMapper mapper,
            ILogger<SubscriptionValidationAppService> logger)
        {
            _subscriptionValidationService = subscriptionValidationService;
            _demoAccessLogRepository = demoAccessLogRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<SubscriptionValidationResponseDto> ValidateSubscriptionAsync(SubscriptionValidationRequestDto request)
        {
            try
            {
                _logger.LogInformation("Validating subscription for user: {UserId}, exam: {ExamCategory}", 
                    request.UserId, request.ExamCategory);

                var validationResult = await _subscriptionValidationService.ValidateSubscriptionAsync(request.UserId, request.ExamCategory);

                var response = new SubscriptionValidationResponseDto
                {
                    IsValid = validationResult.IsValid,
                    HasActiveSubscription = validationResult.HasActiveSubscription,
                    IsExpired = validationResult.IsExpired,
                    IsCancelled = validationResult.IsCancelled,
                    ExpiryDate = validationResult.ExpiryDate,
                    PlanName = validationResult.PlanName,
                    ExamCategory = validationResult.ExamCategory,
                    Features = validationResult.Features,
                    Message = validationResult.Message,
                    DaysUntilExpiry = validationResult.DaysUntilExpiry,
                    RequiresRenewal = validationResult.RequiresRenewal,
                    RenewalUrl = validationResult.RenewalUrl
                };

                _logger.LogInformation("Subscription validation completed for user: {UserId}, valid: {IsValid}", 
                    request.UserId, response.IsValid);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating subscription for user: {UserId}", request.UserId);
                return new SubscriptionValidationResponseDto
                {
                    IsValid = false,
                    Message = "Error validating subscription"
                };
            }
        }

        public async Task<DemoEligibilityResponseDto> CheckDemoEligibilityAsync(DemoEligibilityRequestDto request)
        {
            try
            {
                _logger.LogInformation("Checking demo eligibility for user: {UserId}, exam: {ExamCategory}", 
                    request.UserId, request.ExamCategory);

                var eligibilityResult = await _subscriptionValidationService.CheckDemoEligibilityAsync(request.UserId, request.ExamCategory);

                var response = new DemoEligibilityResponseDto
                {
                    IsEligible = eligibilityResult.IsEligible,
                    HasUsedDemo = eligibilityResult.HasUsedDemo,
                    LastAccessDate = eligibilityResult.LastAccessDate,
                    QuestionsAttempted = eligibilityResult.QuestionsAttempted,
                    RemainingQuestions = eligibilityResult.RemainingQuestions,
                    MaxDemoQuestions = eligibilityResult.MaxDemoQuestions,
                    Message = eligibilityResult.Message,
                    CanProceed = eligibilityResult.CanProceed
                };

                _logger.LogInformation("Demo eligibility check completed for user: {UserId}, eligible: {IsEligible}", 
                    request.UserId, response.IsEligible);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking demo eligibility for user: {UserId}", request.UserId);
                return new DemoEligibilityResponseDto
                {
                    IsEligible = false,
                    CanProceed = false,
                    Message = "Error checking demo eligibility"
                };
            }
        }

        public async Task<bool> IsSubscriptionActiveAsync(int userId)
        {
            try
            {
                return await _subscriptionValidationService.IsSubscriptionActiveAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking active subscription for user: {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> LogDemoAccessAsync(LogDemoAccessDto logDemoAccessDto)
        {
            try
            {
                _logger.LogInformation("Logging demo access for user: {UserId}, exam: {ExamCategory}", 
                    logDemoAccessDto.UserId, logDemoAccessDto.ExamCategory);

                var demoAccessLog = new DemoAccessLog
                {
                    UserId = logDemoAccessDto.UserId,
                    ExamCategory = logDemoAccessDto.ExamCategory,
                    QuestionsAttempted = logDemoAccessDto.QuestionsAttempted,
                    TimeSpentMinutes = logDemoAccessDto.TimeSpentMinutes,
                    IPAddress = logDemoAccessDto.IPAddress,
                    UserAgent = logDemoAccessDto.UserAgent,
                    DeviceType = logDemoAccessDto.DeviceType,
                    IsCompleted = logDemoAccessDto.IsCompleted,
                    AccessDetails = logDemoAccessDto.AccessDetails
                };

                await _demoAccessLogRepository.AddAsync(demoAccessLog);
                await _demoAccessLogRepository.SaveChangesAsync();

                _logger.LogInformation("Successfully logged demo access for user: {UserId}", logDemoAccessDto.UserId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging demo access for user: {UserId}", logDemoAccessDto.UserId);
                return false;
            }
        }

        public async Task<bool> ValidateSubscriptionForServiceAsync(int userId, string examCategory = null)
        {
            try
            {
                var validationResult = await _subscriptionValidationService.ValidateSubscriptionAsync(userId, examCategory);
                return validationResult.IsValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating subscription for service - User: {UserId}, Exam: {ExamCategory}", 
                    userId, examCategory);
                return false;
            }
        }
    }
}
