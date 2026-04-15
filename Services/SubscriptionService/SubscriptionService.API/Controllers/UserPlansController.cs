using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Interfaces;
using Common.Services;
using System.Globalization;

namespace SubscriptionService.API.Controllers
{
    /// <summary>
    /// User Controller for Subscription Plans
    /// Handles user-facing subscription plan operations
    /// </summary>
    [Route("api/user/plans")]
    [ApiController]
    [Authorize]
    public class UserPlansController : ControllerBase
    {
        private readonly ISubscriptionPlanService _subscriptionPlanService;
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly ILogger<UserPlansController> _logger;
        private readonly ILanguageService _languageService;

        public UserPlansController(
            ISubscriptionPlanService subscriptionPlanService,
            IUserSubscriptionService userSubscriptionService,
            ILogger<UserPlansController> logger,
            ILanguageService languageService)
        {
            _subscriptionPlanService = subscriptionPlanService;
            _userSubscriptionService = userSubscriptionService;
            _logger = logger;
            _languageService = languageService;
        }

        /// <summary>
        /// Get subscription plans by exam category
        /// </summary>
        /// <param name="examCategory">Exam category filter</param>
        /// <returns>List of plans for the exam category</returns>
        [HttpGet("by-exam/{examCategory}")]
        public async Task<ActionResult<IEnumerable<SubscriptionPlanListDto>>> GetPlansByExamCategory(string examCategory, [FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var result = await _subscriptionPlanService.GetPlansByExamCategoryAsync(examCategory, currentLanguage);
                return Ok(new
                {
                    success = true,
                    data = result,
                    language = currentLanguage,
                    message = "Subscription plans fetched successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving plans for exam category: {ExamCategory}", examCategory);
                return StatusCode(500, new { success = false, message = "Error fetching subscription plans" });
            }
        }

        /// <summary>
        /// Get all active subscription plans
        /// </summary>
        /// <returns>List of active plans</returns>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<SubscriptionPlanListDto>>> GetActivePlans([FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var result = await _subscriptionPlanService.GetActivePlansAsync(currentLanguage);
                return Ok(new
                {
                    success = true,
                    data = result,
                    language = currentLanguage,
                    message = "Active subscription plans fetched successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active subscription plans");
                return StatusCode(500, new { success = false, message = "Error fetching active subscription plans" });
            }
        }

        /// <summary>
        /// Get active plans excluding plans already purchased by current user.
        /// User can still purchase different plans and hold multiple subscriptions.
        /// </summary>
        [HttpGet("active/available")]
        public async Task<ActionResult<IEnumerable<SubscriptionPlanListDto>>> GetAvailableActivePlans([FromQuery] string? language = null)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized("Invalid user token");

                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var activePlans = await _subscriptionPlanService.GetActivePlansAsync(currentLanguage);
                var history = await _userSubscriptionService.GetUserSubscriptionHistoryAsync(userId);

                var purchasedActivePlanIds = new HashSet<int>();
                var now = DateTime.UtcNow;

                foreach (var subscription in history.Subscriptions)
                {
                    var isActive = string.Equals(subscription.Status, "Active", StringComparison.OrdinalIgnoreCase);
                    if (!isActive)
                    {
                        continue;
                    }

                    if (subscription.ValidTill >= now)
                    {
                        purchasedActivePlanIds.Add(subscription.SubscriptionPlanId);
                    }
                }

                var filteredPlans = new List<SubscriptionPlanListDto>();
                foreach (var plan in activePlans)
                {
                    if (!purchasedActivePlanIds.Contains(plan.Id))
                    {
                        filteredPlans.Add(plan);
                    }
                }

                return Ok(new
                {
                    success = true,
                    data = filteredPlans,
                    language = currentLanguage,
                    message = "Available active subscription plans fetched successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available active subscription plans for user");
                return StatusCode(500, new { success = false, message = "Error fetching available active subscription plans" });
            }
        }

        /// <summary>
        /// Get subscription plan by ID
        /// </summary>
        /// <param name="id">Plan ID</param>
        /// <returns>Plan details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SubscriptionPlanDto>> GetPlanById(int id, [FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var result = await _subscriptionPlanService.GetPlanByIdAsync(id, currentLanguage);
                if (result == null)
                    return NotFound(new { success = false, message = "Subscription plan not found" });

                return Ok(new
                {
                    success = true,
                    data = result,
                    language = currentLanguage,
                    message = "Subscription plan fetched successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plan: {PlanId}", id);
                return StatusCode(500, new { success = false, message = "Error fetching subscription plan" });
            }
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst("UserId") ?? User.FindFirst("sub");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int userId))
            {
                return userId;
            }

            return 0;
        }
    }
}
