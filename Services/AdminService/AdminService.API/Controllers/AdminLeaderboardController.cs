using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdminService.Application.Interfaces;
using AdminService.Application.DTOs;

namespace AdminService.API.Controllers
{
    /// <summary>
    /// Admin Leaderboard Controller - FR-ADM-06
    /// Orchestrates calls to LeaderboardService/AnalyticsService
    /// </summary>
    [Route("api/admin/leaderboard")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminLeaderboardController : ControllerBase
    {
        private readonly IAnalyticsServiceClient _analyticsServiceClient;
        private readonly ILogger<AdminLeaderboardController> _logger;

        public AdminLeaderboardController(
            IAnalyticsServiceClient analyticsServiceClient,
            ILogger<AdminLeaderboardController> logger)
        {
            _analyticsServiceClient = analyticsServiceClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<object>>> GetLeaderboard([FromQuery] int? quizId = null, [FromQuery] int limit = 10)
        {
            try
            {
                var leaderboard = await _analyticsServiceClient.GetLeaderboardDataAsync(quizId, limit);
                return Ok(new ApiResponseDto<object> { Success = true, Data = leaderboard });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting leaderboard");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }
    }
}
