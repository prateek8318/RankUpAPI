using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Application.Services;
using System.ComponentModel.DataAnnotations;
using SubscriptionService.Application.Interfaces;

namespace SubscriptionService.API.Controllers
{
    /// <summary>
    /// Admin Subscription Management Controller
    /// </summary>
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminSubscriptionController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ISubscriptionPlanService _subscriptionPlanService;
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly ILogger<AdminSubscriptionController> _logger;

        public AdminSubscriptionController(
            IPaymentService paymentService,
            ISubscriptionPlanService subscriptionPlanService,
            IUserSubscriptionService userSubscriptionService,
            ILogger<AdminSubscriptionController> logger)
        {
            _paymentService = paymentService;
            _subscriptionPlanService = subscriptionPlanService;
            _userSubscriptionService = userSubscriptionService;
            _logger = logger;
        }

        /// <summary>
        /// Get subscription statistics dashboard
        /// </summary>
        /// <returns>Subscription statistics</returns>
        [HttpGet("statistics")]
        public async Task<ActionResult<SubscriptionStatisticsDto>> GetStatistics()
        {
            try
            {
                var result = await _paymentService.GetStatisticsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription statistics");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get paginated list of payments
        /// </summary>
        /// <param name="request">Pagination and filter parameters</param>
        /// <returns>Paginated payments list</returns>
        [HttpGet("payments")]
        public async Task<ActionResult<PagedResponseDto<PaymentDto>>> GetPagedPayments([FromQuery] AdminPaymentListRequestDto request)
        {
            try
            {
                var (payments, totalCount) = await _paymentService.GetPagedPaymentsAsync(request);
                var response = new PagedResponseDto<PaymentDto>
                {
                    Items = payments.ToList(),
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paginated payments");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get paginated list of user subscriptions
        /// </summary>
        /// <param name="request">Pagination and filter parameters</param>
        /// <returns>Paginated user subscriptions list</returns>
        [HttpGet("user-subscriptions")]
        public async Task<ActionResult<PagedResponseDto<UserSubscriptionDto>>> GetPagedUserSubscriptions([FromQuery] AdminUserSubscriptionListRequestDto request)
        {
            try
            {
                var allSubscriptions = await _userSubscriptionService.GetAllUserSubscriptionsAsync();
                var filtered = allSubscriptions.AsQueryable();

                if (request.UserId.HasValue)
                    filtered = filtered.Where(x => x.UserId == request.UserId.Value);
                if (!string.IsNullOrWhiteSpace(request.Status))
                    filtered = filtered.Where(x => string.Equals(x.Status, request.Status, StringComparison.OrdinalIgnoreCase));

                var totalCount = filtered.Count();
                var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
                var pageSize = request.PageSize < 1 ? 20 : request.PageSize;
                var items = filtered
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(new PagedResponseDto<UserSubscriptionDto>
                {
                    Items = items,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalCount <= 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paginated user subscriptions");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get subscription plans with pagination and filters
        /// </summary>
        /// <param name="request">Pagination and filter parameters</param>
        /// <returns>Paginated subscription plans list</returns>
        [HttpGet("plans")]
        public async Task<ActionResult<SubscriptionPlanPagedResponseDto>> GetPagedPlans([FromQuery] AdminSubscriptionListRequestDto request)
        {
            try
            {
                var result = await _subscriptionPlanService.GetPlansPagedAsync(new SubscriptionPlanPagedRequestDto
                {
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    IncludeInactive = request.IncludeInactive,
                    ExamId = request.ExamId,
                    Language = request.Language
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paginated subscription plans");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Create a new subscription plan
        /// </summary>
        /// <param name="createPlanDto">Plan creation details</param>
        /// <returns>Created subscription plan</returns>
        [HttpPost("plans")]
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
        /// Get subscription plan by ID
        /// </summary>
        /// <param name="id">Plan ID</param>
        /// <returns>Subscription plan details</returns>
        [HttpGet("plans/{id}")]
        public async Task<ActionResult<SubscriptionPlanDto>> GetPlanById(int id)
        {
            try
            {
                var result = await _subscriptionPlanService.GetPlanByIdAsync(id);
                if (result == null)
                    return NotFound($"Subscription plan with ID {id} not found");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription plan: {PlanId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update subscription plan
        /// </summary>
        /// <param name="id">Plan ID</param>
        /// <param name="updatePlanDto">Plan update details</param>
        /// <returns>Updated subscription plan</returns>
        [HttpPut("plans/{id}")]
        public async Task<ActionResult<SubscriptionPlanDto>> UpdatePlan(int id, [FromBody] UpdateSubscriptionPlanDto updatePlanDto)
        {
            try
            {
                var result = await _subscriptionPlanService.UpdatePlanAsync(id, updatePlanDto);
                if (result == null)
                    return NotFound($"Subscription plan with ID {id} not found");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription plan: {PlanId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete subscription plan (soft delete)
        /// </summary>
        /// <param name="id">Plan ID</param>
        /// <returns>No content</returns>
        [HttpDelete("plans/{id}")]
        public async Task<ActionResult> DeletePlan(int id)
        {
            try
            {
                var success = await _subscriptionPlanService.DeletePlanAsync(id);
                if (!success)
                    return NotFound($"Subscription plan with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subscription plan: {PlanId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Toggle plan active status
        /// </summary>
        /// <param name="request">Toggle status request</param>
        /// <returns>No content</returns>
        [HttpPut("plans/toggle-status")]
        public async Task<ActionResult> TogglePlanStatus([FromBody] TogglePlanStatusDto request)
        {
            try
            {
                var plan = await _subscriptionPlanService.GetPlanByIdAsync(request.PlanId);
                if (plan == null)
                    return NotFound($"Subscription plan with ID {request.PlanId} not found");

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
                    IsActive = request.IsActive,
                    Translations = plan.Translations?.ToList()
                };

                await _subscriptionPlanService.UpdatePlanAsync(request.PlanId, updateDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling plan status: {PlanId}", request.PlanId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update plan popularity and recommendation flags
        /// </summary>
        /// <param name="request">Update popularity request</param>
        /// <returns>No content</returns>
        [HttpPut("plans/update-popularity")]
        public async Task<ActionResult> UpdatePlanPopularity([FromBody] UpdatePlanPopularityDto request)
        {
            try
            {
                var plan = await _subscriptionPlanService.GetPlanByIdAsync(request.PlanId);
                if (plan == null)
                    return NotFound($"Subscription plan with ID {request.PlanId} not found");

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
                    IsPopular = request.IsPopular,
                    IsRecommended = request.IsRecommended,
                    CardColorTheme = plan.CardColorTheme,
                    SortOrder = plan.SortOrder,
                    IsActive = plan.IsActive,
                    Translations = plan.Translations?.ToList()
                };

                await _subscriptionPlanService.UpdatePlanAsync(request.PlanId, updateDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating plan popularity: {PlanId}", request.PlanId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Toggle plan popular status
        /// </summary>
        /// <param name="planId">Plan ID</param>
        /// <returns>No content</returns>
        [HttpPut("plans/{planId}/toggle-popular")]
        public async Task<ActionResult> TogglePlanPopular(int planId)
        {
            try
            {
                var plan = await _subscriptionPlanService.GetPlanByIdAsync(planId);
                if (plan == null)
                    return NotFound($"Subscription plan with ID {planId} not found");

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
                    IsPopular = !plan.IsPopular,
                    IsRecommended = plan.IsRecommended,
                    CardColorTheme = plan.CardColorTheme,
                    SortOrder = plan.SortOrder,
                    IsActive = plan.IsActive,
                    Translations = plan.Translations?.ToList()
                };

                await _subscriptionPlanService.UpdatePlanAsync(planId, updateDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling plan popularity: {PlanId}", planId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Toggle plan recommended status
        /// </summary>
        /// <param name="planId">Plan ID</param>
        /// <returns>No content</returns>
        [HttpPut("plans/{planId}/toggle-recommended")]
        public async Task<ActionResult> TogglePlanRecommended(int planId)
        {
            try
            {
                var plan = await _subscriptionPlanService.GetPlanByIdAsync(planId);
                if (plan == null)
                    return NotFound($"Subscription plan with ID {planId} not found");

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
                    IsRecommended = !plan.IsRecommended,
                    CardColorTheme = plan.CardColorTheme,
                    SortOrder = plan.SortOrder,
                    IsActive = plan.IsActive,
                    Translations = plan.Translations?.ToList()
                };

                await _subscriptionPlanService.UpdatePlanAsync(planId, updateDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling plan recommendation: {PlanId}", planId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Bulk update multiple plans
        /// </summary>
        /// <param name="request">Bulk update request</param>
        /// <returns>No content</returns>
        [HttpPut("plans/bulk-update")]
        public async Task<ActionResult> BulkUpdatePlans([FromBody] BulkUpdatePlansDto request)
        {
            try
            {
                foreach (var item in request.Items)
                {
                    var plan = await _subscriptionPlanService.GetPlanByIdAsync(item.PlanId);
                    if (plan == null)
                        continue;

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
                        IsPopular = item.IsPopular ?? plan.IsPopular,
                        IsRecommended = item.IsRecommended ?? plan.IsRecommended,
                        CardColorTheme = plan.CardColorTheme,
                        SortOrder = item.SortOrder ?? plan.SortOrder,
                        IsActive = item.IsActive ?? plan.IsActive,
                        Translations = plan.Translations?.ToList()
                    };

                    await _subscriptionPlanService.UpdatePlanAsync(item.PlanId, updateDto);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk updating plans");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get user subscription details
        /// </summary>
        /// <param name="id">User subscription ID</param>
        /// <returns>User subscription details</returns>
        [HttpGet("user-subscriptions/{id}")]
        public async Task<ActionResult<UserSubscriptionDto>> GetUserSubscriptionById(int id)
        {
            try
            {
                var result = await _userSubscriptionService.GetSubscriptionByIdAsync(id);
                if (result == null)
                    return NotFound($"User subscription with ID {id} not found");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user subscription: {SubscriptionId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Cancel user subscription
        /// </summary>
        /// <param name="request">Cancel subscription request</param>
        /// <returns>No content</returns>
        [HttpPost("user-subscriptions/cancel")]
        public async Task<ActionResult> CancelUserSubscription([FromBody] CancelSubscriptionDto request)
        {
            try
            {
                var success = await _userSubscriptionService.CancelSubscriptionAsync(request);
                if (!success)
                    return NotFound($"User subscription with ID {request.SubscriptionId} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling user subscription: {SubscriptionId}", request.SubscriptionId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Extend user subscription
        /// </summary>
        /// <param name="request">Extend subscription request</param>
        /// <returns>Updated user subscription</returns>
        [HttpPost("user-subscriptions/extend")]
        public async Task<ActionResult<UserSubscriptionDto>> ExtendUserSubscription([FromBody] ExtendSubscriptionDto request)
        {
            try
            {
                var existing = await _userSubscriptionService.GetSubscriptionByIdAsync(request.SubscriptionId);
                if (existing == null)
                    return NotFound($"User subscription with ID {request.SubscriptionId} not found");

                var result = await _userSubscriptionService.RenewSubscriptionAsync(new RenewSubscriptionDto
                {
                    SubscriptionId = request.SubscriptionId,
                    AutoRenewal = existing.AutoRenewal
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extending user subscription: {SubscriptionId}", request.SubscriptionId);
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class ExtendSubscriptionDto
    {
        [Required]
        public int SubscriptionId { get; set; }

        [Required]
        [Range(1, 365)]
        public int Days { get; set; }
    }

    public class AdminUserSubscriptionListRequestDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int? UserId { get; set; }
        public string? Status { get; set; }
    }

    public class AdminSubscriptionListRequestDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public bool IncludeInactive { get; set; } = true;
        public int? ExamId { get; set; }
        public string? Language { get; set; }
    }

    public class TogglePlanStatusDto
    {
        [Required]
        public int PlanId { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }

    public class UpdatePlanPopularityDto
    {
        [Required]
        public int PlanId { get; set; }

        public bool IsPopular { get; set; }
        public bool IsRecommended { get; set; }
    }

    public class BulkUpdatePlansDto
    {
        [Required]
        public List<BulkUpdatePlanItemDto> Items { get; set; } = new();
    }

    public class BulkUpdatePlanItemDto
    {
        [Required]
        public int PlanId { get; set; }

        public bool? IsActive { get; set; }
        public bool? IsPopular { get; set; }
        public bool? IsRecommended { get; set; }
        public int? SortOrder { get; set; }
    }
}
