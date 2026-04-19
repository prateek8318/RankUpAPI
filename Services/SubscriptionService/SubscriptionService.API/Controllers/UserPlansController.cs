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
        /// Get all active subscription plans for user's selected exam
        /// </summary>
        /// <param name="language">Optional language code</param>
        /// <returns>List of active plans filtered by user's selected exam and purchased plans</returns>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<SubscriptionPlanListDto>>> GetActivePlans([FromQuery] string? language = null)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized(new { success = false, message = "Invalid user token" });

                // Get user's selected exam from user profile/service
                var userSelectedExam = await GetUserSelectedExamAsync(userId);
                if (userSelectedExam == null)
                    return BadRequest(new { success = false, message = "Please select an exam first to view subscription plans" });

                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var result = await _subscriptionPlanService.GetActivePlansAsync(currentLanguage, userSelectedExam.ExamId, userId);
                return Ok(new
                {
                    success = true,
                    data = result,
                    language = currentLanguage,
                    selectedExam = new { id = userSelectedExam.ExamId, name = userSelectedExam.ExamName },
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

        /// <summary>
        /// Get subscription plan with duration options by ID
        /// </summary>
        /// <param name="id">Plan ID</param>
        /// <param name="language">Optional language code</param>
        /// <returns>Plan details with duration options (only if user hasn't purchased this plan)</returns>
        [HttpGet("{id}/with-durations")]
        public async Task<ActionResult<PlanWithDurationOptionsDto>> GetPlanWithDurations(int id, [FromQuery] string? language = null)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized(new { success = false, message = "Invalid user token" });

                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var result = await _subscriptionPlanService.GetPlanWithDurationsAsync(id, currentLanguage, userId);
                if (result == null)
                    return NotFound(new { success = false, message = "Subscription plan not found or already purchased" });

                return Ok(new
                {
                    success = true,
                    data = result,
                    language = currentLanguage,
                    message = "Subscription plan with duration options fetched successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plan with durations: {PlanId}", id);
                return StatusCode(500, new { success = false, message = "Error fetching subscription plan" });
            }
        }

        /// <summary>
        /// Get all active plans with duration options for user's selected exam
        /// </summary>
        /// <param name="language">Optional language code</param>
        /// <returns>List of plans with duration options filtered by user's selected exam and purchased plans</returns>
        [HttpGet("with-durations")]
        public async Task<ActionResult<IEnumerable<PlanWithDurationOptionsDto>>> GetActivePlansWithDurations([FromQuery] string? language = null)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized(new { success = false, message = "Invalid user token" });

                // Get user's selected exam from user profile/service
                var userSelectedExam = await GetUserSelectedExamAsync(userId);
                if (userSelectedExam == null)
                    return BadRequest(new { success = false, message = "Please select an exam first to view subscription plans" });

                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var result = await _subscriptionPlanService.GetActivePlansWithDurationsAsync(currentLanguage, userSelectedExam.ExamId, userId);
                return Ok(new
                {
                    success = true,
                    data = result,
                    language = currentLanguage,
                    selectedExam = new { id = userSelectedExam.ExamId, name = userSelectedExam.ExamName },
                    message = "Active subscription plans with duration options fetched successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active subscription plans with durations");
                return StatusCode(500, new { success = false, message = "Error fetching subscription plans" });
            }
        }

        /// <summary>
        /// Purchase a subscription plan with selected duration
        /// </summary>
        /// <param name="purchaseRequest">Purchase request with plan and duration selection</param>
        /// <returns>Purchase response with payment details</returns>
        [HttpPost("purchase")]
        public async Task<ActionResult<PurchasePlanResponseDto>> PurchasePlan([FromBody] PurchasePlanRequestDto purchaseRequest)
        {
            if (purchaseRequest == null)
                return BadRequest(new { success = false, message = "Purchase request is required" });

            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized(new { success = false, message = "Invalid user token" });

                var result = await _subscriptionPlanService.PurchasePlanAsync(userId, purchaseRequest);
                return Ok(new
                {
                    success = result.Success,
                    data = result,
                    message = result.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Plan or duration option not found");
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid purchase operation");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing plan purchase");
                var message = HttpContext.RequestServices.GetService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>()?.EnvironmentName == "Development"
                    ? ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "")
                    : "Internal server error";
                return StatusCode(500, new { success = false, message });
            }
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst("UserId") ?? User.FindFirst("sub") ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int userId))
            {
                return userId;
            }

            return 0;
        }

        private async Task<UserSelectedExamDto?> GetUserSelectedExamAsync(int userId)
        {
            try
            {
                // This would typically call User Service to get user's selected exam
                // For now, implementing a mock that checks user's profile
                // In real implementation, this would call UserService API
                
                // Mock implementation - replace with actual User Service call
                var userExam = await GetUserSelectedExamFromUserServiceAsync(userId);
                
                if (userExam == null)
                {
                    _logger.LogWarning("User {UserId} has not selected any exam", userId);
                    return null;
                }

                return userExam;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user's selected exam for user {UserId}", userId);
                return null;
            }
        }

        private async Task<UserSelectedExamDto?> GetUserSelectedExamFromUserServiceAsync(int userId)
        {
            // TODO: Replace with actual User Service API call
            // This is a mock implementation
            
            // In real implementation, you would call something like:
            // var response = await _httpClient.GetAsync($"/api/user/profile/exam-selection/{userId}");
            // var userExam = await response.Content.ReadFromJsonAsync<UserSelectedExamDto>();
            
            // For now, return null to indicate user needs to select exam first
            return null;
        }
    }

    // DTO for user's selected exam
    public class UserSelectedExamDto
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public string? ExamCategory { get; set; }
        public DateTime? SelectedAt { get; set; }
    }
}
