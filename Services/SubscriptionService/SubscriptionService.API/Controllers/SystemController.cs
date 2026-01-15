using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Interfaces;

namespace SubscriptionService.API.Controllers
{
    /// <summary>
    /// System Controller for Subscription Validation
    /// Used by other microservices to validate user subscriptions
    /// </summary>
    [Route("api/system/[controller]")]
    [ApiController]
    public class SubscriptionValidationController : ControllerBase
    {
        private readonly ISubscriptionValidationAppService _subscriptionValidationService;
        private readonly ILogger<SubscriptionValidationController> _logger;

        public SubscriptionValidationController(
            ISubscriptionValidationAppService subscriptionValidationService,
            ILogger<SubscriptionValidationController> logger)
        {
            _subscriptionValidationService = subscriptionValidationService;
            _logger = logger;
        }

        /// <summary>
        /// Validate subscription for a user (used by other microservices)
        /// </summary>
        /// <param name="request">Validation request details</param>
        /// <returns>Subscription validation result</returns>
        [HttpPost("validate")]
        public async Task<ActionResult<SubscriptionValidationResponseDto>> ValidateSubscription([FromBody] SubscriptionValidationRequestDto request)
        {
            try
            {
                var result = await _subscriptionValidationService.ValidateSubscriptionAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating subscription for user: {UserId}", request.UserId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Check if user has active subscription (simplified validation)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Active subscription status</returns>
        [HttpGet("check-active/{userId}")]
        public async Task<ActionResult<bool>> CheckActiveSubscription(int userId)
        {
            try
            {
                var result = await _subscriptionValidationService.IsSubscriptionActiveAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking active subscription for user: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Validate subscription for a specific service (used by other microservices)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="examCategory">Exam category (optional)</param>
        /// <returns>Validation result</returns>
        [HttpGet("validate-for-service")]
        public async Task<ActionResult<bool>> ValidateSubscriptionForService([FromQuery] int userId, [FromQuery] string? examCategory = null)
        {
            try
            {
                var result = await _subscriptionValidationService.ValidateSubscriptionForServiceAsync(userId, examCategory);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating subscription for service - User: {UserId}, Exam: {ExamCategory}", userId, examCategory);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Check demo eligibility for a user
        /// </summary>
        /// <param name="request">Demo eligibility request</param>
        /// <returns>Demo eligibility result</returns>
        [HttpPost("check-demo-eligibility")]
        public async Task<ActionResult<DemoEligibilityResponseDto>> CheckDemoEligibility([FromBody] DemoEligibilityRequestDto request)
        {
            try
            {
                var result = await _subscriptionValidationService.CheckDemoEligibilityAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking demo eligibility for user: {UserId}", request.UserId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Log demo access (used by other microservices)
        /// </summary>
        /// <param name="logDemoAccessDto">Demo access log details</param>
        /// <returns>Success status</returns>
        [HttpPost("log-demo-access")]
        public async Task<ActionResult<bool>> LogDemoAccess([FromBody] LogDemoAccessDto logDemoAccessDto)
        {
            try
            {
                var result = await _subscriptionValidationService.LogDemoAccessAsync(logDemoAccessDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging demo access for user: {UserId}", logDemoAccessDto.UserId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
