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
        /// Get subscription plan by ID with duration options
        /// </summary>
        /// <param name="id">Plan ID</param>
        /// <param name="language">Optional language code</param>
        /// <returns>Plan details with duration options</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PlanWithDurationOptionsDto>> GetPlanById(int id, [FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var result = await _subscriptionPlanService.GetPlanWithDurationsAsync(id, currentLanguage);
                if (result == null)
                    return NotFound(new { success = false, message = "Subscription plan not found" });

                return Ok(new
                {
                    success = true,
                    data = result,
                    language = currentLanguage,
                    message = "Subscription plan with duration options retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plan: {PlanId}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get subscription plan by ID with duration options (Alternative endpoint for consistency)
        /// </summary>
        /// <param name="id">Plan ID</param>
        /// <param name="language">Optional language code</param>
        /// <returns>Plan details with duration options</returns>
        [HttpGet("{id}/with-durations")]
        public async Task<ActionResult<PlanWithDurationOptionsDto>> GetPlanByIdWithDurations(int id, [FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var result = await _subscriptionPlanService.GetPlanWithDurationsAsync(id, currentLanguage);
                if (result == null)
                    return NotFound(new { success = false, message = "Subscription plan not found" });

                return Ok(new
                {
                    success = true,
                    data = result,
                    language = currentLanguage,
                    message = "Subscription plan with duration options retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plan: {PlanId}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get all subscription plans with duration options. Optionally filter by Master Service ExamId.
        /// </summary>
        /// <param name="examId">Optional: filter by Master Service Exam Id (dynamic exam from Master Service)</param>
        /// <param name="language">Optional: language code from Master Service (e.g. en, hi). Default from header.</param>
        /// <returns>List of plans with duration options</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlanWithDurationOptionsDto>>> GetAllPlans([FromQuery] int? examId = null, [FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                IEnumerable<PlanWithDurationOptionsDto> result;
                if (examId.HasValue)
                    result = await _subscriptionPlanService.GetActivePlansWithDurationsAsync(currentLanguage, examId);
                else
                    result = await _subscriptionPlanService.GetActivePlansWithDurationsAsync(currentLanguage);
                return Ok(new
                {
                    success = true,
                    data = result,
                    language = currentLanguage,
                    message = "Subscription plans with duration options fetched successfully"
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

        /// <summary>
        /// Create a new subscription plan with duration options
        /// </summary>
        /// <param name="createPlanDto">Plan creation details with duration options</param>
        /// <returns>Created plan details with duration options</returns>
        [HttpPost("with-durations")]
        public async Task<ActionResult<PlanWithDurationOptionsDto>> CreatePlanWithDurations([FromBody] CreateSubscriptionPlanWithDurationDto createPlanDto)
        {
            if (createPlanDto == null)
                return BadRequest(new { success = false, message = "Request body is required." });
            
            try
            {
                var result = await _subscriptionPlanService.CreatePlanWithDurationsAsync(createPlanDto);
                return CreatedAtAction(nameof(GetPlanById), new { id = result.Id }, new
                {
                    success = true,
                    data = result,
                    message = "Subscription plan with duration options created successfully"
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Duplicate subscription plan create blocked");
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription plan with durations");
                var message = HttpContext.RequestServices.GetService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>()?.EnvironmentName == "Development"
                    ? ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "")
                    : "Internal server error";
                return StatusCode(500, new { success = false, message });
            }
        }

        /// <summary>
        /// Create a new subscription plan with duration options (Alternative endpoint)
        /// </summary>
        /// <param name="createPlanDto">Plan creation details with duration options</param>
        /// <returns>Created plan details with duration options</returns>
        [HttpPost("create-with-durations")]
        public async Task<ActionResult<PlanWithDurationOptionsDto>> CreatePlanWithDurationsAlternative([FromBody] CreateSubscriptionPlanWithDurationDto createPlanDto)
        {
            if (createPlanDto == null)
                return BadRequest(new { success = false, message = "Request body is required." });
            
            try
            {
                var result = await _subscriptionPlanService.CreatePlanWithDurationsAsync(createPlanDto);
                return CreatedAtAction(nameof(GetPlanById), new { id = result.Id }, new
                {
                    success = true,
                    data = result,
                    message = "Subscription plan with duration options created successfully"
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Duplicate subscription plan create blocked");
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription plan with durations");
                var message = HttpContext.RequestServices.GetService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>()?.EnvironmentName == "Development"
                    ? ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "")
                    : "Internal server error";
                return StatusCode(500, new { success = false, message });
            }
        }

        
        /// <summary>
        /// Add duration options to an existing plan
        /// </summary>
        /// <param name="planId">Plan ID</param>
        /// <param name="durationOptions">List of duration options to add</param>
        /// <returns>Success status</returns>
        [HttpPost("{planId}/duration-options")]
        public async Task<ActionResult<bool>> AddDurationOptions(int planId, [FromBody] List<CreatePlanDurationOptionDto> durationOptions)
        {
            if (durationOptions == null || !durationOptions.Any())
                return BadRequest(new { success = false, message = "Duration options are required." });

            try
            {
                var result = await _subscriptionPlanService.AddDurationOptionsAsync(planId, durationOptions);
                return Ok(new { success = true, data = result, message = "Duration options added successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Subscription plan not found: {PlanId}", planId);
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding duration options to plan: {PlanId}", planId);
                var message = HttpContext.RequestServices.GetService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>()?.EnvironmentName == "Development"
                    ? ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "")
                    : "Internal server error";
                return StatusCode(500, new { success = false, message });
            }
        }

        /// <summary>
        /// Update a duration option
        /// </summary>
        /// <param name="durationOptionId">Duration option ID</param>
        /// <param name="updateDto">Duration option update details</param>
        /// <returns>Updated duration option</returns>
        [HttpPut("duration-options/{durationOptionId}")]
        public async Task<ActionResult<PlanDurationOptionDto>> UpdateDurationOption(int durationOptionId, [FromBody] UpdatePlanDurationOptionDto updateDto)
        {
            try
            {
                var result = await _subscriptionPlanService.UpdateDurationOptionAsync(durationOptionId, updateDto);
                return Ok(new { success = true, data = result, message = "Duration option updated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Duration option not found: {DurationOptionId}", durationOptionId);
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating duration option: {DurationOptionId}", durationOptionId);
                var message = HttpContext.RequestServices.GetService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>()?.EnvironmentName == "Development"
                    ? ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "")
                    : "Internal server error";
                return StatusCode(500, new { success = false, message });
            }
        }

        /// <summary>
        /// Delete a duration option
        /// </summary>
        /// <param name="durationOptionId">Duration option ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("duration-options/{durationOptionId}")]
        public async Task<ActionResult<bool>> DeleteDurationOption(int durationOptionId)
        {
            try
            {
                var result = await _subscriptionPlanService.DeleteDurationOptionAsync(durationOptionId);
                if (!result)
                    return NotFound(new { success = false, message = "Duration option not found" });

                return Ok(new { success = true, data = result, message = "Duration option deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting duration option: {DurationOptionId}", durationOptionId);
                var message = HttpContext.RequestServices.GetService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>()?.EnvironmentName == "Development"
                    ? ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "")
                    : "Internal server error";
                return StatusCode(500, new { success = false, message });
            }
        }

        /// <summary>
        /// Get all subscription plans with duration options (Admin View)
        /// </summary>
        /// <returns>List of plans with duration options</returns>
        [HttpGet("with-durations")]
        public async Task<ActionResult<IEnumerable<PlanWithDurationOptionsDto>>> GetAllPlansWithDurations([FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var result = await _subscriptionPlanService.GetAllPlansWithDurationsAsync(currentLanguage, includeInactive: true);
                return Ok(new
                {
                    success = true,
                    data = result,
                    language = currentLanguage,
                    message = "Subscription plans with duration options fetched successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plans with durations");
                return StatusCode(500, new { success = false, message = "Error fetching subscription plans" });
            }
        }

        /// <summary>
        /// Get all subscription plans with duration options (Admin View) - Alternative endpoint
        /// </summary>
        /// <returns>List of plans with duration options</returns>
        [HttpGet("all-plans-with-durations")]
        public async Task<ActionResult<IEnumerable<PlanWithDurationOptionsDto>>> GetAllPlansWithDurationsAlternative([FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var result = await _subscriptionPlanService.GetAllPlansWithDurationsAsync(currentLanguage, includeInactive: true);
                return Ok(new
                {
                    success = true,
                    data = result,
                    language = currentLanguage,
                    message = "Subscription plans with duration options fetched successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plans with durations");
                return StatusCode(500, new { success = false, message = "Error fetching subscription plans" });
            }
        }

        /// <summary>
        /// Upload plan image
        /// </summary>
        /// <param name="file">Image file</param>
        /// <returns>Image URL</returns>
        [HttpPost("upload-image")]
        public async Task<ActionResult<string>> UploadPlanImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { success = false, message = "No file uploaded" });

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest(new { success = false, message = "Only image files (jpg, jpeg, png, gif, webp) are allowed" });

                // Validate file size (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                    return BadRequest(new { success = false, message = "File size must be less than 5MB" });

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "subscription-plans");
                
                // Create directory if it doesn't exist
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Save file
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Generate URL
                var imageUrl = $"/images/subscription-plans/{fileName}";

                return Ok(new 
                { 
                    success = true, 
                    data = imageUrl,
                    message = "Image uploaded successfully" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading plan image");
                var message = HttpContext.RequestServices.GetService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>()?.EnvironmentName == "Development"
                    ? ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "")
                    : "Internal server error";
                return StatusCode(500, new { success = false, message });
            }
        }
    }
}
