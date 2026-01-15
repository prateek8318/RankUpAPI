using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HomeDashboardService.Application.DTOs;
using HomeDashboardService.Application.Interfaces;

namespace HomeDashboardService.API.Controllers
{
    /// <summary>
    /// Exam Management Controller (Admin)
    /// </summary>
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;
        private readonly ILogger<ExamController> _logger;

        public ExamController(
            IExamService examService,
            ILogger<ExamController> logger)
        {
            _examService = examService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new exam
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ExamDto>> CreateExam([FromBody] CreateExamDto createExamDto)
        {
            try
            {
                var result = await _examService.CreateExamAsync(createExamDto);
                return CreatedAtAction(nameof(GetExamById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating exam");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update an existing exam
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ExamDto>> UpdateExam(int id, [FromBody] UpdateExamDto updateExamDto)
        {
            try
            {
                var result = await _examService.UpdateExamAsync(id, updateExamDto);
                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating exam: {ExamId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete an exam
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteExam(int id)
        {
            try
            {
                var result = await _examService.DeleteExamAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting exam: {ExamId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Enable or disable an exam
        /// </summary>
        [HttpPatch("{id}/enable-disable")]
        public async Task<ActionResult> EnableDisableExam(int id, [FromBody] bool isActive)
        {
            try
            {
                var result = await _examService.EnableDisableExamAsync(id, isActive);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling/disabling exam: {ExamId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get exam by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamDto>> GetExamById(int id)
        {
            try
            {
                var result = await _examService.GetExamByIdAsync(id);
                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting exam: {ExamId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get all exams
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamDto>>> GetAllExams()
        {
            try
            {
                var result = await _examService.GetAllExamsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all exams");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get active exams
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<ExamDto>>> GetActiveExams()
        {
            try
            {
                var result = await _examService.GetActiveExamsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active exams");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
