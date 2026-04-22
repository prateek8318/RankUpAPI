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
    //[Authorize(Roles = "Admin")] // Temporarily disabled for testing
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
        /// Create a new exam with detailed configuration
        /// </summary>
        /// <param name="createDto">Detailed exam creation data</param>
        /// <returns>Created exam details</returns>
        [HttpPost("create")]
        public async Task<ActionResult<ExamDto>> CreateDetailedExam([FromForm] CreateExamDto createDto)
        {
            try
            {
                // Handle image upload
                string? imageUrl = null;
                if (createDto.ImageFile != null)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "exams");
                    Directory.CreateDirectory(uploadsFolder);
                    
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(createDto.ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await createDto.ImageFile.CopyToAsync(stream);
                    }
                    
                    imageUrl = "/images/exams/" + uniqueFileName;
                }
                
                var exam = await _examService.CreateExamAsync(createDto, imageUrl);
                return Ok(new
                {
                    success = true,
                    data = exam,
                    message = "Exam created successfully"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating exam: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { success = false, message = $"Error creating exam: {ex.Message}" });
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
                // Set the ID from URL to updateDto
                updateDto.Id = id;
                
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
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error updating exam" });
            }
        }

        /// <summary>
        /// Get all exams for admin (including drafts and inactive)
        /// </summary>
        /// <param name="isInternational">Optional filter for international exams (true/false)</param>
        /// <param name="language">Optional language code (e.g., 'en', 'hi'). Defaults to header value or 'en'</param>
        /// <returns>List of all exams including drafts</returns>
        [HttpGet("all")]
        public async Task<ActionResult<object>> GetAllExamsForAdmin(
            [FromQuery] bool? isInternational = null,
            [FromQuery] string? language = null)
        {
            try
            {
                var currentLanguage = language ?? _languageService.GetCurrentLanguage();
                var exams = await _examService.GetAllExamsForAdminAsync(isInternational);
                return Ok(new
                {
                    success = true,
                    data = exams,
                    language = currentLanguage,
                    message = "All exams fetched successfully (including drafts)"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error fetching all exams" });
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

        /// <summary>
        /// Hard delete multiple exams except specified IDs
        /// </summary>
        /// <returns>Delete result</returns>
        [HttpDelete("bulk-hard-delete")]
        public async Task<ActionResult<object>> BulkHardDeleteExams()
        {
            try
            {
                var excludedIds = new[] { 63, 64, 65, 66, 67 };
                var deletedCount = await _examService.BulkHardDeleteExamsAsync(excludedIds);
                
                return Ok(new
                {
                    success = true,
                    message = $"Hard deleted {deletedCount} exams successfully",
                    data = new { deletedCount, excludedIds }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error bulk deleting exams" });
            }
        }
    }
}
