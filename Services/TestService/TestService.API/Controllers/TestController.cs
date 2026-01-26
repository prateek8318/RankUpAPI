using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestService.Application.DTOs;
using TestService.Application.Services;
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
    }
}
