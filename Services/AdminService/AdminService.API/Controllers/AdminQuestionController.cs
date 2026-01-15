using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdminService.Application.Interfaces;
using AdminService.Application.DTOs;

namespace AdminService.API.Controllers
{
    /// <summary>
    /// Admin Question Bank Management Controller - FR-ADM-04
    /// Orchestrates calls to QuestionService
    /// </summary>
    [Route("api/admin/questions")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminQuestionController : ControllerBase
    {
        private readonly IQuestionServiceClient _questionServiceClient;
        private readonly IAuditLogService _auditLogService;
        private readonly ILogger<AdminQuestionController> _logger;

        public AdminQuestionController(
            IQuestionServiceClient questionServiceClient,
            IAuditLogService auditLogService,
            ILogger<AdminQuestionController> logger)
        {
            _questionServiceClient = questionServiceClient;
            _auditLogService = auditLogService;
            _logger = logger;
        }

        [HttpGet("quiz/{quizId}")]
        public async Task<ActionResult<ApiResponseDto<object>>> GetQuestionsByQuizId(int quizId)
        {
            try
            {
                var questions = await _questionServiceClient.GetQuestionsByQuizIdAsync(quizId);
                return Ok(new ApiResponseDto<object> { Success = true, Data = questions });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting questions for quiz {quizId}");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<object>>> GetQuestionById(int id)
        {
            try
            {
                var question = await _questionServiceClient.GetQuestionByIdAsync(id);
                if (question == null)
                    return NotFound(new ApiResponseDto<object> { Success = false, ErrorMessage = "Question not found" });

                return Ok(new ApiResponseDto<object> { Success = true, Data = question });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting question {id}");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<object>>> CreateQuestion([FromBody] object createDto)
        {
            try
            {
                var question = await _questionServiceClient.CreateQuestionAsync(createDto);
                if (question == null)
                    return BadRequest(new ApiResponseDto<object> { Success = false, ErrorMessage = "Failed to create question" });

                return CreatedAtAction(nameof(GetQuestionById), new { id = ((dynamic)question).Id }, 
                    new ApiResponseDto<object> { Success = true, Data = question });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating question");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<object>>> UpdateQuestion(int id, [FromBody] object updateDto)
        {
            try
            {
                var question = await _questionServiceClient.UpdateQuestionAsync(id, updateDto);
                if (question == null)
                    return NotFound(new ApiResponseDto<object> { Success = false, ErrorMessage = "Question not found" });

                return Ok(new ApiResponseDto<object> { Success = true, Data = question });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating question {id}");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteQuestion(int id)
        {
            try
            {
                var result = await _questionServiceClient.DeleteQuestionAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting question {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("bulk-upload/{quizId}")]
        public async Task<ActionResult<ApiResponseDto<object>>> BulkUploadQuestions(int quizId, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new ApiResponseDto<object> { Success = false, ErrorMessage = "File is required" });

                using var stream = file.OpenReadStream();
                var result = await _questionServiceClient.BulkUploadQuestionsAsync(quizId, stream, file.FileName);
                return Ok(new ApiResponseDto<object> { Success = true, Data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error bulk uploading questions for quiz {quizId}");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }
    }
}
