using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RankUpAPI.DTOs;
using RankUpAPI.Services.Interfaces;

namespace RankUpAPI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class TestSeriesController : ControllerBase
    {
        private readonly ITestSeriesService _testSeriesService;

        public TestSeriesController(ITestSeriesService testSeriesService)
        {
            _testSeriesService = testSeriesService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TestSeriesDto>>> GetTestSeries([FromQuery] int? examId)
        {
            if (examId.HasValue)
            {
                var testSeries = await _testSeriesService.GetTestSeriesByExamIdAsync(examId.Value);
                return Ok(testSeries);
            }

            var allTestSeries = await _testSeriesService.GetAllTestSeriesAsync();
            return Ok(allTestSeries);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TestSeriesDto>> GetTestSeries(int id)
        {
            var testSeries = await _testSeriesService.GetTestSeriesByIdAsync(id);
            if (testSeries == null)
                return NotFound();

            return Ok(testSeries);
        }

        [HttpPost]
        public async Task<ActionResult<TestSeriesDto>> CreateTestSeries(CreateTestSeriesDto createDto)
        {
            var testSeries = await _testSeriesService.CreateTestSeriesAsync(createDto);
            return CreatedAtAction(nameof(GetTestSeries), new { id = testSeries.Id }, testSeries);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTestSeries(int id, UpdateTestSeriesDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest("ID in the URL does not match the ID in the request body.");

            var result = await _testSeriesService.UpdateTestSeriesAsync(id, updateDto);
            if (result == null)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTestSeries(int id)
        {
            var result = await _testSeriesService.DeleteTestSeriesAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateTestSeriesStatus(int id, [FromBody] bool isActive)
        {
            var result = await _testSeriesService.ToggleTestSeriesStatusAsync(id, isActive);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPost("add-questions")]
        public async Task<IActionResult> AddQuestionsToTestSeries(AddQuestionsToTestSeriesDto dto)
        {
            try
            {
                var result = await _testSeriesService.AddQuestionsToTestSeriesAsync(dto);
                if (!result)
                    return BadRequest("Failed to add questions to test series");

                return Ok(new { message = "Questions added successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding questions", error = ex.Message });
            }
        }

        [HttpDelete("{testSeriesId}/questions/{questionId}")]
        public async Task<IActionResult> RemoveQuestionFromTestSeries(int testSeriesId, int questionId)
        {
            var result = await _testSeriesService.RemoveQuestionFromTestSeriesAsync(testSeriesId, questionId);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
