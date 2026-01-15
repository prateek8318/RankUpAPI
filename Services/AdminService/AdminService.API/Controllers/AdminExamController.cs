using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdminService.Application.Interfaces;
using AdminService.Application.DTOs;
using AdminService.Application.Services;

namespace AdminService.API.Controllers
{
    /// <summary>
    /// Admin Exam Management Controller - FR-ADM-03
    /// Orchestrates calls to ExamService
    /// </summary>
    [Route("api/admin/exams")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminExamController : ControllerBase
    {
        private readonly IExamServiceClient _examServiceClient;
        private readonly IAuditLogService _auditLogService;
        private readonly ILogger<AdminExamController> _logger;

        public AdminExamController(
            IExamServiceClient examServiceClient,
            IAuditLogService auditLogService,
            ILogger<AdminExamController> logger)
        {
            _examServiceClient = examServiceClient;
            _auditLogService = auditLogService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<object>>> GetAllExams()
        {
            var startTime = DateTime.UtcNow;
            var adminId = GetAdminIdFromToken();

            try
            {
                var exams = await _examServiceClient.GetAllExamsAsync();
                var responseTime = (DateTime.UtcNow - startTime).TotalMilliseconds;

                await _auditLogService.LogAuditAsync(
                    adminId, "GET_ALL_EXAMS", "ExamService", 
                    "/api/admin/exams", "GET", null, 
                    System.Text.Json.JsonSerializer.Serialize(exams), 
                    200, GetIpAddress(), GetUserAgent(), (long)responseTime);

                return Ok(new ApiResponseDto<object> { Success = true, Data = exams });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all exams");
                await LogError(adminId, "GET_ALL_EXAMS", "/api/admin/exams", "GET", startTime, ex);
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<object>>> GetExamById(int id)
        {
            var startTime = DateTime.UtcNow;
            var adminId = GetAdminIdFromToken();

            try
            {
                var exam = await _examServiceClient.GetExamByIdAsync(id);
                var responseTime = (DateTime.UtcNow - startTime).TotalMilliseconds;

                await _auditLogService.LogAuditAsync(
                    adminId, "GET_EXAM", "ExamService", 
                    $"/api/admin/exams/{id}", "GET", null, 
                    System.Text.Json.JsonSerializer.Serialize(exam), 
                    200, GetIpAddress(), GetUserAgent(), (long)responseTime);

                if (exam == null)
                    return NotFound(new ApiResponseDto<object> { Success = false, ErrorMessage = "Exam not found" });

                return Ok(new ApiResponseDto<object> { Success = true, Data = exam });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting exam {id}");
                await LogError(adminId, "GET_EXAM", $"/api/admin/exams/{id}", "GET", startTime, ex);
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<object>>> CreateExam([FromBody] object createDto)
        {
            var startTime = DateTime.UtcNow;
            var adminId = GetAdminIdFromToken();

            try
            {
                var requestPayload = System.Text.Json.JsonSerializer.Serialize(createDto);
                var exam = await _examServiceClient.CreateExamAsync(createDto);
                var responseTime = (DateTime.UtcNow - startTime).TotalMilliseconds;

                await _auditLogService.LogAuditAsync(
                    adminId, "CREATE_EXAM", "ExamService", 
                    "/api/admin/exams", "POST", requestPayload, 
                    System.Text.Json.JsonSerializer.Serialize(exam), 
                    201, GetIpAddress(), GetUserAgent(), (long)responseTime);

                if (exam == null)
                    return BadRequest(new ApiResponseDto<object> { Success = false, ErrorMessage = "Failed to create exam" });

                return CreatedAtAction(nameof(GetExamById), new { id = exam.Id }, 
                    new ApiResponseDto<object> { Success = true, Data = exam });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating exam");
                await LogError(adminId, "CREATE_EXAM", "/api/admin/exams", "POST", startTime, ex);
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<object>>> UpdateExam(int id, [FromBody] object updateDto)
        {
            var startTime = DateTime.UtcNow;
            var adminId = GetAdminIdFromToken();

            try
            {
                var requestPayload = System.Text.Json.JsonSerializer.Serialize(updateDto);
                var exam = await _examServiceClient.UpdateExamAsync(id, updateDto);
                var responseTime = (DateTime.UtcNow - startTime).TotalMilliseconds;

                await _auditLogService.LogAuditAsync(
                    adminId, "UPDATE_EXAM", "ExamService", 
                    $"/api/admin/exams/{id}", "PUT", requestPayload, 
                    System.Text.Json.JsonSerializer.Serialize(exam), 
                    200, GetIpAddress(), GetUserAgent(), (long)responseTime);

                if (exam == null)
                    return NotFound(new ApiResponseDto<object> { Success = false, ErrorMessage = "Exam not found" });

                return Ok(new ApiResponseDto<object> { Success = true, Data = exam });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating exam {id}");
                await LogError(adminId, "UPDATE_EXAM", $"/api/admin/exams/{id}", "PUT", startTime, ex);
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteExam(int id)
        {
            var startTime = DateTime.UtcNow;
            var adminId = GetAdminIdFromToken();

            try
            {
                var result = await _examServiceClient.DeleteExamAsync(id);
                var responseTime = (DateTime.UtcNow - startTime).TotalMilliseconds;

                await _auditLogService.LogAuditAsync(
                    adminId, "DELETE_EXAM", "ExamService", 
                    $"/api/admin/exams/{id}", "DELETE", null, 
                    System.Text.Json.JsonSerializer.Serialize(new { success = result }), 
                    result ? 200 : 404, GetIpAddress(), GetUserAgent(), (long)responseTime);

                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting exam {id}");
                await LogError(adminId, "DELETE_EXAM", $"/api/admin/exams/{id}", "DELETE", startTime, ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("{id}/enable-disable")]
        public async Task<ActionResult> EnableDisableExam(int id, [FromBody] bool isActive)
        {
            var startTime = DateTime.UtcNow;
            var adminId = GetAdminIdFromToken();

            try
            {
                var result = await _examServiceClient.EnableDisableExamAsync(id, isActive);
                var responseTime = (DateTime.UtcNow - startTime).TotalMilliseconds;

                await _auditLogService.LogAuditAsync(
                    adminId, "ENABLE_DISABLE_EXAM", "ExamService", 
                    $"/api/admin/exams/{id}/enable-disable", "PATCH", 
                    System.Text.Json.JsonSerializer.Serialize(new { isActive }), 
                    System.Text.Json.JsonSerializer.Serialize(new { success = result }), 
                    result ? 200 : 404, GetIpAddress(), GetUserAgent(), (long)responseTime);

                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error enabling/disabling exam {id}");
                await LogError(adminId, "ENABLE_DISABLE_EXAM", $"/api/admin/exams/{id}/enable-disable", "PATCH", startTime, ex);
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task LogError(int adminId, string action, string endpoint, string method, DateTime startTime, Exception ex)
        {
            await _auditLogService.LogAuditAsync(
                adminId, action, "ExamService", endpoint, method, 
                null, null, 500, GetIpAddress(), GetUserAgent(), 
                (long)(DateTime.UtcNow - startTime).TotalMilliseconds, ex.Message);
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

        private string? GetIpAddress() => HttpContext.Connection.RemoteIpAddress?.ToString();
        private string? GetUserAgent() => Request.Headers["User-Agent"].ToString();
    }
}
