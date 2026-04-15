using ExamService.Application.DTOs;
using ExamService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Services;

namespace ExamService.API.Controllers
{
    /// <summary>
    /// Admin Exam Management API Controller
    /// </summary>
    [Route("api/admin/exams")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IExamService _examService;
        private readonly ILanguageService _languageService;

        public AdminController(IExamService examService, ILanguageService languageService)
        {
            _examService = examService;
            _languageService = languageService;
        }

        /// <summary>
        /// Get exam statistics for admin dashboard
        /// </summary>
        /// <returns>Exam statistics summary</returns>
        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetExamStats()
        {
            try
            {
                var stats = await _examService.GetExamStatsAsync();
                return Ok(new
                {
                    success = true,
                    data = stats,
                    message = "Exam statistics fetched successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error fetching exam statistics" });
            }
        }

        /// <summary>
        /// Get all exam categories
        /// </summary>
        /// <param name="language">Optional language code (e.g., 'en', 'hi'). Defaults to header value or 'en'</param>
        /// <returns>List of exam categories</returns>
        [HttpGet("categories")]
        public async Task<ActionResult<object>> GetExamCategories([FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var categories = await _examService.GetExamCategoriesAsync();
                return Ok(new
                {
                    success = true,
                    data = categories,
                    language = currentLanguage,
                    message = "Exam categories fetched successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error fetching exam categories" });
            }
        }

        /// <summary>
        /// Get exam types by category ID
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <param name="language">Optional language code (e.g., 'en', 'hi'). Defaults to header value or 'en'</param>
        /// <returns>List of exam types for the category</returns>
        [HttpGet("categories/{categoryId}/types")]
        public async Task<ActionResult<object>> GetExamTypesByCategory(int categoryId, [FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var types = await _examService.GetExamTypesByCategoryAsync(categoryId);
                return Ok(new
                {
                    success = true,
                    data = types,
                    language = currentLanguage,
                    message = "Exam types fetched successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error fetching exam types" });
            }
        }

        /// <summary>
        /// Create a new exam with detailed configuration
        /// </summary>
        /// <param name="createDto">Detailed exam creation data</param>
        /// <returns>Created exam details</returns>
        [HttpPost("create")]
        public async Task<ActionResult<ExamDto>> CreateDetailedExam(CreateExamDto createDto)
        {
            try
            {
                var exam = await _examService.CreateExamAsync(createDto);
                return Ok(new
                {
                    success = true,
                    data = exam,
                    message = "Exam created successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error creating exam" });
            }
        }

        /// <summary>
        /// Update exam configuration
        /// </summary>
        /// <param name="id">Exam ID</param>
        /// <param name="updateDto">Exam update data</param>
        /// <returns>Updated exam details</returns>
        [HttpPut("{id}/update")]
        public async Task<ActionResult<ExamDto>> UpdateExamConfiguration(int id, UpdateExamDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                    return BadRequest(new { success = false, message = "ID in URL does not match ID in request body" });

                var exam = await _examService.UpdateExamAsync(id, updateDto);
                if (exam == null)
                    return NotFound(new { success = false, message = "Exam not found" });

                return Ok(new
                {
                    success = true,
                    data = exam,
                    message = "Exam updated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error updating exam" });
            }
        }

        /// <summary>
        /// Get exams by category and type
        /// </summary>
        /// <param name="categoryId">Category ID (optional)</param>
        /// <param name="typeId">Type ID (optional)</param>
        /// <param name="status">Status filter (optional)</param>
        /// <param name="language">Optional language code (e.g., 'en', 'hi'). Defaults to header value or 'en'</param>
        /// <returns>Filtered list of exams</returns>
        [HttpGet("filtered")]
        public async Task<ActionResult<object>> GetFilteredExams(
            [FromQuery] int? categoryId = null,
            [FromQuery] int? typeId = null,
            [FromQuery] string? status = null,
            [FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var exams = await _examService.GetFilteredExamsAsync(categoryId, typeId, status);
                return Ok(new
                {
                    success = true,
                    data = exams,
                    language = currentLanguage,
                    message = "Filtered exams fetched successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error fetching filtered exams" });
            }
        }

        /// <summary>
        /// Update exam status (Draft, Active, Scheduled, Completed)
        /// </summary>
        /// <param name="id">Exam ID</param>
        /// <param name="status">New status</param>
        /// <returns>Updated exam status</returns>
        [HttpPatch("{id}/status")]
        public async Task<ActionResult<object>> UpdateExamStatus(int id, [FromBody] string status)
        {
            try
            {
                var result = await _examService.UpdateExamStatusAsync(id, status);
                if (!result)
                    return NotFound(new { success = false, message = "Exam not found" });

                return Ok(new
                {
                    success = true,
                    message = $"Exam status updated to {status}",
                    data = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error updating exam status" });
            }
        }

        /// <summary>
        /// Get exam management data for admin dashboard
        /// </summary>
        /// <param name="language">Optional language code (e.g., 'en', 'hi'). Defaults to header value or 'en'</param>
        /// <returns>Complete exam management data</returns>
        [HttpGet("dashboard")]
        public async Task<ActionResult<object>> GetExamDashboard([FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var dashboard = await _examService.GetExamDashboardAsync();
                return Ok(new
                {
                    success = true,
                    data = dashboard,
                    language = currentLanguage,
                    message = "Exam dashboard data fetched successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error fetching exam dashboard data" });
            }
        }

        /// <summary>
        /// Delete an exam (soft delete)
        /// </summary>
        /// <param name="id">Exam ID</param>
        /// <returns>Delete result</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> DeleteExam(int id)
        {
            try
            {
                var result = await _examService.DeleteExamAsync(id);
                if (!result)
                    return NotFound(new { success = false, message = "Exam not found" });

                return Ok(new
                {
                    success = true,
                    message = "Exam deleted successfully",
                    data = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error deleting exam" });
            }
        }
    }
}
