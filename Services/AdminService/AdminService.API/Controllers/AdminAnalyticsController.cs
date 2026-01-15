using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdminService.Application.Interfaces;
using AdminService.Application.DTOs;

namespace AdminService.API.Controllers
{
    /// <summary>
    /// Admin Analytics Controller - FR-ADM-09
    /// Orchestrates calls to AnalyticsService
    /// </summary>
    [Route("api/admin/analytics")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminAnalyticsController : ControllerBase
    {
        private readonly IAnalyticsServiceClient _analyticsServiceClient;
        private readonly ILogger<AdminAnalyticsController> _logger;

        public AdminAnalyticsController(
            IAnalyticsServiceClient analyticsServiceClient,
            ILogger<AdminAnalyticsController> logger)
        {
            _analyticsServiceClient = analyticsServiceClient;
            _logger = logger;
        }

        [HttpGet("users")]
        public async Task<ActionResult<ApiResponseDto<object>>> GetUserAnalytics([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var analytics = await _analyticsServiceClient.GetUserAnalyticsAsync(startDate, endDate);
                return Ok(new ApiResponseDto<object> { Success = true, Data = analytics });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user analytics");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpGet("quizzes")]
        public async Task<ActionResult<ApiResponseDto<object>>> GetQuizAnalytics([FromQuery] int? quizId = null, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var analytics = await _analyticsServiceClient.GetQuizAnalyticsAsync(quizId, startDate, endDate);
                return Ok(new ApiResponseDto<object> { Success = true, Data = analytics });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quiz analytics");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpGet("subscriptions")]
        public async Task<ActionResult<ApiResponseDto<object>>> GetSubscriptionAnalytics([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var analytics = await _analyticsServiceClient.GetSubscriptionAnalyticsAsync(startDate, endDate);
                return Ok(new ApiResponseDto<object> { Success = true, Data = analytics });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription analytics");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }
    }
}
