using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Interfaces;

namespace SubscriptionService.API.Controllers
{
    /// <summary>
    /// User Subscription Controller
    /// </summary>
    [Route("api/user/[controller]")]
    [ApiController]
    [Authorize]
    public class SubscriptionsController : ControllerBase
    {
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly ILogger<SubscriptionsController> _logger;

        public SubscriptionsController(
            IUserSubscriptionService userSubscriptionService,
            ILogger<SubscriptionsController> logger)
        {
            _userSubscriptionService = userSubscriptionService;
            _logger = logger;
        }

        /// <summary>
        /// Get current user's active subscription
        /// </summary>
        /// <returns>User's subscription details</returns>
        [HttpGet("my-subscription")]
        public async Task<ActionResult<UserSubscriptionDto>> GetMySubscription()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized("Invalid user token");

                var result = await _userSubscriptionService.GetMySubscriptionAsync(userId);
                if (result == null)
                    return NotFound("No active subscription found");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user subscription");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get subscription history for the current user
        /// </summary>
        /// <returns>User subscription history</returns>
        [HttpGet("history")]
        public async Task<ActionResult<SubscriptionHistoryDto>> GetSubscriptionHistory()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized("Invalid user token");

                var result = await _userSubscriptionService.GetUserSubscriptionHistoryAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription history for user");
                return StatusCode(500, "Internal server error");
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
                    return Unauthorized("Invalid user token");

                createSubscriptionDto.UserId = userId;
                var result = await _userSubscriptionService.CreateSubscriptionAsync(createSubscriptionDto);
                return CreatedAtAction(nameof(GetMySubscription), new { }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user subscription");
                return StatusCode(500, "Internal server error");
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
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating subscription");
                return StatusCode(500, "Internal server error");
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
                    return Unauthorized("Invalid user token");

                // Verify the subscription belongs to the current user
                var subscription = await _userSubscriptionService.GetSubscriptionByIdAsync(cancelSubscriptionDto.SubscriptionId);
                if (subscription == null || subscription.UserId != userId)
                    return NotFound("Subscription not found or access denied");

                var result = await _userSubscriptionService.CancelSubscriptionAsync(cancelSubscriptionDto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Subscription not found for cancellation: {SubscriptionId}", cancelSubscriptionDto.SubscriptionId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling subscription: {SubscriptionId}", cancelSubscriptionDto.SubscriptionId);
                return StatusCode(500, "Internal server error");
            }
        }

        private int GetUserIdFromToken()
        {
            // This is a simplified version. In production, you should properly validate the JWT token
            // and extract the user ID from claims
            var userIdClaim = User.FindFirst("UserId") ?? User.FindFirst("sub");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }
    }
}
