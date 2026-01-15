using Microsoft.Extensions.Logging;
using SubscriptionService.Domain.Entities;
using SubscriptionService.Domain.Interfaces;

namespace SubscriptionService.Domain.Services
{
    public class SubscriptionValidationService : ISubscriptionValidationService
    {
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly IDemoAccessLogRepository _demoAccessLogRepository;
        private readonly ILogger<SubscriptionValidationService> _logger;

        public SubscriptionValidationService(
            IUserSubscriptionRepository userSubscriptionRepository,
            IDemoAccessLogRepository demoAccessLogRepository,
            ILogger<SubscriptionValidationService> logger)
        {
            _userSubscriptionRepository = userSubscriptionRepository;
            _demoAccessLogRepository = demoAccessLogRepository;
            _logger = logger;
        }

        public async Task<SubscriptionValidationResult> ValidateSubscriptionAsync(int userId, string examCategory = null)
        {
            try
            {
                var result = new SubscriptionValidationResult();

                // Get active subscription for user
                var subscription = await _userSubscriptionRepository.GetByUserIdAsync(userId);
                
                if (subscription == null)
                {
                    result.IsValid = false;
                    result.HasActiveSubscription = false;
                    result.Message = "No active subscription found";
                    result.RequiresRenewal = true;
                    result.RenewalUrl = "/api/user/subscriptions/plans";
                    return result;
                }

                result.HasActiveSubscription = true;
                result.PlanName = subscription.SubscriptionPlan.Name;
                result.ExamCategory = subscription.SubscriptionPlan.ExamCategory;
                result.Features = subscription.SubscriptionPlan.Features;
                result.ExpiryDate = subscription.EndDate;

                // Check if subscription is expired
                var now = DateTime.UtcNow;
                if (subscription.EndDate <= now)
                {
                    result.IsExpired = true;
                    result.IsValid = false;
                    result.Status = SubscriptionStatus.Expired;
                    result.Message = "Subscription has expired";
                    result.RequiresRenewal = true;
                    result.RenewalUrl = "/api/user/subscriptions/plans";
                }
                else if (subscription.Status == SubscriptionStatus.Cancelled)
                {
                    result.IsCancelled = true;
                    result.IsValid = false;
                    result.Status = SubscriptionStatus.Cancelled;
                    result.Message = "Subscription has been cancelled";
                    result.RequiresRenewal = true;
                    result.RenewalUrl = "/api/user/subscriptions/plans";
                }
                else if (subscription.Status == SubscriptionStatus.Active)
                {
                    result.IsValid = true;
                    result.Status = SubscriptionStatus.Active;
                    result.Message = "Subscription is active";
                    result.DaysUntilExpiry = (int)(subscription.EndDate - now).TotalDays;

                    // Check if exam category matches (if specified)
                    if (!string.IsNullOrEmpty(examCategory) && 
                        !string.IsNullOrEmpty(subscription.SubscriptionPlan.ExamCategory) &&
                        subscription.SubscriptionPlan.ExamCategory != examCategory)
                    {
                        result.IsValid = false;
                        result.Message = $"Subscription is for {subscription.SubscriptionPlan.ExamCategory}, not {examCategory}";
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating subscription for user: {UserId}", userId);
                return new SubscriptionValidationResult
                {
                    IsValid = false,
                    Message = "Error validating subscription"
                };
            }
        }

        public async Task<DemoAccessResult> CheckDemoEligibilityAsync(int userId, string examCategory)
        {
            try
            {
                var result = new DemoAccessResult
                {
                    MaxDemoQuestions = 10 // Configurable limit
                };

                // Check if user already has an active subscription
                var hasActiveSubscription = await IsSubscriptionActiveAsync(userId);
                if (hasActiveSubscription)
                {
                    result.IsEligible = false;
                    result.CanProceed = true; // They can proceed with full access
                    result.Message = "User has active subscription, no demo needed";
                    return result;
                }

                // Check if user has already used demo for this exam category
                var lastDemoAccess = await _demoAccessLogRepository.GetLastDemoAccessAsync(userId, examCategory);
                
                if (lastDemoAccess != null)
                {
                    result.HasUsedDemo = true;
                    result.LastAccessDate = lastDemoAccess.AccessDate;
                    result.QuestionsAttempted = lastDemoAccess.QuestionsAttempted;
                    
                    // Calculate remaining questions
                    result.RemainingQuestions = Math.Max(0, result.MaxDemoQuestions - result.QuestionsAttempted);
                    
                    if (result.RemainingQuestions <= 0)
                    {
                        result.IsEligible = false;
                        result.CanProceed = false;
                        result.Message = "Demo access limit exceeded for this exam category";
                    }
                    else
                    {
                        result.IsEligible = true;
                        result.CanProceed = true;
                        result.Message = $"Demo access available. {result.RemainingQuestions} questions remaining";
                    }
                }
                else
                {
                    result.IsEligible = true;
                    result.CanProceed = true;
                    result.HasUsedDemo = false;
                    result.QuestionsAttempted = 0;
                    result.RemainingQuestions = result.MaxDemoQuestions;
                    result.Message = "Demo access available";
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking demo eligibility for user: {UserId}, exam: {ExamCategory}", userId, examCategory);
                return new DemoAccessResult
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
                var subscription = await _userSubscriptionRepository.GetByUserIdAsync(userId);
                return subscription != null && 
                       subscription.Status == SubscriptionStatus.Active && 
                       subscription.EndDate > DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking active subscription for user: {UserId}", userId);
                return false;
            }
        }

        public async Task<UserSubscription?> GetActiveSubscriptionAsync(int userId)
        {
            try
            {
                return await _userSubscriptionRepository.GetByUserIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active subscription for user: {UserId}", userId);
                return null;
            }
        }
    }
}
