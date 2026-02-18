using AdminService.Application.DTOs;
using AdminService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminService.API.Controllers
{
    [ApiController]
    [Route("api/admin/subscription-plans")]
    [Authorize(Roles = "Admin")]
    public class SubscriptionPlanController : ControllerBase
    {
        private readonly ISubscriptionPlanService _subscriptionPlanService;
        private readonly ILogger<SubscriptionPlanController> _logger;

        public SubscriptionPlanController(
            ISubscriptionPlanService subscriptionPlanService,
            ILogger<SubscriptionPlanController> logger)
        {
            _subscriptionPlanService = subscriptionPlanService;
            _logger = logger;
        }

        [HttpGet("stats")]
        public async Task<ActionResult<SubscriptionPlanStatsDto>> GetStats()
        {
            try
            {
                var stats = await _subscriptionPlanService.GetStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription plan stats");
                return StatusCode(500, new { success = false, message = "Error retrieving stats" });
            }
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetAllPlans([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                var plans = await _subscriptionPlanService.GetAllPlansAsync(page, pageSize);
                var totalCount = await _subscriptionPlanService.GetStatsAsync();
                
                return Ok(new
                {
                    success = true,
                    data = plans,
                    pagination = new
                    {
                        page,
                        pageSize,
                        totalCount = totalCount.ActivePlans // Temporary, should implement separate count method
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all subscription plans");
                return StatusCode(500, new { success = false, message = "Error retrieving plans" });
            }
        }

        [HttpGet("filtered")]
        public async Task<ActionResult<object>> GetFilteredPlans([FromQuery] SubscriptionPlanFilterRequest filter)
        {
            try
            {
                var (plans, totalCount) = await _subscriptionPlanService.GetFilteredPlansAsync(filter);
                
                return Ok(new
                {
                    success = true,
                    data = plans,
                    pagination = new
                    {
                        page = filter.Page,
                        pageSize = filter.PageSize,
                        totalCount
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting filtered subscription plans");
                return StatusCode(500, new { success = false, message = "Error retrieving filtered plans" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SubscriptionPlanDto>> GetPlanById(int id)
        {
            try
            {
                var plan = await _subscriptionPlanService.GetPlanByIdAsync(id);
                if (plan == null)
                    return NotFound(new { success = false, message = "Plan not found" });

                return Ok(new { success = true, data = plan });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription plan by ID: {PlanId}", id);
                return StatusCode(500, new { success = false, message = "Error retrieving plan" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<SubscriptionPlanDto>> CreatePlan([FromBody] CreateSubscriptionPlanRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Invalid data", errors = ModelState });

                var plan = await _subscriptionPlanService.CreatePlanAsync(request);
                
                return CreatedAtAction(
                    nameof(GetPlanById),
                    new { id = plan.Id },
                    new { success = true, data = plan });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription plan");
                return StatusCode(500, new { success = false, message = "Error creating plan" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SubscriptionPlanDto>> UpdatePlan(int id, [FromBody] UpdateSubscriptionPlanRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Invalid data", errors = ModelState });

                var plan = await _subscriptionPlanService.UpdatePlanAsync(id, request);
                
                return Ok(new { success = true, data = plan });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Plan not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription plan: {PlanId}", id);
                return StatusCode(500, new { success = false, message = "Error updating plan" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePlan(int id)
        {
            try
            {
                var result = await _subscriptionPlanService.DeletePlanAsync(id);
                if (!result)
                    return NotFound(new { success = false, message = "Plan not found" });

                return Ok(new { success = true, message = "Plan deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subscription plan: {PlanId}", id);
                return StatusCode(500, new { success = false, message = "Error deleting plan" });
            }
        }

        [HttpPatch("{id}/toggle-popular")]
        public async Task<ActionResult> TogglePopular(int id)
        {
            try
            {
                var plan = await _subscriptionPlanService.GetPlanByIdAsync(id);
                if (plan == null)
                    return NotFound(new { success = false, message = "Plan not found" });

                var updateRequest = new UpdateSubscriptionPlanRequest
                {
                    PlanName = plan.PlanName,
                    ExamType = plan.ExamType,
                    Price = plan.Price,
                    Duration = plan.Duration,
                    ColorCode = plan.ColorCode,
                    IsPopular = !plan.IsPopular,
                    IsRecommended = plan.IsRecommended,
                    IsActive = plan.IsActive
                };

                var updatedPlan = await _subscriptionPlanService.UpdatePlanAsync(id, updateRequest);
                
                return Ok(new { success = true, data = updatedPlan });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling popular status: {PlanId}", id);
                return StatusCode(500, new { success = false, message = "Error updating plan" });
            }
        }

        [HttpPatch("{id}/toggle-recommended")]
        public async Task<ActionResult> ToggleRecommended(int id)
        {
            try
            {
                var plan = await _subscriptionPlanService.GetPlanByIdAsync(id);
                if (plan == null)
                    return NotFound(new { success = false, message = "Plan not found" });

                var updateRequest = new UpdateSubscriptionPlanRequest
                {
                    PlanName = plan.PlanName,
                    ExamType = plan.ExamType,
                    Price = plan.Price,
                    Duration = plan.Duration,
                    ColorCode = plan.ColorCode,
                    IsPopular = plan.IsPopular,
                    IsRecommended = !plan.IsRecommended,
                    IsActive = plan.IsActive
                };

                var updatedPlan = await _subscriptionPlanService.UpdatePlanAsync(id, updateRequest);
                
                return Ok(new { success = true, data = updatedPlan });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling recommended status: {PlanId}", id);
                return StatusCode(500, new { success = false, message = "Error updating plan" });
            }
        }

        [HttpPatch("{id}/toggle-status")]
        public async Task<ActionResult> ToggleStatus(int id)
        {
            try
            {
                var plan = await _subscriptionPlanService.GetPlanByIdAsync(id);
                if (plan == null)
                    return NotFound(new { success = false, message = "Plan not found" });

                var updateRequest = new UpdateSubscriptionPlanRequest
                {
                    PlanName = plan.PlanName,
                    ExamType = plan.ExamType,
                    Price = plan.Price,
                    Duration = plan.Duration,
                    ColorCode = plan.ColorCode,
                    IsPopular = plan.IsPopular,
                    IsRecommended = plan.IsRecommended,
                    IsActive = !plan.IsActive
                };

                var updatedPlan = await _subscriptionPlanService.UpdatePlanAsync(id, updateRequest);
                
                return Ok(new { success = true, data = updatedPlan });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling status: {PlanId}", id);
                return StatusCode(500, new { success = false, message = "Error updating plan" });
            }
        }
    }
}
