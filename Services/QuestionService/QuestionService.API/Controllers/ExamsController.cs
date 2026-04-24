using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionService.Application.DTOs;
using QuestionService.Application.Services;
using QuestionService.Application.Interfaces;
using QuestionService.API.Helpers;
using System.Security.Claims;

namespace QuestionService.API.Controllers
{
    [ApiController]
    [Route("api/exams")]
    [Authorize]
    public class ExamsController : ControllerBase
    {
        private readonly IMockTestService _mockTestService;
        private readonly ILogger<ExamsController> _logger;

        public ExamsController(IMockTestService mockTestService, ILogger<ExamsController> logger)
        {
            _mockTestService = mockTestService;
            _logger = logger;
        }

        private int GetAuthenticatedUserId()
        {
            return AuthClaimsHelper.GetUserId(User);
        }

        #region User Endpoints

        /// <summary>
        /// Get all available exams for user
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<object>> GetExamsForUser()
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                var exams = await _mockTestService.GetExamsForUserAsync(userId);
                return Ok(new { success = true, data = exams });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exams for user");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get specific exam details
        /// </summary>
        [HttpGet("{examId:int}")]
        public async Task<ActionResult<object>> GetExamDetails(int examId)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                var exam = await _mockTestService.GetExamDetailsAsync(userId, examId);
                return Ok(new { success = true, data = exam });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exam {ExamId} details", examId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get subjects for specific exam
        /// </summary>
        [HttpGet("{examId:int}/subjects")]
        public async Task<ActionResult<object>> GetSubjectsForExam(int examId)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                var subjects = await _mockTestService.GetSubjectsForExamAsync(userId, examId);
                return Ok(new { success = true, data = subjects });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subjects for exam {ExamId}", examId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get specific subject details under exam
        /// </summary>
        [HttpGet("{examId:int}/subjects/{subjectId:int}")]
        public async Task<ActionResult<object>> GetSubjectDetails(int examId, int subjectId)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                var subject = await _mockTestService.GetSubjectDetailsAsync(userId, examId, subjectId);
                return Ok(new { success = true, data = subject });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subject {SubjectId} details for exam {ExamId}", subjectId, examId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        #endregion

        #region Admin Endpoints

        /// <summary>
        /// Create a new exam (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> CreateExam([FromBody] CreateExamDto dto)
        {
            try
            {
                var exam = await _mockTestService.CreateExamAsync(dto);
                return Ok(new { success = true, data = exam, message = "Exam created successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating exam");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Update exam details (Admin only)
        /// </summary>
        [HttpPut("{examId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> UpdateExam(int examId, [FromBody] UpdateExamDto dto)
        {
            try
            {
                var exam = await _mockTestService.UpdateExamAsync(examId, dto);
                return Ok(new { success = true, data = exam, message = "Exam updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating exam {ExamId}", examId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Delete exam (Admin only)
        /// </summary>
        [HttpDelete("{examId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> DeleteExam(int examId)
        {
            try
            {
                var result = await _mockTestService.DeleteExamAsync(examId);
                return Ok(new { success = true, data = result, message = "Exam deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting exam {ExamId}", examId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        #endregion
    }
}
