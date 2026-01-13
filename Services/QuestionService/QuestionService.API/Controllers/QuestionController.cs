using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionService.Application.DTOs;
using QuestionService.Application.Services;
using QuestionApplicationService = QuestionService.Application.Services.QuestionService;

namespace QuestionService.API.Controllers
{
    [Route("api/questions")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly QuestionApplicationService _service;

        public QuestionController(QuestionApplicationService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetAll()
        {
            var questions = await _service.GetAllAsync();
            return Ok(questions);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<QuestionDto>> GetById(int id)
        {
            var question = await _service.GetByIdAsync(id);
            if (question == null)
                return NotFound();
            return Ok(question);
        }

        [HttpGet("chapter/{chapterId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetByChapter(int chapterId)
        {
            var questions = await _service.GetByChapterIdAsync(chapterId);
            return Ok(questions);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<QuestionDto>> Create(CreateQuestionDto dto)
        {
            var question = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = question.Id }, question);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateQuestionDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            var result = await _service.UpdateAsync(id, dto);
            if (result == null)
                return NotFound();

            return NoContent();
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
