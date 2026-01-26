using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestService.Application.DTOs;
using TestService.Application.Services;

namespace TestService.API.Controllers
{
    [Route("api/testseries")]
    [ApiController]
    public class TestSeriesController : ControllerBase
    {
        private readonly TestSeriesService _service;

        public TestSeriesController(TestSeriesService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<TestSeriesDto>>> GetAll()
        {
            var testSeries = await _service.GetAllAsync();
            return Ok(testSeries);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<TestSeriesDto>> GetById(int id)
        {
            var testSeries = await _service.GetByIdAsync(id);
            if (testSeries == null)
                return NotFound();
            return Ok(testSeries);
        }

        [HttpGet("exam/{examId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<TestSeriesDto>>> GetByExam(int examId)
        {
            var testSeries = await _service.GetByExamIdAsync(examId);
            return Ok(testSeries);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TestSeriesDto>> Create(CreateTestSeriesDto dto)
        {
            try
            {
                var testSeries = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = testSeries.Id }, testSeries);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateTestSeriesDto dto)
        {
            try
            {
                var result = await _service.UpdateAsync(id, dto);
                if (result == null)
                    return NotFound();
                return NoContent();
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
            var result = await _service.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
