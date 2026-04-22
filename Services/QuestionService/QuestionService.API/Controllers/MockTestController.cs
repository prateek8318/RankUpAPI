using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionService.Application.DTOs;
using QuestionService.Application.Services;
using QuestionService.Application.Interfaces;
using System.Security.Claims;

namespace QuestionService.API.Controllers
{
    [ApiController]
    [Authorize]
    public class MockTestController : ControllerBase
    {
        private readonly IMockTestService _mockTestService;
        private readonly ILogger<MockTestController> _logger;

        public MockTestController(IMockTestService mockTestService, ILogger<MockTestController> logger)
        {
            _mockTestService = mockTestService;
            _logger = logger;
        }

        private int GetAuthenticatedUserId()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdValue) || !int.TryParse(userIdValue, out var userId))
                return 0;

            return userId;
        }

        #region Admin Endpoints

        /// <summary>
        /// Create a new mock test with exam integration
        /// </summary>
        [HttpPost("admin/mock-tests")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> CreateMockTest([FromBody] CreateMockTestDto dto)
        {
            try
            {
                var mockTest = await _mockTestService.CreateMockTestAsync(dto);
                return Ok(new { success = true, data = mockTest, message = "Mock test created successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating mock test");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Update existing mock test
        /// </summary>
        [HttpPut("admin/mock-tests/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> UpdateMockTest(int id, [FromBody] UpdateMockTestDto dto)
        {
            try
            {
                dto.Id = id;
                var mockTest = await _mockTestService.UpdateMockTestAsync(dto);
                return Ok(new { success = true, data = mockTest, message = "Mock test updated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating mock test: {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Delete mock test (soft delete)
        /// </summary>
        [HttpDelete("admin/mock-tests/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> DeleteMockTest(int id)
        {
            try
            {
                var result = await _mockTestService.DeleteMockTestAsync(id);
                if (!result)
                    return NotFound(new { success = false, message = "Mock test not found" });

                return Ok(new { success = true, message = "Mock test deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting mock test: {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get mock test by ID (admin view)
        /// </summary>
        [HttpGet("admin/mock-tests/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> GetMockTestById(int id)
        {
            try
            {
                var mockTest = await _mockTestService.GetMockTestByIdAsync(id);
                if (mockTest == null)
                    return NotFound(new { success = false, message = "Mock test not found" });

                return Ok(new { success = true, data = mockTest });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock test: {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get all mock tests with pagination and filters (admin view)
        /// </summary>
        [HttpGet("admin/mock-tests")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> GetMockTests(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? examId = null,
            [FromQuery] int? subjectId = null,
            [FromQuery] bool? isActive = null)
        {
            try
            {
                var result = await _mockTestService.GetMockTestsAsync(pageNumber, pageSize, examId, subjectId, isActive);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock tests");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Add question to mock test
        /// </summary>
        [HttpPost("admin/mock-tests/{mockTestId}/questions")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> AddQuestionToMockTest(
            int mockTestId,
            [FromBody] AddQuestionToMockTestDto dto)
        {
            try
            {
                var result = await _mockTestService.AddQuestionToMockTestAsync(
                    mockTestId, dto.QuestionId, dto.QuestionNumber, dto.Marks, dto.NegativeMarks);
                
                return Ok(new { success = true, message = "Question added to mock test successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding question to mock test: {MockTestId}", mockTestId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Remove question from mock test
        /// </summary>
        [HttpDelete("admin/mock-tests/{mockTestId}/questions/{questionId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> RemoveQuestionFromMockTest(int mockTestId, int questionId)
        {
            try
            {
                var result = await _mockTestService.RemoveQuestionFromMockTestAsync(mockTestId, questionId);
                
                return Ok(new { success = true, message = "Question removed from mock test successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing question from mock test: {MockTestId}", mockTestId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get mock test questions
        /// </summary>
        [HttpGet("admin/mock-tests/{mockTestId}/questions")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> GetMockTestQuestions(int mockTestId)
        {
            try
            {
                var questions = await _mockTestService.GetMockTestQuestionsAsync(mockTestId);
                return Ok(new { success = true, data = questions });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock test questions: {MockTestId}", mockTestId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get mock test statistics
        /// </summary>
        [HttpGet("admin/mock-tests/statistics")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> GetMockTestStatistics(
            [FromQuery] int? examId = null,
            [FromQuery] int? subjectId = null)
        {
            try
            {
                var statistics = await _mockTestService.GetMockTestStatisticsAsync(examId, subjectId);
                return Ok(new { success = true, data = statistics });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock test statistics");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        #endregion

        #region User Endpoints

        /// <summary>
        /// Get mock tests for user with subscription validation
        /// </summary>
        [HttpGet("mock-tests")]
        public async Task<ActionResult<object>> GetMockTestsForUser(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? examId = null,
            [FromQuery] int? subjectId = null)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                var result = await _mockTestService.GetMockTestsForUserAsync(userId, pageNumber, pageSize, examId, subjectId);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock tests for user");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get mock test detail for user with subscription check
        /// </summary>
        [HttpGet("mock-tests/{mockTestId}")]
        public async Task<ActionResult<object>> GetMockTestDetailForUser(int mockTestId)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                var mockTest = await _mockTestService.GetMockTestDetailForUserAsync(userId, mockTestId);
                if (mockTest == null)
                    return NotFound(new { success = false, message = "Mock test not found" });

                return Ok(new { success = true, data = mockTest });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock test detail for user: {MockTestId}", mockTestId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Check if user can access mock test (subscription validation)
        /// </summary>
        [HttpGet("mock-tests/{mockTestId}/access")]
        public async Task<ActionResult<object>> CheckMockTestAccess(int mockTestId)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                var accessCheck = await _mockTestService.CheckMockTestAccessAsync(userId, mockTestId);
                return Ok(new { success = true, data = accessCheck });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking mock test access: {MockTestId}", mockTestId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Start mock test session
        /// </summary>
        [HttpPost("mock-tests/start")]
        public async Task<ActionResult<object>> StartMockTest([FromBody] StartMockTestDto dto)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                dto.UserId = userId;
                var session = await _mockTestService.StartMockTestAsync(dto);
                return Ok(new { success = true, data = session, message = "Mock test started successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting mock test");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get mock test session
        /// </summary>
        [HttpGet("mock-tests/sessions/{sessionId}")]
        public async Task<ActionResult<object>> GetMockTestSession(int sessionId)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                var session = await _mockTestService.GetMockTestSessionAsync(sessionId, userId);
                if (session == null)
                    return NotFound(new { success = false, message = "Session not found" });

                return Ok(new { success = true, data = session });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock test session: {SessionId}", sessionId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Submit mock test
        /// </summary>
        [HttpPost("mock-tests/submit")]
        public async Task<ActionResult<object>> SubmitMockTest([FromBody] SubmitMockTestDto dto)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                var result = await _mockTestService.SubmitMockTestAsync(dto.SessionId, userId, dto.Answers);
                return Ok(new { success = true, data = result, message = "Mock test submitted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting mock test");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get user's mock test performance
        /// </summary>
        [HttpGet("mock-tests/performance")]
        public async Task<ActionResult<object>> GetUserPerformance([FromQuery] int? examId = null)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                var performance = await _mockTestService.GetMockTestPerformanceAsync(userId, examId);
                return Ok(new { success = true, data = performance });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user performance");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        #endregion
    }

    // Additional DTOs for endpoints
    public class AddQuestionToMockTestDto
    {
        public int QuestionId { get; set; }
        public int QuestionNumber { get; set; }
        public decimal Marks { get; set; }
        public decimal NegativeMarks { get; set; }
    }

    public class SubmitMockTestDto
    {
        public int SessionId { get; set; }
        public List<QuizAnswerRequestDto> Answers { get; set; } = new();
    }
}
