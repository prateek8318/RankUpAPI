using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Interfaces;
using Common.Services;

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
        private readonly ILanguageService _languageService;

        public SubscriptionPlansController(
            ISubscriptionPlanService subscriptionPlanService,
            ILogger<SubscriptionPlansController> logger,
            ILanguageService languageService)
        {
            _subscriptionPlanService = subscriptionPlanService;
            _logger = logger;
            _languageService = languageService;
        }

        /// <summary>
        /// Create a new subscription plan
        /// </summary>
        /// <param name="createPlanDto">Plan creation details</param>
        /// <returns>Created plan details</returns>
        [HttpPost]
        public async Task<ActionResult<SubscriptionPlanDto>> CreatePlan([FromBody] CreateSubscriptionPlanDto? createPlanDto)
        {
            if (createPlanDto == null)
                return BadRequest(new { success = false, message = "Request body is required." });
            try
            {
                var result = await _subscriptionPlanService.CreatePlanAsync(createPlanDto);
                return CreatedAtAction(nameof(GetPlanById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Duplicate subscription plan create blocked");
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription plan");
                var message = HttpContext.RequestServices.GetService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>()?.EnvironmentName == "Development"
                    ? ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "")
                    : "Internal server error";
                return StatusCode(500, new { success = false, message });
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
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Duplicate subscription plan update blocked: {PlanId}", id);
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription plan: {PlanId}", id);
                var message = HttpContext.RequestServices.GetService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>()?.EnvironmentName == "Development"
                    ? ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "")
                    : "Internal server error";
                return StatusCode(500, new { success = false, message });
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
                var message = HttpContext.RequestServices.GetService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>()?.EnvironmentName == "Development"
                    ? ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "")
                    : "Internal server error";
                return StatusCode(500, new { success = false, message });
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
        /// Get all subscription plans. Optionally filter by Master Service ExamId.
        /// </summary>
        /// <param name="examId">Optional: filter by Master Service Exam Id (dynamic exam from Master Service)</param>
        /// <param name="language">Optional: language code from Master Service (e.g. en, hi). Default from header.</param>
        /// <returns>List of plans</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubscriptionPlanListDto>>> GetAllPlans([FromQuery] int? examId = null, [FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                IEnumerable<SubscriptionPlanListDto> result;
                if (examId.HasValue)
                    result = await _subscriptionPlanService.GetPlansByExamIdAsync(examId.Value, currentLanguage);
                else
                    result = await _subscriptionPlanService.GetAllPlansAsync(currentLanguage);
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
                _logger.LogError(ex, "Error retrieving subscription plans");
                return StatusCode(500, new { success = false, message = "Error fetching subscription plans" });
            }
        }

        /// <summary>
        /// Get paginated plans for admin (includes active + inactive by default).
        /// </summary>
        [HttpGet("paged")]
        public async Task<ActionResult<SubscriptionPlanPagedResponseDto>> GetAllPlansPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] bool includeInactive = true,
            [FromQuery] int? examId = null,
            [FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var request = new SubscriptionPlanPagedRequestDto
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    IncludeInactive = includeInactive,
                    ExamId = examId,
                    Language = currentLanguage
                };

                var result = await _subscriptionPlanService.GetPlansPagedAsync(request);
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
                _logger.LogError(ex, "Error retrieving paginated subscription plans");
                return StatusCode(500, new { success = false, message = "Error fetching subscription plans" });
            }
        }

        /// <summary>
        /// Get plans by Master Service Exam Id (dynamic exam selection from Master Service).
        /// </summary>
        /// <param name="examId">Master Service Exam Id</param>
        /// <param name="language">Optional: language code (from Master Service)</param>
        [HttpGet("by-exam-id/{examId:int}")]
        public async Task<ActionResult<IEnumerable<SubscriptionPlanListDto>>> GetPlansByExamId(int examId, [FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var result = await _subscriptionPlanService.GetPlansByExamIdAsync(examId, currentLanguage);
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
                _logger.LogError(ex, "Error retrieving plans for exam id: {ExamId}", examId);
                return StatusCode(500, new { success = false, message = "Error fetching subscription plans" });
            }
        }

        /// <summary>
        /// Get plans by exam category
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
        /// Get all active subscription plans
        /// </summary>
        /// <returns>List of active plans</returns>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<SubscriptionPlanListDto>>> GetActivePlans([FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var result = await _subscriptionPlanService.GetActivePlansAsync(currentLanguage);
                return Ok(new
                {
                    success = true,
                    data = result,
                    language = currentLanguage,
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
        /// Get subscription plan statistics
        /// </summary>
        /// <returns>Subscription plan statistics</returns>
        [HttpGet("stats")]
        public async Task<ActionResult<SubscriptionPlanStatsDto>> GetStats()
        {
            try
            {
                var result = await _subscriptionPlanService.GetStatsAsync();
                return Ok(new
                {
                    success = true,
                    data = result,
                    message = "Subscription plan statistics fetched successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plan statistics");
                return StatusCode(500, new { success = false, message = "Error fetching subscription plan statistics" });
            }
        }
    }
}
