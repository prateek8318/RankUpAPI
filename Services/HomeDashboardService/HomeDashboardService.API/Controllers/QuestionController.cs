using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HomeDashboardService.Application.DTOs;
using HomeDashboardService.Application.Interfaces;

namespace HomeDashboardService.API.Controllers
{
    /// <summary>
    /// Question Management Controller (Admin)
    /// </summary>
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        private readonly ILogger<QuestionController> _logger;

        public QuestionController(
            IQuestionService questionService,
            ILogger<QuestionController> logger)
        {
            _questionService = questionService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<QuestionDto>> CreateQuestion([FromBody] CreateQuestionDto createQuestionDto)
        {
            try
            {
                var result = await _questionService.CreateQuestionAsync(createQuestionDto);
                return CreatedAtAction(nameof(GetQuestionById), new { id = result.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating question");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<QuestionDto>> UpdateQuestion(int id, [FromBody] UpdateQuestionDto updateQuestionDto)
        {
            try
            {
                var result = await _questionService.UpdateQuestionAsync(id, updateQuestionDto);
                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating question: {QuestionId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteQuestion(int id)
        {
            try
            {
                var result = await _questionService.DeleteQuestionAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting question: {QuestionId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("{id}/enable-disable")]
        public async Task<ActionResult> EnableDisableQuestion(int id, [FromBody] bool isActive)
        {
            try
            {
                var result = await _questionService.EnableDisableQuestionAsync(id, isActive);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling/disabling question: {QuestionId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionDto>> GetQuestionById(int id)
        {
            try
            {
                var result = await _questionService.GetQuestionByIdAsync(id);
                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting question: {QuestionId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("quiz/{quizId}")]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestionsByQuizId(int quizId)
        {
            try
            {
                var result = await _questionService.GetQuestionsByQuizIdAsync(quizId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting questions for quiz: {QuizId}", quizId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("bulk-upload/{quizId}")]
        public async Task<ActionResult<BulkUploadResultDto>> BulkUploadQuestions(int quizId, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("File is required");

                if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase) &&
                    !file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                    return BadRequest("Only CSV and Excel files are supported");

                var userId = GetUserIdFromToken();
                if (userId == 0)
                    return Unauthorized("Invalid user token");

                using var stream = file.OpenReadStream();
                var result = await _questionService.BulkUploadQuestionsAsync(quizId, stream, file.FileName, userId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk uploading questions for quiz: {QuizId}", quizId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("bulk-upload/{logId}")]
        public async Task<ActionResult<BulkUploadLogDto>> GetBulkUploadLog(int logId)
        {
            try
            {
                var result = await _questionService.GetBulkUploadLogByIdAsync(logId);
                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bulk upload log: {LogId}", logId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("bulk-upload/{logId}/errors")]
        public async Task<ActionResult<List<BulkUploadErrorDto>>> GetBulkUploadErrors(int logId)
        {
            try
            {
                var result = await _questionService.GetBulkUploadErrorsAsync(logId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bulk upload errors: {LogId}", logId);
                return StatusCode(500, "Internal server error");
            }
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst("UserId") ?? User.FindFirst("sub");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }
    }
}
