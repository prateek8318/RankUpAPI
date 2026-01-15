using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HomeDashboardService.Application.DTOs;
using HomeDashboardService.Application.Interfaces;

namespace HomeDashboardService.API.Controllers
{
    /// <summary>
    /// Admin Dashboard Controller
    /// </summary>
    [Route("api/admin/[controller]")]
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
    }
}
