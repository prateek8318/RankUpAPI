using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Interfaces;
using Common.Services;

namespace SubscriptionService.API.Controllers
{
    /// <summary>
    /// Admin Subscription Plans Management Controller
    /// Handles plan management within SubscriptionService.
    /// </summary>
    [Route("api/admin/subscription-plans")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminSubscriptionPlansController : ControllerBase
    {
        private const string InternalServerErrorMessage = "Internal server error";
        private const string ConflictOperationMessage = "Operation could not be completed due to current plan state.";
        private const string ResourceNotFoundMessage = "Requested resource was not found.";
        private readonly ISubscriptionPlanService _subscriptionPlanService;
        private readonly ILogger<AdminSubscriptionPlansController> _logger;
        private readonly ILanguageService _languageService;

        public AdminSubscriptionPlansController(
            ISubscriptionPlanService subscriptionPlanService,
            ILogger<AdminSubscriptionPlansController> logger,
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
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<SubscriptionPlanDto>> CreatePlan([FromForm] CreateSubscriptionPlanDto? createPlanDto)
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
                return Conflict(new { success = false, message = ConflictOperationMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription plan");
                return StatusCode(500, new { success = false, message = InternalServerErrorMessage });
            }
        }

        /// <summary>
        /// Update an existing subscription plan
        /// </summary>
        /// <param name="id">Plan ID</param>
        /// <param name="updatePlanDto">Plan update details</param>
        /// <returns>Updated plan details</returns>
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<SubscriptionPlanDto>> UpdatePlan(int id, [FromForm] UpdateSubscriptionPlanDto updatePlanDto)
        {
            try
            {
                var result = await _subscriptionPlanService.UpdatePlanAsync(id, updatePlanDto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Subscription plan not found: {PlanId}", id);
                return NotFound(new { success = false, message = ResourceNotFoundMessage });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Duplicate subscription plan update blocked: {PlanId}", id);
                return Conflict(new { success = false, message = ConflictOperationMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription plan: {PlanId}", id);
                return StatusCode(500, new { success = false, message = InternalServerErrorMessage });
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
                return StatusCode(500, new { success = false, message = InternalServerErrorMessage });
            }
        }

        /// <summary>
        /// Toggle (or explicitly set) plan active status.
        /// Backward-compatible endpoint for clients calling /{id}/toggle-status.
        /// </summary>
        /// <param name="id">Plan ID</param>
        /// <param name="request">Optional explicit status; if omitted, status is toggled</param>
        /// <returns>Updated status payload</returns>
        [HttpPut("{id}/toggle-status")]
        public async Task<ActionResult<object>> TogglePlanStatusById(int id, [FromBody] TogglePlanStatusByIdRequestDto? request = null)
        {
            try
            {
                var plan = await _subscriptionPlanService.GetPlanByIdAsync(id);
                if (plan == null)
                    return NotFound(new { success = false, message = $"Subscription plan with ID {id} not found" });

                var newIsActive = request?.IsActive ?? !plan.IsActive;
                var updateDto = new UpdateSubscriptionPlanDto
                {
                    Name = plan.Name,
                    Description = plan.Description,
                    Type = plan.Type,
                    Price = plan.Price,
                    Currency = plan.Currency,
                    TestPapersCount = plan.TestPapersCount,
                    Discount = plan.Discount,
                    Duration = plan.Duration,
                    DurationType = plan.DurationType,
                    ValidityDays = plan.ValidityDays,
                    ExamId = plan.ExamId,
                    ExamCategory = plan.ExamCategory,
                    Features = plan.Features,
                    ImageUrl = plan.ImageUrl,
                    IsPopular = plan.IsPopular,
                    IsRecommended = plan.IsRecommended,
                    CardColorTheme = plan.CardColorTheme,
                    SortOrder = plan.SortOrder,
                    IsActive = newIsActive,
                    Translations = plan.Translations?.ToList()
                };

                await _subscriptionPlanService.UpdatePlanAsync(id, updateDto);

                return Ok(new
                {
                    success = true,
                    data = new { planId = id, isActive = newIsActive },
                    message = "Plan status updated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling subscription plan status: {PlanId}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Toggle (or explicitly set) plan popular status.
        /// Backward-compatible endpoint for clients calling /{id}/toggle-popular.
        /// </summary>
        /// <param name="id">Plan ID</param>
        /// <param name="request">Optional explicit status; if omitted, status is toggled</param>
        /// <returns>Updated status payload</returns>
        [HttpPut("{id}/toggle-popular")]
        public async Task<ActionResult<object>> TogglePlanPopularById(int id, [FromBody] TogglePlanPopularByIdRequestDto? request = null)
        {
            try
            {
                var plan = await _subscriptionPlanService.GetPlanByIdAsync(id);
                if (plan == null)
                    return NotFound(new { success = false, message = $"Subscription plan with ID {id} not found" });

                var newIsPopular = request?.IsPopular ?? !plan.IsPopular;
                var updateDto = new UpdateSubscriptionPlanDto
                {
                    Name = plan.Name,
                    Description = plan.Description,
                    Type = plan.Type,
                    Price = plan.Price,
                    Currency = plan.Currency,
                    TestPapersCount = plan.TestPapersCount,
                    Discount = plan.Discount,
                    Duration = plan.Duration,
                    DurationType = plan.DurationType,
                    ValidityDays = plan.ValidityDays,
                    ExamId = plan.ExamId,
                    ExamCategory = plan.ExamCategory,
                    Features = plan.Features,
                    ImageUrl = plan.ImageUrl,
                    IsPopular = newIsPopular,
                    IsRecommended = plan.IsRecommended,
                    CardColorTheme = plan.CardColorTheme,
                    SortOrder = plan.SortOrder,
                    IsActive = plan.IsActive,
                    Translations = plan.Translations?.ToList()
                };

                await _subscriptionPlanService.UpdatePlanAsync(id, updateDto);

                return Ok(new
                {
                    success = true,
                    data = new { planId = id, isPopular = newIsPopular },
                    message = "Plan popular status updated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling subscription plan popular status: {PlanId}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Toggle (or explicitly set) plan recommended status.
        /// Backward-compatible endpoint for clients calling /{id}/toggle-recommended.
        /// </summary>
        /// <param name="id">Plan ID</param>
        /// <param name="request">Optional explicit status; if omitted, status is toggled</param>
        /// <returns>Updated status payload</returns>
        [HttpPut("{id}/toggle-recommended")]
        public async Task<ActionResult<object>> TogglePlanRecommendedById(int id, [FromBody] TogglePlanRecommendedByIdRequestDto? request = null)
        {
            try
            {
                var plan = await _subscriptionPlanService.GetPlanByIdAsync(id);
                if (plan == null)
                    return NotFound(new { success = false, message = $"Subscription plan with ID {id} not found" });

                var newIsRecommended = request?.IsRecommended ?? !plan.IsRecommended;
                var updateDto = new UpdateSubscriptionPlanDto
                {
                    Name = plan.Name,
                    Description = plan.Description,
                    Type = plan.Type,
                    Price = plan.Price,
                    Currency = plan.Currency,
                    TestPapersCount = plan.TestPapersCount,
                    Discount = plan.Discount,
                    Duration = plan.Duration,
                    DurationType = plan.DurationType,
                    ValidityDays = plan.ValidityDays,
                    ExamId = plan.ExamId,
                    ExamCategory = plan.ExamCategory,
                    Features = plan.Features,
                    ImageUrl = plan.ImageUrl,
                    IsPopular = plan.IsPopular,
                    IsRecommended = newIsRecommended,
                    CardColorTheme = plan.CardColorTheme,
                    SortOrder = plan.SortOrder,
                    IsActive = plan.IsActive,
                    Translations = plan.Translations?.ToList()
                };

                await _subscriptionPlanService.UpdatePlanAsync(id, updateDto);

                return Ok(new
                {
                    success = true,
                    data = new { planId = id, isRecommended = newIsRecommended },
                    message = "Plan recommended status updated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling subscription plan recommended status: {PlanId}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
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
        [HttpPost("create-with-durations")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<PlanWithDurationOptionsDto>> CreatePlanWithDurations([FromForm] CreatePlanWithDurationsFormDataDto? request)
        {
            var createPlanDto = ParseCreatePlanWithDurationsRequest(request);
            if (createPlanDto == null)
                return BadRequest(new { success = false, message = "Invalid multipart payload. Expected FormData with 'data' JSON and optional 'ImageFile'." });
            
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
                return Conflict(new { success = false, message = ConflictOperationMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription plan with durations");
                return StatusCode(500, new { success = false, message = InternalServerErrorMessage });
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
                return NotFound(new { success = false, message = ResourceNotFoundMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding duration options to plan: {PlanId}", planId);
                return StatusCode(500, new { success = false, message = InternalServerErrorMessage });
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
                return NotFound(new { success = false, message = ResourceNotFoundMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating duration option: {DurationOptionId}", durationOptionId);
                return StatusCode(500, new { success = false, message = InternalServerErrorMessage });
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
                return StatusCode(500, new { success = false, message = InternalServerErrorMessage });
            }
        }

        /// <summary>
        /// Get all subscription plans with duration options (Admin View)
        /// </summary>
        /// <returns>List of plans with duration options</returns>
        [HttpGet("durations")]
        public async Task<ActionResult<IEnumerable<PlanWithDurationOptionsDto>>> GetAllPlansWithDurations(
            [FromQuery] string? language = null,
            [FromQuery] int? cursor = null,
            [FromQuery] int limit = 20)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var result = (await _subscriptionPlanService.GetAllPlansWithDurationsAsync(currentLanguage, includeInactive: true))
                    .OrderByDescending(p => p.Id)
                    .ToList();

                if (limit <= 0) limit = 20;
                if (limit > 100) limit = 100;

                var filtered = cursor.HasValue
                    ? result.Where(p => p.Id < cursor.Value).ToList()
                    : result;

                var pageItems = filtered.Take(limit).ToList();
                var hasMore = filtered.Count > pageItems.Count;
                var nextCursor = hasMore ? pageItems.Last().Id : (int?)null;

                return Ok(new
                {
                    success = true,
                    data = pageItems,
                    pagination = new
                    {
                        cursor,
                        limit,
                        nextCursor,
                        hasMore
                    },
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
        /// Create or update subscription plans with duration options
        /// </summary>
        /// <param name="request">Plan creation/update details with duration options</param>
        /// <returns>Created/updated plan details with duration options</returns>
        [HttpPost("durations")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<PlanWithDurationOptionsDto>> CreateOrUpdatePlanWithDurations([FromForm] CreatePlanWithDurationsFormDataDto? request)
        {
            var createPlanDto = ParseCreatePlanWithDurationsRequest(request);
            if (createPlanDto == null && Request.HasFormContentType)
            {
                createPlanDto = ParseCreatePlanWithDurationsFlatForm(Request.Form);
            }
            if (createPlanDto == null)
                return BadRequest(new { success = false, message = "Invalid multipart payload. Expected FormData with 'data' JSON and optional 'ImageFile'." });
            
            try
            {
                var result = await _subscriptionPlanService.UpsertPlanWithDurationsAsync(createPlanDto);
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
                return Conflict(new { success = false, message = ConflictOperationMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription plan with durations");
                return StatusCode(500, new { success = false, message = InternalServerErrorMessage });
            }
        }

        [HttpPost("durations")]
        [Consumes("application/json")]
        public async Task<ActionResult<PlanWithDurationOptionsDto>> CreateOrUpdatePlanWithDurationsJson([FromBody] CreateSubscriptionPlanWithDurationDto? request)
        {
            if (request == null)
                return BadRequest(new { success = false, message = "Request body is required." });

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid request payload.",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            try
            {
                var result = await _subscriptionPlanService.UpsertPlanWithDurationsAsync(request);
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
                return Conflict(new { success = false, message = ConflictOperationMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription plan with durations via JSON");
                return StatusCode(500, new { success = false, message = InternalServerErrorMessage });
            }
        }

        /// <summary>
        /// Backward-compatible endpoint for clients posting multipart data to create plans with durations.
        /// This creates a plan with duration options.
        /// </summary>
        [HttpPost("create-durations")]
        [Consumes("multipart/form-data")]
        public Task<ActionResult<PlanWithDurationOptionsDto>> CreatePlanWithDurationsCompat([FromForm] CreatePlanWithDurationsFormDataDto? request)
        {
            return CreatePlanWithDurations(request);
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
                return StatusCode(500, new { success = false, message = InternalServerErrorMessage });
            }
        }

        private static CreateSubscriptionPlanWithDurationDto? ParseCreatePlanWithDurationsRequest(CreatePlanWithDurationsFormDataDto? request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Data))
                return null;

            try
            {
                var dto = JsonSerializer.Deserialize<CreateSubscriptionPlanWithDurationDto>(
                    request.Data,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (dto == null)
                    return null;

                // For this form-data flow, recommended is controlled via dedicated toggle endpoint.
                dto.IsRecommended = false;
                dto.ImageFile = request.ImageFile;
                return dto;
            }
            catch
            {
                return null;
            }
        }

        private static CreateSubscriptionPlanWithDurationDto? ParseCreatePlanWithDurationsFlatForm(IFormCollection form)
        {
            try
            {
                if (form == null || !form.Keys.Any())
                    return null;

                var dto = new CreateSubscriptionPlanWithDurationDto
                {
                    Name = form["name"].ToString(),
                    Description = form["description"].ToString(),
                    Currency = string.IsNullOrWhiteSpace(form["currency"]) ? "INR" : form["currency"].ToString(),
                    ExamCategory = form["examCategory"].ToString(),
                    CardColorTheme = form["cardColorTheme"].ToString(),
                    ImageUrl = form["imageUrl"].ToString()
                };

                if (int.TryParse(form["id"], out var id)) dto.Id = id;
                if (int.TryParse(form["type"], out var type)) dto.Type = (SubscriptionService.Domain.Entities.PlanType)type;
                if (decimal.TryParse(form["basePrice"], out var basePrice)) dto.BasePrice = basePrice;
                if (int.TryParse(form["testPapersCount"], out var testPapersCount)) dto.TestPapersCount = testPapersCount;
                if (int.TryParse(form["examId"], out var examId)) dto.ExamId = examId;
                if (int.TryParse(form["sortOrder"], out var sortOrder)) dto.SortOrder = sortOrder;
                if (bool.TryParse(form["isPopular"], out var isPopular)) dto.IsPopular = isPopular;
                if (bool.TryParse(form["isRecommended"], out var isRecommended)) dto.IsRecommended = isRecommended;

                var featuresRaw = form["features"].ToString();
                if (!string.IsNullOrWhiteSpace(featuresRaw))
                {
                    dto.Features = JsonSerializer.Deserialize<List<string>>(featuresRaw) ?? new List<string>();
                }

                var durationOptionsRaw = form["durationOptions"].ToString();
                if (!string.IsNullOrWhiteSpace(durationOptionsRaw))
                {
                    dto.DurationOptions = JsonSerializer.Deserialize<List<CreatePlanDurationOptionDto>>(
                        durationOptionsRaw,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<CreatePlanDurationOptionDto>();
                }

                var translationsRaw = form["translations"].ToString();
                if (!string.IsNullOrWhiteSpace(translationsRaw))
                {
                    dto.Translations = JsonSerializer.Deserialize<List<SubscriptionPlanTranslationDto>>(
                        translationsRaw,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<SubscriptionPlanTranslationDto>();
                }

                if (form.Files != null && form.Files.Count > 0)
                {
                    dto.ImageFile = form.Files.GetFile("ImageFile") ?? form.Files[0];
                }

                if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Description) || dto.Type == 0 || dto.DurationOptions == null || dto.DurationOptions.Count == 0)
                    return null;

                return dto;
            }
            catch
            {
                return null;
            }
        }
    }

    public class TogglePlanStatusByIdRequestDto
    {
        public bool? IsActive { get; set; }
    }

    public class TogglePlanPopularByIdRequestDto
    {
        public bool? IsPopular { get; set; }
    }

    public class TogglePlanRecommendedByIdRequestDto
    {
        public bool? IsRecommended { get; set; }
    }

    public class CreatePlanWithDurationsFormDataDto
    {
        public string? Data { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
