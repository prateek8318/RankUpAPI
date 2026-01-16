using ExamService.Application.DTOs;
using ExamService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamService.API.Controllers
{
    [Route("api/exams")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamController(IExamService examService)
        {
            _examService = examService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ExamDto>>> GetExams([FromQuery] int? qualificationId = null)
        {
            if (qualificationId.HasValue)
            {
                var exams = await _examService.GetExamsByQualificationAsync(qualificationId.Value);
                return Ok(exams);
            }
            
            var allExams = await _examService.GetAllExamsAsync();
            return Ok(allExams);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ExamDto>> GetExam(int id)
        {
            var exam = await _examService.GetExamByIdAsync(id);
            if (exam == null)
                return NotFound();

            return Ok(exam);
        }

        [HttpGet("by-qualification/{qualificationId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ExamDto>>> GetExamsByQualification(int qualificationId)
        {
            var exams = await _examService.GetExamsByQualificationAsync(qualificationId);
            return Ok(exams);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ExamDto>> CreateExam(CreateExamDto createDto)
        {
            var exam = await _examService.CreateExamAsync(createDto);
            return CreatedAtAction(nameof(GetExam), new { id = exam.Id }, exam);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateExam(int id, UpdateExamDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest("ID in the URL does not match the ID in the request body.");

            var result = await _examService.UpdateExamAsync(id, updateDto);
            if (result == null)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteExam(int id)
        {
            var result = await _examService.DeleteExamAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateExamStatus(int id, [FromBody] bool isActive)
        {
            var result = await _examService.ToggleExamStatusAsync(id, isActive);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
