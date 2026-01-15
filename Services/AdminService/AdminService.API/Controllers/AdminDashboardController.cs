using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdminService.Application.DTOs;
using AdminService.Application.Interfaces;

namespace AdminService.API.Controllers
{
    /// <summary>
    /// Admin Dashboard Controller - FR-ADM-02
    /// Aggregates metrics from multiple services
    /// </summary>
    [Route("api/admin/dashboard")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IDashboardAggregationService _dashboardAggregationService;
        private readonly IAuditLogService _auditLogService;
        private readonly ILogger<AdminDashboardController> _logger;

        public AdminDashboardController(
            IDashboardAggregationService dashboardAggregationService,
            IAuditLogService auditLogService,
            ILogger<AdminDashboardController> logger)
        {
            _dashboardAggregationService = dashboardAggregationService;
            _auditLogService = auditLogService;
            _logger = logger;
        }

        /// <summary>
        /// Get aggregated dashboard metrics from all services
        /// </summary>
        [HttpGet("metrics")]
        public async Task<ActionResult<AdminDashboardMetricsDto>> GetDashboardMetrics()
        {
            var startTime = DateTime.UtcNow;
            var adminId = GetAdminIdFromToken();

            try
            {
                var result = await _dashboardAggregationService.GetAggregatedDashboardMetricsAsync();
                
                var responseTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
                await _auditLogService.LogAuditAsync(
                    adminId, "GET_DASHBOARD_METRICS", "AdminService", 
                    "/api/admin/dashboard/metrics", "GET", 
                    null, System.Text.Json.JsonSerializer.Serialize(result), 
                    200, GetIpAddress(), GetUserAgent(), (long)responseTime);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard metrics");
                await _auditLogService.LogAuditAsync(
                    adminId, "GET_DASHBOARD_METRICS", "AdminService", 
                    "/api/admin/dashboard/metrics", "GET", 
                    null, null, 500, GetIpAddress(), GetUserAgent(), 
                    (long)(DateTime.UtcNow - startTime).TotalMilliseconds, ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get metric trends
        /// </summary>
        [HttpGet("trends")]
        public async Task<ActionResult<MetricTrendsDto>> GetMetricTrends()
        {
            try
            {
                var result = await _dashboardAggregationService.GetMetricTrendsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting metric trends");
                return StatusCode(500, "Internal server error");
            }
        }

        private int GetAdminIdFromToken()
        {
            var adminIdClaim = User.FindFirst("AdminId") ?? User.FindFirst("UserId") ?? User.FindFirst("sub");
            if (adminIdClaim != null && int.TryParse(adminIdClaim.Value, out int adminId))
            {
                return adminId;
            }
            return 0;
        }

        private string? GetIpAddress()
        {
            return HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        private string? GetUserAgent()
        {
            return Request.Headers["User-Agent"].ToString();
        }
    }
}
