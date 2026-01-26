using ExamService.Application.DTOs;
using ExamService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamService.API.Controllers
{
    /// <summary>
    /// Exam Management API Controller
    /// </summary>
    [Route("api/exams")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;
        private readonly IWebHostEnvironment _environment;

        public ExamController(IExamService examService, IWebHostEnvironment environment)
        {
            _examService = examService;
            _environment = environment;
        }

        /// <summary>
        /// Get all exams or filter by qualification ID, stream ID and/or international status
        /// </summary>
        /// <param name="qualificationId">Optional qualification ID to filter exams</param>
        /// <param name="streamId">Optional stream ID to filter exams</param>
        /// <param name="isInternational">Optional filter for international exams (true/false)</param>
        /// <returns>List of exams</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ExamDto>>> GetExams([FromQuery] int? qualificationId = null, [FromQuery] int? streamId = null, [FromQuery] bool? isInternational = null)
        {
            if (qualificationId.HasValue)
            {
                var exams = await _examService.GetExamsByQualificationAsync(qualificationId.Value, streamId);
                
                // Filter by international status if provided
                if (isInternational.HasValue)
                {
                    exams = exams.Where(e => e.IsInternational == isInternational.Value);
                }
                
                return Ok(exams);
            }
            
            var allExams = await _examService.GetAllExamsAsync(isInternational);
            return Ok(allExams);
        }

        /// <summary>
        /// Get exam by ID
        /// </summary>
        /// <param name="id">Exam ID</param>
        /// <returns>Exam details</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ExamDto>> GetExam(int id)
        {
            var exam = await _examService.GetExamByIdAsync(id);
            if (exam == null)
                return NotFound();

            return Ok(exam);
        }

        /// <summary>
        /// Get exams by qualification ID and optionally by stream ID
        /// </summary>
        /// <param name="qualificationId">Qualification ID</param>
        /// <param name="streamId">Optional stream ID</param>
        /// <returns>List of exams for the qualification and stream</returns>
        [HttpGet("by-qualification/{qualificationId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ExamDto>>> GetExamsByQualification(int qualificationId, [FromQuery] int? streamId = null)
        {
            var exams = await _examService.GetExamsByQualificationAsync(qualificationId, streamId);
            return Ok(exams);
        }

        /// <summary>
        /// Get exams based on user's international exam interest
        /// </summary>
        /// <returns>List of exams filtered by user preference</returns>
        [HttpGet("for-user")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ExamDto>>> GetExamsForUser()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
                var exams = await _examService.GetExamsForUserAsync(userId);
                return Ok(exams);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving exams for user: {ex.Message}");
            }
        }

        /// <summary>
        /// Create a new exam (Admin only)
        /// </summary>
        /// <param name="createDto">Exam creation data</param>
        /// <returns>Created exam details</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ExamDto>> CreateExam(CreateExamDto createDto)
        {
            var exam = await _examService.CreateExamAsync(createDto);
            return CreatedAtAction(nameof(GetExam), new { id = exam.Id }, exam);
        }

        /// <summary>
        /// Update an existing exam (Admin only)
        /// </summary>
        /// <param name="id">Exam ID</param>
        /// <param name="updateDto">Exam update data</param>
        /// <returns>No content</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateExam(int id, UpdateExamDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest("ID in the URL does not match the ID in the request body.");

            var result = await _examService.UpdateExamAsync(id, updateDto);
            if (result == null)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Delete an exam (Admin only)
        /// </summary>
        /// <param name="id">Exam ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteExam(int id)
        {
            var result = await _examService.DeleteExamAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Update exam status (Admin only)
        /// </summary>
        /// <param name="id">Exam ID</param>
        /// <param name="isActive">Active status</param>
        /// <returns>No content</returns>
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateExamStatus(int id, [FromBody] bool isActive)
        {
            var result = await _examService.ToggleExamStatusAsync(id, isActive);
            if (!result)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Upload image for exam (Admin only)
        /// </summary>
        /// <param name="id">Exam ID</param>
        /// <param name="file">Image file to upload</param>
        /// <returns>Image URL</returns>
        [HttpPost("{id}/upload-image")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadExamImage(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var exam = await _examService.GetExamByIdAsync(id);
            if (exam == null)
                return NotFound();

            var uploadsFolder = Path.Combine(_environment.ContentRootPath, "wwwroot", "images", "exams");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var imageUrl = $"/images/exams/{uniqueFileName}";
            
            var updateDto = new UpdateExamDto
            {
                Id = id,
                Name = exam.Name,
                Description = exam.Description,
                DurationInMinutes = exam.DurationInMinutes,
                TotalMarks = exam.TotalMarks,
                PassingMarks = exam.PassingMarks,
                ImageUrl = imageUrl,
                QualificationIds = exam.QualificationIds,
                StreamIds = exam.StreamIds
            };

            var result = await _examService.UpdateExamAsync(id, updateDto);
            if (result == null)
                return BadRequest("Failed to update exam with image URL.");

            return Ok(new { ImageUrl = imageUrl });
        }

        [HttpPost("seed-international-exams")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SeedInternationalExams()
        {
            try
            {
                var createdCount = await _examService.SeedInternationalExamsAsync();
                return Ok(new { message = $"Created {createdCount} international exams" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error seeding international exams: {ex.Message}");
            }
        }
    }
}
