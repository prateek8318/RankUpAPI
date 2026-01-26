using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestService.Application.DTOs;
using TestService.Application.Services;

namespace TestService.API.Controllers
{
    [Route("api/test-execution")]
    [ApiController]
    [Authorize]
    public class TestExecutionController : ControllerBase
    {
        private readonly TestExecutionService _testExecutionService;

        public TestExecutionController(TestExecutionService testExecutionService)
        {
            _testExecutionService = testExecutionService;
        }

        [HttpPost("start/{testId}")]
        public async Task<ActionResult<UserTestAttemptDto>> StartTest(int testId)
        {
            try
            {
                // TODO: Get userId from claims
                var userId = 1; // Placeholder
                var attempt = await _testExecutionService.StartTestAsync(testId, userId);
                return Ok(attempt);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("save-answer/{attemptId}")]
        public async Task<IActionResult> SaveAnswer(int attemptId, [FromBody] SaveAnswerRequestDto request)
        {
            try
            {
                // TODO: Get userId from claims and validate ownership
                var userId = 1; // Placeholder
                await _testExecutionService.SaveAnswerAsync(attemptId, request.QuestionId, request.Answer, userId);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("submit/{attemptId}")]
        public async Task<ActionResult<TestResultDto>> SubmitTest(int attemptId)
        {
            try
            {
                // TODO: Get userId from claims and validate ownership
                var userId = 1; // Placeholder
                var result = await _testExecutionService.SubmitTestAsync(attemptId, userId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("attempt/{attemptId}")]
        public async Task<ActionResult<UserTestAttemptDto>> GetAttempt(int attemptId)
        {
            try
            {
                // TODO: Get userId from claims and validate ownership
                var userId = 1; // Placeholder
                var attempt = await _testExecutionService.GetAttemptAsync(attemptId, userId);
                return Ok(attempt);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpGet("attempt/{attemptId}/questions")]
        public async Task<ActionResult<TestQuestionListDto>> GetTestQuestions(int attemptId)
        {
            try
            {
                // TODO: Get userId from claims and validate ownership
                var userId = 1; // Placeholder
                var questions = await _testExecutionService.GetTestQuestionsAsync(attemptId, userId);
                return Ok(questions);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }
    }

    public class SaveAnswerRequestDto
    {
        public int QuestionId { get; set; }
        public string? Answer { get; set; }
    }
}
