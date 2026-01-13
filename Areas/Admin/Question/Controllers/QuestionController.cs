using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RankUpAPI.Areas.Admin.Question.DTOs;
using RankUpAPI.Areas.Admin.Question.Services.Interfaces;

namespace RankUpAPI.Areas.Admin.Question.Controllers
{
    [Area("Admin")]
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestions(
            [FromQuery] int? chapterId,
            [FromQuery] int? subjectId,
            [FromQuery] int? examId)
        {
            if (chapterId.HasValue)
            {
                var questions = await _questionService.GetQuestionsByChapterIdAsync(chapterId.Value);
                return Ok(questions);
            }

            if (subjectId.HasValue)
            {
                var questions = await _questionService.GetQuestionsBySubjectIdAsync(subjectId.Value);
                return Ok(questions);
            }

            if (examId.HasValue)
            {
                var questions = await _questionService.GetQuestionsByExamIdAsync(examId.Value);
                return Ok(questions);
            }

            var allQuestions = await _questionService.GetAllQuestionsAsync();
            return Ok(allQuestions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionDto>> GetQuestion(int id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null)
                return NotFound();

            return Ok(question);
        }

        [HttpPost]
        public async Task<ActionResult<QuestionDto>> CreateQuestion(CreateQuestionDto createDto)
        {
            var question = await _questionService.CreateQuestionAsync(createDto);
            return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, question);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(int id, UpdateQuestionDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest("ID in the URL does not match the ID in the request body.");

            var result = await _questionService.UpdateQuestionAsync(id, updateDto);
            if (result == null)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var result = await _questionService.DeleteQuestionAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateQuestionStatus(int id, [FromBody] bool isActive)
        {
            var result = await _questionService.ToggleQuestionStatusAsync(id, isActive);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPost("bulk-upload")]
        public async Task<ActionResult<BulkUploadQuestionDto>> BulkUploadQuestions(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var result = await _questionService.BulkUploadQuestionsAsync(file);
            return Ok(result);
        }
    }
}
