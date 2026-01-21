using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HomeDashboardService.Application.DTOs;
using HomeDashboardService.Application.Interfaces;

namespace HomeDashboardService.API.Controllers
{
    /// <summary>
    /// Admin Dashboard Controller
    /// </summary>
    // Use explicit route so the path is stable regardless of controller name
    [Route("api/admin/dashboard")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _adminDashboardService;
        private readonly ILogger<AdminDashboardController> _logger;

        public AdminDashboardController(
            IAdminDashboardService adminDashboardService,
            ILogger<AdminDashboardController> logger)
        {
            _adminDashboardService = adminDashboardService;
            _logger = logger;
        }

        /// <summary>
        /// Get admin dashboard metrics
        /// </summary>
        /// <returns>Admin dashboard metrics</returns>
        [HttpGet("metrics")]
        public async Task<ActionResult<AdminDashboardMetricsDto>> GetAdminDashboardMetrics()
        {
            try
            {
                var result = await _adminDashboardService.GetAdminDashboardMetricsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving admin dashboard metrics");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get metric trends
        /// </summary>
        /// <returns>Metric trends data</returns>
        [HttpGet("trends")]
        public async Task<ActionResult<MetricTrendsDto>> GetMetricTrends()
        {
            try
            {
                var result = await _adminDashboardService.GetMetricTrendsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving metric trends");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Create dashboard banner
        /// </summary>
        /// <param name="banner">Banner data</param>
        /// <returns>Created banner</returns>
        [HttpPost("banners")]
        public async Task<ActionResult<DashboardBannerDto>> CreateBanner([FromBody] CreateBannerDto banner)
        {
            try
            {
                var result = await _adminDashboardService.CreateBannerAsync(banner);
                return CreatedAtAction(nameof(CreateBanner), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating banner");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update dashboard banner
        /// </summary>
        /// <param name="id">Banner ID</param>
        /// <param name="banner">Banner data</param>
        /// <returns>Updated banner</returns>
        [HttpPut("banners/{id}")]
        public async Task<ActionResult<DashboardBannerDto>> UpdateBanner(int id, [FromBody] UpdateBannerDto banner)
        {
            try
            {
                var result = await _adminDashboardService.UpdateBannerAsync(id, banner);
                if (result == null)
                    return NotFound("Banner not found");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating banner");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Create free test
        /// </summary>
        /// <param name="test">Free test data</param>
        /// <returns>Created free test</returns>
        [HttpPost("free-tests")]
        public async Task<ActionResult<FreeTestDto>> CreateFreeTest([FromBody] CreateFreeTestDto test)
        {
            try
            {
                var result = await _adminDashboardService.CreateFreeTestAsync(test);
                return CreatedAtAction(nameof(CreateFreeTest), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating free test");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update free test
        /// </summary>
        /// <param name="id">Test ID</param>
        /// <param name="test">Test data</param>
        /// <returns>Updated free test</returns>
        [HttpPut("free-tests/{id}")]
        public async Task<ActionResult<FreeTestDto>> UpdateFreeTest(int id, [FromBody] UpdateFreeTestDto test)
        {
            try
            {
                var result = await _adminDashboardService.UpdateFreeTestAsync(id, test);
                if (result == null)
                    return NotFound("Free test not found");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating free test");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Create daily target
        /// </summary>
        /// <param name="target">Daily target data</param>
        /// <returns>Created daily target</returns>
        [HttpPost("daily-targets")]
        public async Task<ActionResult<DailyTargetDto>> CreateDailyTarget([FromBody] CreateDailyTargetDto target)
        {
            try
            {
                var result = await _adminDashboardService.CreateDailyTargetAsync(target);
                return CreatedAtAction(nameof(CreateDailyTarget), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating daily target");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update daily target
        /// </summary>
        /// <param name="id">Target ID</param>
        /// <param name="target">Target data</param>
        /// <returns>Updated daily target</returns>
        [HttpPut("daily-targets/{id}")]
        public async Task<ActionResult<DailyTargetDto>> UpdateDailyTarget(int id, [FromBody] UpdateDailyTargetDto target)
        {
            try
            {
                var result = await _adminDashboardService.UpdateDailyTargetAsync(id, target);
                if (result == null)
                    return NotFound("Daily target not found");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating daily target");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Create trending test
        /// </summary>
        /// <param name="test">Trending test data</param>
        /// <returns>Created trending test</returns>
        [HttpPost("trending-tests")]
        public async Task<ActionResult<TrendingTestDto>> CreateTrendingTest([FromBody] CreateTrendingTestDto test)
        {
            try
            {
                var result = await _adminDashboardService.CreateTrendingTestAsync(test);
                return CreatedAtAction(nameof(CreateTrendingTest), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating trending test");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update trending test
        /// </summary>
        /// <param name="id">Test ID</param>
        /// <param name="test">Test data</param>
        /// <returns>Updated trending test</returns>
        [HttpPut("trending-tests/{id}")]
        public async Task<ActionResult<TrendingTestDto>> UpdateTrendingTest(int id, [FromBody] UpdateTrendingTestDto test)
        {
            try
            {
                var result = await _adminDashboardService.UpdateTrendingTestAsync(id, test);
                if (result == null)
                    return NotFound("Trending test not found");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating trending test");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
