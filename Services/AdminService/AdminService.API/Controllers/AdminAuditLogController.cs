using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdminService.Application.DTOs;
using AdminService.Application.Interfaces;

namespace AdminService.API.Controllers
{
    /// <summary>
    /// Admin Audit Log Controller
    /// Provides access to audit logs
    /// </summary>
    [Route("api/admin/audit-logs")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminAuditLogController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;
        private readonly ILogger<AdminAuditLogController> _logger;

        public AdminAuditLogController(
            IAuditLogService auditLogService,
            ILogger<AdminAuditLogController> logger)
        {
            _auditLogService = auditLogService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAuditLogs(
            [FromQuery] int? adminId = null,
            [FromQuery] string? serviceName = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var logs = await _auditLogService.GetAuditLogsAsync(adminId, serviceName, startDate, endDate, page, pageSize);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting audit logs");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
