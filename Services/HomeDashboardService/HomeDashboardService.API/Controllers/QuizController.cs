using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HomeDashboardService.Application.DTOs;
using HomeDashboardService.Application.Interfaces;

namespace HomeDashboardService.API.Controllers
{
    /// <summary>
    /// Quiz Management Controller (Admin)
    /// </summary>
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly ILogger<QuizController> _logger;

        public QuizController(
            IQuizService quizService,
            ILogger<QuizController> logger)
        {
            _quizService = quizService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<QuizDto>> CreateQuiz([FromBody] CreateQuizDto createQuizDto)
        {
            try
            {
                var result = await _quizService.CreateQuizAsync(createQuizDto);
                return CreatedAtAction(nameof(GetQuizById), new { id = result.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating quiz");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<QuizDto>> UpdateQuiz(int id, [FromBody] UpdateQuizDto updateQuizDto)
        {
            try
            {
                var result = await _quizService.UpdateQuizAsync(id, updateQuizDto);
                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quiz: {QuizId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteQuiz(int id)
        {
            try
            {
                var result = await _quizService.DeleteQuizAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting quiz: {QuizId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("{id}/enable-disable")]
        public async Task<ActionResult> EnableDisableQuiz(int id, [FromBody] bool isActive)
        {
            try
            {
                var result = await _quizService.EnableDisableQuizAsync(id, isActive);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling/disabling quiz: {QuizId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuizDto>> GetQuizById(int id)
        {
            try
            {
                var result = await _quizService.GetQuizByIdAsync(id);
                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quiz: {QuizId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("chapter/{chapterId}")]
        public async Task<ActionResult<IEnumerable<QuizDto>>> GetQuizzesByChapterId(int chapterId)
        {
            try
            {
                var result = await _quizService.GetQuizzesByChapterIdAsync(chapterId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quizzes for chapter: {ChapterId}", chapterId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
