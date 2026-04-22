using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Interfaces;

namespace SubscriptionService.API.Controllers
{
    /// <summary>
    /// User Subscription Management Controller for Admins
    /// </summary>
    [Route("api/admin/user-subscriptions")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminUserSubscriptionsController : ControllerBase
    {
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly ILogger<AdminUserSubscriptionsController> _logger;

        public AdminUserSubscriptionsController(
            IUserSubscriptionService userSubscriptionService,
            ILogger<AdminUserSubscriptionsController> logger)
        {
            _userSubscriptionService = userSubscriptionService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new user subscription (admin override)
        /// </summary>
        /// <param name="createUserSubscriptionDto">User subscription creation details</param>
        /// <returns>Created user subscription details</returns>
        [HttpPost]
        public async Task<ActionResult<UserSubscriptionDto>> CreateUserSubscription([FromBody] CreateUserSubscriptionDto createUserSubscriptionDto)
        {
            try
            {
                var result = await _userSubscriptionService.CreateSubscriptionAsync(createUserSubscriptionDto);
                return Ok(new
                {
                    success = true,
                    data = result,
                    message = "User subscription created successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user subscription for user: {UserId}", createUserSubscriptionDto.UserId);
                return StatusCode(500, new { success = false, message = "Error creating user subscription" });
            }
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
        /// Get active subscription for a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User active subscription details</returns>
        [HttpGet("user/{userId}/active")]
        public async Task<ActionResult<UserSubscriptionDto>> GetActiveSubscriptionForUser(int userId)
        {
            try
            {
                var result = await _userSubscriptionService.GetActiveSubscriptionForUserAsync(userId);
                if (result == null)
                    return NotFound(new { Message = "No active subscription found for this user" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active subscription for user: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update a user subscription
        /// </summary>
        /// <param name="id">Subscription ID</param>
        /// <param name="updateUserSubscriptionDto">Update details</param>
        /// <returns>Updated subscription details</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<UserSubscriptionDto>> UpdateUserSubscription(int id)
        {
            try
            {
                // For now, just get the active subscription by user ID
                var result = await _userSubscriptionService.GetMySubscriptionAsync(id);
                if (result == null)
                    return NotFound(new { success = false, message = "User subscription not found" });
                
                return Ok(new
                {
                    success = true,
                    data = result,
                    message = "User subscription retrieved successfully"
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User subscription not found: {SubscriptionId}", id);
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user subscription: {SubscriptionId}", id);
                return StatusCode(500, new { success = false, message = "Error updating user subscription" });
            }
        }

        /// <summary>
        /// Cancel a user subscription by ID
        /// </summary>
        /// <param name="id">Subscription ID</param>
        /// <returns>Cancelled subscription details</returns>
        [HttpPut("{id}/cancel")]
        public async Task<ActionResult<UserSubscriptionDto>> CancelSubscription(int id)
        {
            try
            {
                var cancelDto = new CancelSubscriptionDto { SubscriptionId = id };
                var result = await _userSubscriptionService.CancelSubscriptionAsync(cancelDto);
                return Ok(new
                {
                    success = true,
                    data = result,
                    message = "Subscription cancelled successfully"
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User subscription not found for cancellation: {SubscriptionId}", id);
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling user subscription: {SubscriptionId}", id);
                return StatusCode(500, new { success = false, message = "Error cancelling user subscription" });
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

        /// <summary>
        /// Get user management statistics
        /// </summary>
        /// <returns>User management stats including total users, active subscribers, free users, new users</returns>
        [HttpGet("user-management/stats")]
        public async Task<ActionResult<UserManagementStatsDto>> GetUserManagementStats()
        {
            try
            {
                var result = await _userSubscriptionService.GetUserManagementStatsAsync();
                return Ok(new
                {
                    success = true,
                    data = result,
                    message = "User management statistics retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user management statistics");
                return StatusCode(500, new { success = false, message = "Error retrieving user management statistics" });
            }
        }

        /// <summary>
        /// Get all users with their subscription details
        /// </summary>
        /// <returns>List of users with subscription information</returns>
        [HttpGet("user-management/users")]
        public async Task<ActionResult<IEnumerable<UserManagementDto>>> GetAllUsersWithSubscriptionDetails()
        {
            try
            {
                var result = await _userSubscriptionService.GetAllUsersWithSubscriptionDetailsAsync();
                return Ok(new
                {
                    success = true,
                    data = result,
                    message = "Users with subscription details retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users with subscription details");
                return StatusCode(500, new { success = false, message = "Error retrieving users with subscription details" });
            }
        }

        /// <summary>
        /// Block or unblock a user
        /// </summary>
        /// <param name="blockUserDto">User blocking details</param>
        /// <returns>Blocking operation result</returns>
        [HttpPost("user-management/block")]
        public async Task<ActionResult<BlockUserResponseDto>> BlockUser([FromBody] BlockUserDto blockUserDto)
        {
            try
            {
                if (blockUserDto.UserId <= 0)
                {
                    return BadRequest(new { success = false, message = "Invalid user ID" });
                }

                var result = await _userSubscriptionService.BlockUserAsync(blockUserDto);
                
                if (result.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        data = result,
                        message = result.Message
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        data = result,
                        message = result.Message
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error blocking/unblocking user {UserId}", blockUserDto.UserId);
                return StatusCode(500, new { success = false, message = "Error blocking/unblocking user" });
            }
        }
    }
}
