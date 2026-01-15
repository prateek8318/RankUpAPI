using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Interfaces;

namespace SubscriptionService.API.Controllers
{
    /// <summary>
    /// User Controller for Subscription Plans
    /// Handles user-facing subscription plan operations
    /// </summary>
    [Route("api/user/[controller]")]
    [ApiController]
    [Authorize]
    public class UserPlansController : ControllerBase
    {
        private readonly ISubscriptionPlanService _subscriptionPlanService;
        private readonly ILogger<UserPlansController> _logger;

        public UserPlansController(
            ISubscriptionPlanService subscriptionPlanService,
            ILogger<UserPlansController> logger)
        {
            _subscriptionPlanService = subscriptionPlanService;
            _logger = logger;
        }

        /// <summary>
        /// Get subscription plans by exam category
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
    }
}
