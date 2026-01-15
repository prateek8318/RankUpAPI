using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdminService.Application.Interfaces;
using AdminService.Application.DTOs;

namespace AdminService.API.Controllers
{
    /// <summary>
    /// Admin Quiz Management Controller
    /// Orchestrates calls to QuizService
    /// </summary>
    [Route("api/admin/quizzes")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminQuizController : ControllerBase
    {
        private readonly IQuizServiceClient _quizServiceClient;
        private readonly ILogger<AdminQuizController> _logger;

        public AdminQuizController(
            IQuizServiceClient quizServiceClient,
            ILogger<AdminQuizController> logger)
        {
            _quizServiceClient = quizServiceClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<object>>> GetAllQuizzes()
        {
            try
            {
                var quizzes = await _quizServiceClient.GetAllQuizzesAsync();
                return Ok(new ApiResponseDto<object> { Success = true, Data = quizzes });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all quizzes");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<object>>> GetQuizById(int id)
        {
            try
            {
                var quiz = await _quizServiceClient.GetQuizByIdAsync(id);
                if (quiz == null)
                    return NotFound(new ApiResponseDto<object> { Success = false, ErrorMessage = "Quiz not found" });

                return Ok(new ApiResponseDto<object> { Success = true, Data = quiz });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting quiz {id}");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<object>>> CreateQuiz([FromBody] object createDto)
        {
            try
            {
                var quiz = await _quizServiceClient.CreateQuizAsync(createDto);
                if (quiz == null)
                    return BadRequest(new ApiResponseDto<object> { Success = false, ErrorMessage = "Failed to create quiz" });

                return CreatedAtAction(nameof(GetQuizById), new { id = ((dynamic)quiz).Id }, 
                    new ApiResponseDto<object> { Success = true, Data = quiz });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating quiz");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<object>>> UpdateQuiz(int id, [FromBody] object updateDto)
        {
            try
            {
                var quiz = await _quizServiceClient.UpdateQuizAsync(id, updateDto);
                if (quiz == null)
                    return NotFound(new ApiResponseDto<object> { Success = false, ErrorMessage = "Quiz not found" });

                return Ok(new ApiResponseDto<object> { Success = true, Data = quiz });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating quiz {id}");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteQuiz(int id)
        {
            try
            {
                var result = await _quizServiceClient.DeleteQuizAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting quiz {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
