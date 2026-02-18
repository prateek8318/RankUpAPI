using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Interfaces;

namespace SubscriptionService.API.Controllers
{
    /// <summary>
    /// Admin Controller for Subscription Plans
    /// Handles plan management within SubscriptionService.
    /// </summary>
    // NOTE:
    // Route is aligned with AdminService's SubscriptionServiceClient:
    //  - GET    /api/admin/subscription-plans
    //  - GET    /api/admin/subscription-plans/{id}
    //  - GET    /api/admin/subscription-plans/active
    //  - POST   /api/admin/subscription-plans
    //  - PUT    /api/admin/subscription-plans/{id}
    //  - DELETE /api/admin/subscription-plans/{id}
    [Route("api/admin/subscription-plans")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SubscriptionPlansController : ControllerBase
    {
        private readonly ISubscriptionPlanService _subscriptionPlanService;
        private readonly ILogger<SubscriptionPlansController> _logger;

        public SubscriptionPlansController(
            ISubscriptionPlanService subscriptionPlanService,
            ILogger<SubscriptionPlansController> logger)
        {
            _subscriptionPlanService = subscriptionPlanService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new subscription plan
        /// </summary>
        /// <param name="createPlanDto">Plan creation details</param>
        /// <returns>Created plan details</returns>
        [HttpPost]
        public async Task<ActionResult<SubscriptionPlanDto>> CreatePlan([FromBody] CreateSubscriptionPlanDto createPlanDto)
        {
            try
            {
                var result = await _subscriptionPlanService.CreatePlanAsync(createPlanDto);
                return CreatedAtAction(nameof(GetPlanById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription plan");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update an existing subscription plan
        /// </summary>
        /// <param name="id">Plan ID</param>
        /// <param name="updatePlanDto">Plan update details</param>
        /// <returns>Updated plan details</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<SubscriptionPlanDto>> UpdatePlan(int id, [FromBody] UpdateSubscriptionPlanDto updatePlanDto)
        {
            try
            {
                var result = await _subscriptionPlanService.UpdatePlanAsync(id, updatePlanDto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Subscription plan not found: {PlanId}", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription plan: {PlanId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete a subscription plan (soft delete)
        /// </summary>
        /// <param name="id">Plan ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeletePlan(int id)
        {
            try
            {
                var result = await _subscriptionPlanService.DeletePlanAsync(id);
                if (!result)
                    return NotFound($"Subscription plan with ID {id} not found");

                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subscription plan: {PlanId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get subscription plan by ID
        /// </summary>
        /// <param name="id">Plan ID</param>
        /// <returns>Plan details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SubscriptionPlanDto>> GetPlanById(int id)
        {
            try
            {
                var result = await _subscriptionPlanService.GetPlanByIdAsync(id);
                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plan: {PlanId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get all subscription plans
        /// </summary>
        /// <returns>List of all plans</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubscriptionPlanListDto>>> GetAllPlans()
        {
            try
            {
                var result = await _subscriptionPlanService.GetAllPlansAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all subscription plans");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get plans by exam category
        /// </summary>
        /// <param name="examCategory">Exam category filter</param>
        /// <returns>List of plans for the exam category</returns>
        [HttpGet("by-exam/{examCategory}")]
        public async Task<ActionResult<IEnumerable<SubscriptionPlanListDto>>> GetPlansByExamCategory(string examCategory)
        {
            try
            {
                var result = await _subscriptionPlanService.GetPlansByExamCategoryAsync(examCategory);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving plans for exam category: {ExamCategory}", examCategory);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get all active subscription plans
        /// </summary>
        /// <returns>List of active plans</returns>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<SubscriptionPlanListDto>>> GetActivePlans()
        {
            try
            {
                var result = await _subscriptionPlanService.GetActivePlansAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active subscription plans");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
