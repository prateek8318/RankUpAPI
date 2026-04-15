using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TestService.Application.DTOs;
using TestService.Application.Services;
using TestService.API.Models;
using TestServiceAppService = TestService.Application.Services.TestService;

namespace TestService.API.Controllers
{
    [Route("api/tests")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly TestServiceAppService _testService;

        public TestController(TestServiceAppService testService)
        {
            _testService = testService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<TestDto>>> GetTests([FromQuery] TestFilterDto filter)
        {
            try
            {
                var tests = await _testService.GetTestsAsync(filter);
                return Ok(tests);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("user/available")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserTestListResponseDto>>> GetAvailableTestsForUser(
            [FromQuery] int examId,
            [FromQuery] int? practiceModeId = null,
            [FromQuery] int? subjectId = null,
            [FromQuery] int? seriesId = null,
            [FromQuery] int? year = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var userId = GetUserIdOrThrow();
            var result = await _testService.GetAvailableTestsForUserAsync(new UserTestListRequestDto
            {
                UserId = userId,
                ExamId = examId,
                PracticeModeId = practiceModeId,
                SubjectId = subjectId,
                SeriesId = seriesId,
                Year = year,
                PageNumber = pageNumber,
                PageSize = pageSize
            });

            return Ok(new ApiResponse<UserTestListResponseDto> { Success = true, Message = "Success", Data = result });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<TestDto>> GetById(int id)
        {
            var test = await _testService.GetByIdAsync(id);
            if (test == null)
                return NotFound();
            return Ok(test);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TestDto>> Create(CreateTestDto dto)
        {
            try
            {
                var test = await _testService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = test.Id }, test);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("map-plan")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> MapPlan([FromBody] MapTestToPlanDto dto)
        {
            var result = await _testService.MapTestToPlanAsync(dto);
            if (!result)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Unable to map test to plan.", Data = null });
            }

            return Ok(new ApiResponse<object> { Success = true, Message = "Success", Data = null });
        }

        [HttpGet("{testId:int}/leaderboard")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<LeaderboardEntryDto>>>> GetLeaderboard(int testId, [FromQuery] int top = 20)
        {
            var rows = await _testService.GetLeaderboardAsync(testId, top);
            return Ok(new ApiResponse<IReadOnlyList<LeaderboardEntryDto>> { Success = true, Message = "Success", Data = rows });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateTestDto dto)
        {
            try
            {
                var result = await _testService.UpdateAsync(id, dto);
                if (result == null)
                    return NotFound();
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _testService.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpPost("bulk-upload/{seriesId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TestBulkUploadResultDto>> BulkUpload(int seriesId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            try
            {
                using var stream = file.OpenReadStream();
                var result = await _testService.BulkUploadTestsAsync(seriesId, stream, file.FileName, 1); // TODO: Get userId from claims
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
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
