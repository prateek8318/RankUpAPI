using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TestService.API.Models;
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
        public async Task<ActionResult<ApiResponse<UserTestAttemptDto>>> StartTest(int testId)
        {
            try
            {
                var userId = GetUserIdOrThrow();
                var attempt = await _testExecutionService.StartTestAsync(testId, userId);
                return Ok(new ApiResponse<UserTestAttemptDto> { Success = true, Message = "Success", Data = attempt });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<UserTestAttemptDto> { Success = false, Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<UserTestAttemptDto> { Success = false, Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<UserTestAttemptDto> { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("save-answer/{attemptId}")]
        public async Task<ActionResult<ApiResponse<SaveAnswerActionResultDto>>> SaveAnswer(int attemptId, [FromBody] SaveAnswerActionRequestDto request)
        {
            try
            {
                var userId = GetUserIdOrThrow();
                var result = await _testExecutionService.SaveAnswerActionAsync(attemptId, request, userId);
                return Ok(new ApiResponse<SaveAnswerActionResultDto> { Success = true, Message = "Success", Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<SaveAnswerActionResultDto> { Success = false, Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<SaveAnswerActionResultDto> { Success = false, Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<SaveAnswerActionResultDto> { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("submit/{attemptId}")]
        public async Task<ActionResult<ApiResponse<TestResultDto>>> SubmitTest(int attemptId)
        {
            try
            {
                var userId = GetUserIdOrThrow();
                var result = await _testExecutionService.SubmitTestAsync(attemptId, userId);
                return Ok(new ApiResponse<TestResultDto> { Success = true, Message = "Success", Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<TestResultDto> { Success = false, Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<TestResultDto> { Success = false, Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse<TestResultDto> { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("attempt/{attemptId}")]
        public async Task<ActionResult<ApiResponse<UserTestAttemptDto>>> GetAttempt(int attemptId)
        {
            try
            {
                var userId = GetUserIdOrThrow();
                var attempt = await _testExecutionService.GetAttemptAsync(attemptId, userId);
                return Ok(new ApiResponse<UserTestAttemptDto> { Success = true, Message = "Success", Data = attempt });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<UserTestAttemptDto> { Success = false, Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new ApiResponse<UserTestAttemptDto> { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("attempt/{attemptId}/questions")]
        public async Task<ActionResult<ApiResponse<TestQuestionListDto>>> GetTestQuestions(int attemptId)
        {
            try
            {
                var userId = GetUserIdOrThrow();
                var questions = await _testExecutionService.GetTestQuestionsAsync(attemptId, userId);
                return Ok(new ApiResponse<TestQuestionListDto> { Success = true, Message = "Success", Data = questions });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<TestQuestionListDto> { Success = false, Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new ApiResponse<TestQuestionListDto> { Success = false, Message = ex.Message });
            }
        }

        private int GetUserIdOrThrow()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("sub")?.Value
                              ?? User.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out var userId) || userId <= 0)
            {
                throw new UnauthorizedAccessException("Valid user claim not found in token.");
            }

            return userId;
        }
    }
}
