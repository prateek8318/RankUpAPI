using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdminService.Application.Interfaces;
using AdminService.Application.DTOs;

namespace AdminService.API.Controllers
{
    /// <summary>
    /// Admin Subscription Management Controller - FR-ADM-05
    /// Orchestrates calls to SubscriptionService
    /// </summary>
    [Route("api/admin/subscriptions")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminSubscriptionController : ControllerBase
    {
        private readonly ISubscriptionServiceClient _subscriptionServiceClient;
        private readonly ILogger<AdminSubscriptionController> _logger;

        public AdminSubscriptionController(
            ISubscriptionServiceClient subscriptionServiceClient,
            ILogger<AdminSubscriptionController> logger)
        {
            _subscriptionServiceClient = subscriptionServiceClient;
            _logger = logger;
        }

        [HttpGet("plans")]
        public async Task<ActionResult<ApiResponseDto<object>>> GetAllSubscriptionPlans()
        {
            try
            {
                var plans = await _subscriptionServiceClient.GetAllSubscriptionsAsync();
                return Ok(new ApiResponseDto<object> { Success = true, Data = plans });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription plans");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpGet("plans/{id}")]
        public async Task<ActionResult<ApiResponseDto<object>>> GetSubscriptionPlanById(int id)
        {
            try
            {
                var plan = await _subscriptionServiceClient.GetSubscriptionByIdAsync(id);
                if (plan == null)
                    return NotFound(new ApiResponseDto<object> { Success = false, ErrorMessage = "Subscription plan not found" });

                return Ok(new ApiResponseDto<object> { Success = true, Data = plan });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting subscription plan {id}");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpPost("plans")]
        public async Task<ActionResult<ApiResponseDto<object>>> CreateSubscriptionPlan([FromBody] object createDto)
        {
            try
            {
                var plan = await _subscriptionServiceClient.CreateSubscriptionPlanAsync(createDto);
                if (plan == null)
                    return BadRequest(new ApiResponseDto<object> { Success = false, ErrorMessage = "Failed to create subscription plan" });

                return CreatedAtAction(nameof(GetSubscriptionPlanById), new { id = ((dynamic)plan).Id }, 
                    new ApiResponseDto<object> { Success = true, Data = plan });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription plan");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpPut("plans/{id}")]
        public async Task<ActionResult<ApiResponseDto<object>>> UpdateSubscriptionPlan(int id, [FromBody] object updateDto)
        {
            try
            {
                var plan = await _subscriptionServiceClient.UpdateSubscriptionPlanAsync(id, updateDto);
                if (plan == null)
                    return NotFound(new ApiResponseDto<object> { Success = false, ErrorMessage = "Subscription plan not found" });

                return Ok(new ApiResponseDto<object> { Success = true, Data = plan });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating subscription plan {id}");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpDelete("plans/{id}")]
        public async Task<ActionResult> DeleteSubscriptionPlan(int id)
        {
            try
            {
                var result = await _subscriptionServiceClient.DeleteSubscriptionPlanAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting subscription plan {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
