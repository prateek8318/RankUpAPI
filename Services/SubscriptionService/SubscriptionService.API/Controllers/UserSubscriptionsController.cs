using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Interfaces;

namespace SubscriptionService.API.Controllers
{
    /// <summary>
    /// User Subscription Management Controller
    /// </summary>
    [Route("api/user/subscriptions")]
    [ApiController]
    [Authorize]
    public class UserSubscriptionsController : ControllerBase
    {
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly ILogger<UserSubscriptionsController> _logger;

        public UserSubscriptionsController(
            IUserSubscriptionService userSubscriptionService,
            ILogger<UserSubscriptionsController> logger)
        {
            _userSubscriptionService = userSubscriptionService;
            _logger = logger;
        }

        /// <summary>
        /// Get current user's active subscription
        /// </summary>
        /// <returns>User's subscription details</returns>
        [HttpGet("current")]
        public async Task<ActionResult<UserSubscriptionDto>> GetCurrentSubscription()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized(new { success = false, message = "Invalid user token" });

                var result = await _userSubscriptionService.GetMySubscriptionAsync(userId);
                if (result == null)
                    return NotFound(new { success = false, message = "No active subscription found" });

                return Ok(new { success = true, data = result, message = "Current subscription retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user subscription");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get subscription history for the current user
        /// </summary>
        /// <param name="page">Page number for pagination</param>
        /// <param name="pageSize">Page size for pagination</param>
        /// <returns>User subscription history</returns>
        [HttpGet]
        public async Task<ActionResult<SubscriptionHistoryDto>> GetSubscriptionHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized(new { success = false, message = "Invalid user token" });

                var result = await _userSubscriptionService.GetUserSubscriptionHistoryAsync(userId);
                return Ok(new { success = true, data = result, message = "Subscription history retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription history for user");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Create a new subscription (after payment verification)
        /// </summary>
        /// <param name="createSubscriptionDto">Subscription creation details</param>
        /// <returns>Created subscription details</returns>
        [HttpPost]
        public async Task<ActionResult<UserSubscriptionDto>> CreateSubscription([FromBody] CreateUserSubscriptionDto createSubscriptionDto)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized(new { success = false, message = "Invalid user token" });

                createSubscriptionDto.UserId = userId;
                var result = await _userSubscriptionService.CreateSubscriptionAsync(createSubscriptionDto);
                return CreatedAtAction(nameof(GetCurrentSubscription), new { }, new { success = true, data = result, message = "Subscription created successfully" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid subscription create request for user");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Subscription plan not found while creating user subscription");
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user subscription");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Activate subscription after payment verification
        /// </summary>
        /// <param name="activateSubscriptionDto">Activation details</param>
        /// <returns>Activated subscription details</returns>
        [HttpPost("activate")]
        public async Task<ActionResult<PaymentVerificationResultDto>> ActivateSubscription([FromBody] ActivateSubscriptionDto activateSubscriptionDto)
        {
            try
            {
                var result = await _userSubscriptionService.ActivateSubscriptionAsync(activateSubscriptionDto);
                return Ok(new { success = true, data = result, message = "Subscription activated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating subscription");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Cancel user subscription
        /// </summary>
        /// <param name="cancelSubscriptionDto">Cancellation details</param>
        /// <returns>Success status</returns>
        [HttpPost("cancel")]
        public async Task<ActionResult<bool>> CancelSubscription([FromBody] CancelSubscriptionDto cancelSubscriptionDto)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized(new { success = false, message = "Invalid user token" });

                // Verify the subscription belongs to the current user
                var subscription = await _userSubscriptionService.GetSubscriptionByIdAsync(cancelSubscriptionDto.SubscriptionId);
                if (subscription == null || subscription.UserId != userId)
                    return NotFound(new { success = false, message = "Subscription not found or access denied" });

                var result = await _userSubscriptionService.CancelSubscriptionAsync(cancelSubscriptionDto);
                return Ok(new { success = true, data = result, message = "Subscription cancelled successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Subscription not found for cancellation: {SubscriptionId}", cancelSubscriptionDto.SubscriptionId);
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling subscription: {SubscriptionId}", cancelSubscriptionDto.SubscriptionId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get all active subscription plans for users
        /// </summary>
        /// <param name="language">Optional language code (en, hi, etc.)</param>
        /// <returns>List of active subscription plans</returns>
        [HttpGet("plans/available")]
        public async Task<ActionResult<IEnumerable<SubscriptionPlanListDto>>> GetAvailablePlans([FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? "en";
                var result = await _userSubscriptionService.GetActivePlansAsync(currentLanguage);
                return Ok(new
                {
                    success = true,
                    data = result,
                    language = currentLanguage,
                    message = "Available subscription plans fetched successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active subscription plans");
                return StatusCode(500, new { success = false, message = "Error fetching available subscription plans" });
            }
        }

        private int GetUserIdFromToken()
        {
            // Extract user ID from JWT token claims
            // The token is generated with ClaimTypes.NameIdentifier in AuthController
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) 
                             ?? User.FindFirst("sub") 
                             ?? User.FindFirst("UserId");
            
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }
    }
}
