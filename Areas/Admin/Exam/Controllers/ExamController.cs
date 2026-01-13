using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RankUpAPI.Areas.Admin.Exam.DTOs;
using RankUpAPI.Areas.Admin.Exam.Services.Interfaces;

namespace RankUpAPI.Areas.Admin.Exam.Controllers
{
    [Area("Admin")]
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamController(IExamService examService)
        {
            _examService = examService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ExamDto>>> GetExams()
        {
            var exams = await _examService.GetAllExamsAsync();
            return Ok(exams);
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
        [HttpGet("qualification/{qualificationId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ExamDto>>> GetExamsByQualification(int qualificationId)
        {
            var exams = await _examService.GetExamsByQualificationAsync(qualificationId);
            return Ok(exams);
        }

        [HttpPost]
        public async Task<ActionResult<ExamDto>> CreateExam(CreateExamDto createDto)
        {
            var exam = await _examService.CreateExamAsync(createDto);
            return CreatedAtAction(nameof(GetExam), new { id = exam.Id }, exam);
        }

        [HttpPut("{id}")]
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
        public async Task<IActionResult> DeleteExam(int id)
        {
            var result = await _examService.DeleteExamAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateExamStatus(int id, [FromBody] bool isActive)
        {
            var result = await _examService.ToggleExamStatusAsync(id, isActive);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
