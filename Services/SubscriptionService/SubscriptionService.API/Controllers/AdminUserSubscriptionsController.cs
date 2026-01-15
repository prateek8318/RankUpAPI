using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Interfaces;

namespace SubscriptionService.API.Controllers
{
    /// <summary>
    /// User Subscription Management Controller for Admins
    /// </summary>
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
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
        /// Get all user subscriptions
        /// </summary>
        /// <returns>List of all user subscriptions</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserSubscriptionDto>>> GetAllUserSubscriptions()
        {
            try
            {
                var result = await _userSubscriptionService.GetAllUserSubscriptionsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all user subscriptions");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get subscription history for a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User subscription history</returns>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<SubscriptionHistoryDto>> GetUserSubscriptionHistory(int userId)
        {
            try
            {
                var result = await _userSubscriptionService.GetUserSubscriptionHistoryAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription history for user: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Manually renew a user subscription
        /// </summary>
        /// <param name="renewSubscriptionDto">Renewal details</param>
        /// <returns>Renewed subscription details</returns>
        [HttpPost("renew")]
        public async Task<ActionResult<UserSubscriptionDto>> RenewUserSubscription([FromBody] RenewSubscriptionDto renewSubscriptionDto)
        {
            try
            {
                var result = await _userSubscriptionService.RenewSubscriptionAsync(renewSubscriptionDto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Subscription not found for renewal: {SubscriptionId}", renewSubscriptionDto.SubscriptionId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error renewing subscription: {SubscriptionId}", renewSubscriptionDto.SubscriptionId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Manually cancel a user subscription
        /// </summary>
        /// <param name="cancelSubscriptionDto">Cancellation details</param>
        /// <returns>Success status</returns>
        [HttpPost("cancel")]
        public async Task<ActionResult<bool>> CancelUserSubscription([FromBody] CancelSubscriptionDto cancelSubscriptionDto)
        {
            try
            {
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

        /// <summary>
        /// Get active subscriptions
        /// </summary>
        /// <returns>List of active subscriptions</returns>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<UserSubscriptionDto>>> GetActiveSubscriptions()
        {
            try
            {
                var result = await _userSubscriptionService.GetActiveSubscriptionsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active subscriptions");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get subscriptions expiring soon
        /// </summary>
        /// <param name="daysBeforeExpiry">Days before expiry to check (default: 7)</param>
        /// <returns>List of expiring subscriptions</returns>
        [HttpGet("expiring")]
        public async Task<ActionResult<IEnumerable<UserSubscriptionDto>>> GetExpiringSubscriptions([FromQuery] int daysBeforeExpiry = 7)
        {
            try
            {
                var result = await _userSubscriptionService.GetExpiringSubscriptionsAsync(daysBeforeExpiry);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving expiring subscriptions");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
