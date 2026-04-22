using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Interfaces;
using Common.Services;
using Microsoft.Extensions.Configuration;

namespace SubscriptionService.API.Controllers
{
    /// <summary>
    /// User Subscription Plans Controller (Authentication Required)
    /// </summary>
    [Route("api/v1/user/plans")]
    [ApiController]
    [Authorize]
    public class UserPlansController : ControllerBase
    {
        private readonly ISubscriptionPlanService _subscriptionPlanService;
        private readonly ILogger<UserPlansController> _logger;
        private readonly ILanguageService _languageService;
        private readonly IConfiguration _configuration;

        public UserPlansController(
            ISubscriptionPlanService subscriptionPlanService,
            ILogger<UserPlansController> logger,
            ILanguageService languageService,
            IConfiguration configuration)
        {
            _subscriptionPlanService = subscriptionPlanService;
            _logger = logger;
            _languageService = languageService;
            _configuration = configuration;
        }

        /// <summary>
        /// Get all active subscription plans filtered by current user's selected exams
        /// </summary>
        /// <param name="language">Optional: language code (e.g., en, hi)</param>
        /// <param name="page">Page number for pagination</param>
        /// <param name="pageSize">Page size for pagination</param>
        /// <returns>List of active subscription plans filtered by user's exam preferences</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubscriptionPlanListDto>>> GetActivePlans([FromQuery] string? language = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                
                // Get current user's ID from token
                var currentUserId = GetCurrentUserId();
                if (currentUserId == 0)
                    return Unauthorized(new { success = false, message = "Invalid user token" });

                // Get user's exam preferences and filter plans
                var userExamIds = await GetUserExamIdsAsync(currentUserId);
                if (userExamIds.Any())
                {
                    var filteredResult = await _subscriptionPlanService.GetActivePlansByExamIdsAsync(currentLanguage, userExamIds);
                    return Ok(new
                    {
                        success = true,
                        data = filteredResult,
                        language = currentLanguage,
                        userId = currentUserId,
                        examFilters = userExamIds,
                        message = "Active subscription plans filtered by your exam preferences fetched successfully"
                    });
                }
                
                // Fallback to all active plans if no exam preferences
                var result = await _subscriptionPlanService.GetActivePlansAsync(currentLanguage);
                return Ok(new
                {
                    success = true,
                    data = result,
                    language = currentLanguage,
                    userId = currentUserId,
                    message = "All active subscription plans fetched successfully (no exam preferences set)"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active subscription plans");
                return StatusCode(500, new { success = false, message = "Error fetching active subscription plans" });
            }
        }

        /// <summary>
        /// Get plans by exam category (Public Access)
        /// </summary>
        /// <param name="examCategory">Exam category filter</param>
        /// <param name="language">Optional: language code</param>
        /// <param name="page">Page number for pagination</param>
        /// <param name="pageSize">Page size for pagination</param>
        /// <returns>List of plans for the exam category</returns>
        [HttpGet("by-exam/{examCategory}")]
        public async Task<ActionResult<IEnumerable<SubscriptionPlanListDto>>> GetPlansByExamCategory(string examCategory, [FromQuery] string? language = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
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
        /// Get plan by ID with duration options
        /// </summary>
        /// <param name="id">Plan ID</param>
        /// <param name="language">Optional: language code</param>
        /// <returns>Plan details with duration options</returns>
        [HttpGet("durations/{id}")]
        public async Task<ActionResult<PlanWithDurationOptionsDto>> GetPlanWithDurations(int id, [FromQuery] string? language = null)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == 0)
                    return Unauthorized(new { success = false, message = "Invalid user token" });

                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var result = await _subscriptionPlanService.GetPlanWithDurationsAsync(id, currentLanguage, currentUserId);
                if (result == null)
                    return NotFound(new { success = false, message = "Subscription plan not found" });

                return Ok(new
                {
                    success = true,
                    data = result,
                    language = currentLanguage,
                    userId = currentUserId,
                    message = "Subscription plan with duration options fetched successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plan: {PlanId}", id);
                return StatusCode(500, new { success = false, message = "Error fetching subscription plan" });
            }
        }

        /// <summary>
        /// Get current user ID from JWT token
        /// </summary>
        /// <returns>User ID</returns>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                             ?? User.FindFirst("sub")
                             ?? User.FindFirst("UserId");

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }

        /// <summary>
        /// Get user's selected exam IDs from JWT token claims
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of exam IDs selected by the user</returns>
        private async Task<List<int>> GetUserExamIdsAsync(int userId)
        {
            try
            {
                var examIds = new List<int>();
                
                // Extract only ExamId from JWT token claims
                var examIdClaim = User.FindFirst("ExamId");
                
                // Add ExamId if present in token
                if (examIdClaim != null && int.TryParse(examIdClaim.Value, out int examId))
                    examIds.Add(examId);
                
                // If no ExamId in token, return empty list (will show no plans)
                if (!examIds.Any())
                {
                    _logger.LogInformation("No ExamId found in token for userId: {UserId}", userId);
                }
                
                return examIds.Distinct().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting ExamId from token for userId: {UserId}", userId);
                return new List<int>();
            }
        }
    }
}
