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
    [Route("api/mock-tests")]
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
            return AuthClaimsHelper.GetUserId(User);
        }

        #region Admin Endpoints

        /// <summary>
        /// Create a new mock test with image upload support
        /// </summary>
        [HttpPost("admin/mock-tests")]
        [Authorize(Roles = "Admin")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<object>> CreateMockTest([FromForm] MockTestCreateWithImageDto dto)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                dto.CreatedBy = userId;
                var mockTest = await _mockTestService.CreateMockTestWithImageAsync(dto);
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
        /// Save mock test as draft
        /// </summary>
        [HttpPost("admin/mock-tests/draft")]
        [Authorize(Roles = "Admin")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<object>> SaveMockTestDraft([FromForm] MockTestCreateWithImageDto dto)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                dto.CreatedBy = userId;
                var mockTest = await _mockTestService.CreateMockTestDraftAsync(dto);
                return Ok(new { success = true, data = mockTest, message = "Mock test saved as draft successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving mock test draft");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Create a new mock test (JSON body)
        /// </summary>
        [HttpPost("admin/mock-tests")]
        [Authorize(Roles = "Admin")]
        [Consumes("application/json")]
        public async Task<ActionResult<object>> CreateMockTestJson([FromBody] CreateMockTestDto dto)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                dto.CreatedBy = userId;
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
        /// Get comprehensive list of mock tests with filtering and pagination (Admin View)
        /// </summary>
        [HttpPost("admin/mock-tests/list")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> GetMockTestsList([FromBody] MockTestListRequestDto request)
        {
            try
            {
                var result = await _mockTestService.GetMockTestsListAsync(request);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock tests list");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get mock test summary statistics for admin dashboard
        /// </summary>
        [HttpGet("admin/mock-tests/summary")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> GetMockTestSummary()
        {
            try
            {
                var summary = await _mockTestService.GetMockTestSummaryAsync();
                return Ok(new { success = true, data = summary });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock test summary");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Create a new mock test with exam integration (Legacy)
        /// </summary>
        [HttpPost("admin/mock-tests/legacy")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> CreateMockTestLegacy([FromBody] CreateMockTestDto dto)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                dto.CreatedBy = userId;
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
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
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
        /// Get comprehensive mock test statistics
        /// </summary>
        [HttpGet("admin/mock-tests/statistics")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> GetMockTestStatistics(
            [FromQuery] int? examId = null,
            [FromQuery] int? subjectId = null)
        {
            try
            {
                var request = new MockTestStatisticsRequestDto
                {
                    ExamId = examId,
                    SubjectId = subjectId
                };
                var statistics = await _mockTestService.GetMockTestStatisticsAsync(request);
                return Ok(new { success = true, data = statistics, message = "Mock test statistics fetched successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock test statistics");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get mock test statistics (Legacy endpoint)
        /// </summary>
        [HttpGet("admin/mock-tests/statistics/legacy")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> GetMockTestStatisticsLegacy(
            [FromQuery] int? examId = null,
            [FromQuery] int? subjectId = null)
        {
            try
            {
                var statistics = await _mockTestService.GetMockTestStatisticsLegacyAsync(examId, subjectId);
                return Ok(new { success = true, data = statistics });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock test statistics (legacy)");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get mock test statistics (Public endpoint)
        /// </summary>
        [HttpGet("mock-tests/statistics")]
        [Authorize]
        public async Task<ActionResult<object>> GetMockTestStatisticsPublic(
            [FromQuery] int? examId = null,
            [FromQuery] int? subjectId = null)
        {
            try
            {
                var request = new MockTestStatisticsRequestDto
                {
                    ExamId = examId,
                    SubjectId = subjectId
                };
                var statistics = await _mockTestService.GetMockTestStatisticsAsync(request);
                return Ok(new { success = true, data = statistics, message = "Mock test statistics fetched successfully" });
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
        /// Get subject-wise mock tests for user (Better UX)
        /// </summary>
        [HttpPost("mock-tests/subject-wise")]
        public async Task<ActionResult<object>> GetSubjectWiseMockTests([FromBody] MockTestListRequestDto request)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                var result = await _mockTestService.GetSubjectWiseMockTestsAsync(userId, request);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subject-wise mock tests for user");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get mock test filter options for user
        /// </summary>
        [HttpGet("mock-tests/filter-options")]
        public async Task<ActionResult<object>> GetMockTestFilterOptions()
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                var filterOptions = await _mockTestService.GetMockTestFilterOptionsAsync(userId);
                return Ok(new { success = true, data = filterOptions });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock test filter options");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get comprehensive list of mock tests for user with filtering and pagination
        /// </summary>
        [HttpPost("mock-tests/list")]
        public async Task<ActionResult<object>> GetMockTestsForUserList([FromBody] MockTestListRequestDto request)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                var result = await _mockTestService.GetMockTestsForUserListAsync(userId, request);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock tests for user");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get all mock tests for user with optional filters
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<object>> GetMockTests(
            [FromQuery] int? examId = null,
            [FromQuery] int? subjectId = null,
            [FromQuery] string? testCategory = null,
            [FromQuery] string? access = null,
            [FromQuery] string? difficulty = null)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                var mockTests = await _mockTestService.GetMockTestsForUserAsync(userId, examId, subjectId, testCategory, access, difficulty);
                return Ok(new { success = true, data = mockTests });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock tests for user");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get mock tests for specific exam
        /// </summary>
        [HttpGet("exams/{examId}")]
        public async Task<ActionResult<object>> GetMockTestsForExam(int examId)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                var mockTests = await _mockTestService.GetMockTestsForExamAsync(userId, examId);
                return Ok(new { success = true, data = mockTests });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock tests for exam {ExamId}", examId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get mock tests for specific subject
        /// </summary>
        [HttpGet("subjects/{subjectId}")]
        public async Task<ActionResult<object>> GetMockTestsForSubject(int subjectId)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                var mockTests = await _mockTestService.GetMockTestsForSubjectSimpleAsync(userId, subjectId);
                return Ok(new { success = true, data = mockTests });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock tests for subject {SubjectId}", subjectId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get specific mock test details
        /// </summary>
        [HttpGet("{mockTestId:int}")]
        public async Task<ActionResult<object>> GetMockTestDetails(int mockTestId)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                var mockTest = await _mockTestService.GetMockTestDetailsSimpleAsync(userId, mockTestId);
                if (mockTest == null)
                    return NotFound(new { success = false, message = "Mock test not found" });

                var attemptsUsed = mockTest.AttemptsUsed;
                var attemptsRemaining = Math.Max(0, mockTest.AttemptsAllowed - attemptsUsed);
                var bestScore = mockTest.Attempts.Count == 0 ? 0m : mockTest.Attempts.Max(x => x.ObtainedMarks);

                var responseData = new
                {
                    mockTestId = mockTest.Id,
                    name = mockTest.Name,
                    description = mockTest.Description,
                    examName = mockTest.ExamName,
                    subjectName = mockTest.SubjectName ?? string.Empty,
                    totalQuestions = mockTest.TotalQuestions,
                    totalMarks = mockTest.TotalMarks,
                    passingMarks = mockTest.PassingMarks,
                    durationInMinutes = mockTest.DurationInMinutes,
                    difficulty = string.IsNullOrWhiteSpace(mockTest.Difficulty) ? "Medium" : mockTest.Difficulty,
                    language = "Hindi, English",
                    access = mockTest.AccessType,
                    price = string.Equals(mockTest.AccessType, "Free", StringComparison.OrdinalIgnoreCase) ? 0m : 0m,
                    instructions = new[]
                    {
                        "Read each question carefully before answering",
                        "Test will be automatically submitted when time expires"
                    },
                    attemptsUsed,
                    attemptsRemaining,
                    isUnlocked = mockTest.IsUnlocked,
                    bestScore,
                    userRank = 0,
                    totalParticipants = 0
                };

                return Ok(new { success = true, data = responseData });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock test {MockTestId} details", mockTestId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Start a mock test session
        /// </summary>
        [HttpPost("{mockTestId:int}/start")]
        public async Task<ActionResult<object>> StartMockTest(
            int mockTestId,
            [FromBody] StartMockTestLanguageRequestDto? request = null)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                var dto = new StartMockTestDto
                {
                    MockTestId = mockTestId,
                    UserId = userId,
                    LanguageCode = string.IsNullOrWhiteSpace(request?.LanguageCode) ? "en" : request.LanguageCode
                };

                var session = await _mockTestService.StartMockTestAsync(dto);
                return Ok(new { success = true, data = session });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting mock test {MockTestId}", mockTestId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Save/update a single question answer for an in-progress mock test session
        /// </summary>
        [HttpPost("{sessionId:int}/answer")]
        public async Task<ActionResult<object>> SaveMockTestAnswer(int sessionId, [FromBody] SaveMockTestAnswerDto request)
        {
            try
            {
                if (sessionId <= 0)
                    return BadRequest(new { success = false, message = "Invalid sessionId. Start mock test first and pass returned sessionId." });

                if (request == null || request.QuestionId <= 0)
                    return BadRequest(new { success = false, message = "Invalid questionId. Pass a valid questionId from session questions list." });

                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                var saved = await _mockTestService.SaveMockTestAnswerAsync(sessionId, userId, request);
                if (!saved)
                    return NotFound(new { success = false, message = "Session not found or already completed" });

                return Ok(new { success = true, message = "Answer saved" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving answer for session {SessionId}", sessionId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Submit mock test from already saved answers
        /// </summary>
        [HttpPost("{sessionId:int}/submit")]
        public async Task<ActionResult<object>> SubmitMockTestSession(int sessionId, [FromBody] SubmitMockTestSessionDto? _ = null)
        {
            try
            {
                if (sessionId <= 0)
                    return BadRequest(new { success = false, message = "Invalid sessionId. Start mock test first and pass returned sessionId." });

                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                var result = await _mockTestService.SubmitMockTestAsync(sessionId, userId);
                return Ok(new { success = true, data = result, message = "Mock test submitted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting mock test session {SessionId}", sessionId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get mock tests for user with subscription validation (Legacy)
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
        public async Task<ActionResult<object>> StartMockTest([FromBody] StartMockTestRequestDto request)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                var dto = new StartMockTestDto
                {
                    MockTestId = request.MockTestId,
                    UserId = userId,
                    LanguageCode = string.IsNullOrWhiteSpace(request.LanguageCode) ? "en" : request.LanguageCode
                };
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
        public int QuestionNumber { get; set; } = 0;
        public decimal Marks { get; set; }
        public decimal NegativeMarks { get; set; }
    }

    public class SubmitMockTestDto
    {
        public int SessionId { get; set; }
        public List<QuizAnswerRequestDto> Answers { get; set; } = new();
    }

    public class StartMockTestRequestDto
    {
        public int MockTestId { get; set; }
        public string LanguageCode { get; set; } = "en";
    }

    public class StartMockTestLanguageRequestDto
    {
        public string? LanguageCode { get; set; }
    }
}
