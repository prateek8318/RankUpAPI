using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdminService.Application.DTOs;
using AdminService.Application.Interfaces;

namespace AdminService.API.Controllers
{
    /// <summary>
    /// Admin Export Controller
    /// Handles data exports to Excel
    /// </summary>
    [Route("api/admin/exports")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminExportController : ControllerBase
    {
        private readonly IExportService _exportService;
        private readonly ILogger<AdminExportController> _logger;

        public AdminExportController(
            IExportService exportService,
            ILogger<AdminExportController> logger)
        {
            _exportService = exportService;
            _logger = logger;
        }

        [HttpPost("users")]
        public async Task<ActionResult<ExportResultDto>> ExportUsers([FromBody] ExportFilterDto? filter = null)
        {
            try
            {
                var adminId = GetAdminIdFromToken();
                var result = await _exportService.ExportUsersToExcelAsync(filter, adminId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting users");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("exams")]
        public async Task<ActionResult<ExportResultDto>> ExportExams([FromBody] ExportFilterDto? filter = null)
        {
            try
            {
                var adminId = GetAdminIdFromToken();
                var result = await _exportService.ExportExamsToExcelAsync(filter, adminId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting exams");
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

        [HttpGet("logs")]
        public async Task<ActionResult<IEnumerable<ExportLogDto>>> GetExportLogs([FromQuery] int? adminId = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                var logs = await _exportService.GetExportLogsAsync(adminId, page, pageSize);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting export logs");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("logs/{id}")]
        public async Task<ActionResult<ExportLogDto>> GetExportLogById(int id)
        {
            try
            {
                var log = await _exportService.GetExportLogByIdAsync(id);
                if (log == null)
                    return NotFound();

                return Ok(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting export log {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadExport(int id)
        {
            try
            {
                var log = await _exportService.GetExportLogByIdAsync(id);
                if (log == null || string.IsNullOrEmpty(log.FilePath) || !System.IO.File.Exists(log.FilePath))
                    return NotFound("Export file not found");

                var fileBytes = await System.IO.File.ReadAllBytesAsync(log.FilePath);
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", log.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading export {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
