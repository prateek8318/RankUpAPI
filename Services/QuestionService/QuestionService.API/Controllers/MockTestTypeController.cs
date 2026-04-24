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
    [Authorize]
    [Route("api/[controller]")]
    public class MockTestTypeController : ControllerBase
    {
        private readonly IMockTestService _mockTestService;
        private readonly ILogger<MockTestTypeController> _logger;

        public MockTestTypeController(IMockTestService mockTestService, ILogger<MockTestTypeController> logger)
        {
            _mockTestService = mockTestService;
            _logger = logger;
        }

        private int GetAuthenticatedUserId()
        {
            return AuthClaimsHelper.GetUserId(User);
        }

        #region Mock Tests - Subject Wise Questions

        /// <summary>
        /// Get Mock Tests (Subject Wise Questions)
        /// </summary>
        [HttpGet("mock-tests")]
        public async Task<ActionResult<object>> GetMockTests(
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
                
                // Filter only MockTest type
                var mockTests = result.MockTests.Where(mt => mt.MockTestType == MockTestType.MockTest).ToList();
                
                return Ok(new { 
                    success = true, 
                    data = new MockTestListResponseDto
                    {
                        MockTests = mockTests,
                        TotalCount = mockTests.Count,
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalPages = (int)Math.Ceiling((double)mockTests.Count / pageSize),
                        UserSubscription = result.UserSubscription
                    },
                    message = "Mock tests retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock tests");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Create Mock Test (Subject Wise)
        /// </summary>
        [HttpPost("admin/mock-tests")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> CreateMockTest([FromBody] CreateMockTestDto dto)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                // Validate for MockTest type
                dto.MockTestType = MockTestType.MockTest;
                dto.CreatedBy = userId;
                if (!dto.SubjectId.HasValue)
                {
                    return BadRequest(new { success = false, message = "SubjectId is required for Mock Test" });
                }

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

        #endregion

        #region Test Series - Full Length Papers

        /// <summary>
        /// Get Test Series (Full Length Papers)
        /// </summary>
        [HttpGet("test-series")]
        public async Task<ActionResult<object>> GetTestSeries(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? examId = null)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                var result = await _mockTestService.GetMockTestsForUserAsync(userId, pageNumber, pageSize, examId);
                
                // Filter only TestSeries type
                var testSeries = result.MockTests.Where(mt => mt.MockTestType == MockTestType.TestSeries).ToList();
                
                return Ok(new { 
                    success = true, 
                    data = new MockTestListResponseDto
                    {
                        MockTests = testSeries,
                        TotalCount = testSeries.Count,
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalPages = (int)Math.Ceiling((double)testSeries.Count / pageSize),
                        UserSubscription = result.UserSubscription
                    },
                    message = "Test series retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving test series");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Create Test Series (Full Length Paper)
        /// </summary>
        [HttpPost("admin/test-series")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> CreateTestSeries([FromBody] CreateMockTestDto dto)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                // Validate for TestSeries type
                dto.MockTestType = MockTestType.TestSeries;
                dto.CreatedBy = userId;
                dto.SubjectId = null; // Full length papers don't require specific subject
                dto.TopicId = null;
                
                if (dto.DurationInMinutes < 60)
                {
                    return BadRequest(new { success = false, message = "Full length papers should be at least 60 minutes" });
                }

                var testSeries = await _mockTestService.CreateMockTestAsync(dto);
                return Ok(new { success = true, data = testSeries, message = "Test series created successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating test series");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        #endregion

        #region Deep Practice - Topic Wise MCQs

        /// <summary>
        /// Get Deep Practice (Topic Wise MCQs)
        /// </summary>
        [HttpGet("deep-practice")]
        public async Task<ActionResult<object>> GetDeepPractice(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? examId = null,
            [FromQuery] int? subjectId = null,
            [FromQuery] int? topicId = null,
            [FromQuery] string? difficulty = null)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                var result = await _mockTestService.GetMockTestsForUserAsync(userId, pageNumber, pageSize, examId, subjectId);
                
                // Filter only DeepPractice type and additional criteria
                var deepPractice = result.MockTests.Where(mt => 
                    mt.MockTestType == MockTestType.DeepPractice &&
                    (!subjectId.HasValue || mt.SubjectId == subjectId) &&
                    (!topicId.HasValue || mt.TopicId == topicId) &&
                    (string.IsNullOrEmpty(difficulty) || mt.Difficulty == difficulty)
                ).ToList();
                
                return Ok(new { 
                    success = true, 
                    data = new MockTestListResponseDto
                    {
                        MockTests = deepPractice,
                        TotalCount = deepPractice.Count,
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalPages = (int)Math.Ceiling((double)deepPractice.Count / pageSize),
                        UserSubscription = result.UserSubscription
                    },
                    message = "Deep practice tests retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving deep practice tests");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Create Deep Practice (Topic Wise MCQs)
        /// </summary>
        [HttpPost("admin/deep-practice")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> CreateDeepPractice([FromBody] CreateMockTestDto dto)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                // Validate for DeepPractice type
                dto.MockTestType = MockTestType.DeepPractice;
                dto.CreatedBy = userId;
                
                if (!dto.SubjectId.HasValue)
                {
                    return BadRequest(new { success = false, message = "SubjectId is required for Deep Practice" });
                }
                
                if (!dto.TopicId.HasValue)
                {
                    return BadRequest(new { success = false, message = "TopicId is required for Deep Practice" });
                }
                
                if (string.IsNullOrEmpty(dto.Difficulty))
                {
                    return BadRequest(new { success = false, message = "Difficulty is required for Deep Practice" });
                }
                
                if (!new[] { "Easy", "Medium", "Hard" }.Contains(dto.Difficulty))
                {
                    return BadRequest(new { success = false, message = "Difficulty must be Easy, Medium, or Hard" });
                }

                var deepPractice = await _mockTestService.CreateMockTestAsync(dto);
                return Ok(new { success = true, data = deepPractice, message = "Deep practice created successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating deep practice");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        #endregion

        #region Previous Years Solved Papers

        /// <summary>
        /// Get Previous Years Solved Papers
        /// </summary>
        [HttpGet("previous-years")]
        public async Task<ActionResult<object>> GetPreviousYears(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? examId = null,
            [FromQuery] int? year = null)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                var result = await _mockTestService.GetMockTestsForUserAsync(userId, pageNumber, pageSize, examId);
                
                // Filter only PreviousYear type and year filter
                var previousYears = result.MockTests.Where(mt => 
                    mt.MockTestType == MockTestType.PreviousYear &&
                    (!year.HasValue || mt.Year == year)
                ).ToList();
                
                return Ok(new { 
                    success = true, 
                    data = new MockTestListResponseDto
                    {
                        MockTests = previousYears,
                        TotalCount = previousYears.Count,
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalPages = (int)Math.Ceiling((double)previousYears.Count / pageSize),
                        UserSubscription = result.UserSubscription
                    },
                    message = "Previous year papers retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving previous year papers");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Create Previous Year Solved Paper
        /// </summary>
        [HttpPost("admin/previous-years")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> CreatePreviousYear([FromBody] CreateMockTestDto dto)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                // Validate for PreviousYear type
                dto.MockTestType = MockTestType.PreviousYear;
                dto.CreatedBy = userId;
                dto.SubjectId = null; // Previous year papers may cover multiple subjects
                dto.TopicId = null;
                
                if (!dto.Year.HasValue)
                {
                    return BadRequest(new { success = false, message = "Year is required for Previous Year papers" });
                }
                
                if (dto.Year < 2000 || dto.Year > DateTime.Now.Year)
                {
                    return BadRequest(new { success = false, message = "Year must be between 2000 and current year" });
                }

                var previousYear = await _mockTestService.CreateMockTestAsync(dto);
                return Ok(new { success = true, data = previousYear, message = "Previous year paper created successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating previous year paper");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        #endregion

        #region Combined Endpoints

        /// <summary>
        /// Get all mock test types with counts
        /// </summary>
        [HttpGet("overview")]
        public async Task<ActionResult<object>> GetMockTestOverview([FromQuery] int? examId = null)
        {
            try
            {
                var userId = GetAuthenticatedUserId();
                if (userId <= 0)
                    return Unauthorized(new { success = false, message = "Invalid user" });

                var result = await _mockTestService.GetMockTestsForUserAsync(userId, 1, 1000, examId); // Get all for overview
                
                var overview = new
                {
                    MockTests = new
                    {
                        Count = result.MockTests.Count(mt => mt.MockTestType == MockTestType.MockTest),
                        Items = result.MockTests.Where(mt => mt.MockTestType == MockTestType.MockTest).Take(5).ToList()
                    },
                    TestSeries = new
                    {
                        Count = result.MockTests.Count(mt => mt.MockTestType == MockTestType.TestSeries),
                        Items = result.MockTests.Where(mt => mt.MockTestType == MockTestType.TestSeries).Take(5).ToList()
                    },
                    DeepPractice = new
                    {
                        Count = result.MockTests.Count(mt => mt.MockTestType == MockTestType.DeepPractice),
                        Items = result.MockTests.Where(mt => mt.MockTestType == MockTestType.DeepPractice).Take(5).ToList()
                    },
                    PreviousYears = new
                    {
                        Count = result.MockTests.Count(mt => mt.MockTestType == MockTestType.PreviousYear),
                        Items = result.MockTests.Where(mt => mt.MockTestType == MockTestType.PreviousYear).Take(5).ToList()
                    },
                    UserSubscription = result.UserSubscription
                };

                return Ok(new { success = true, data = overview, message = "Mock test overview retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock test overview");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        #endregion
    }
}
