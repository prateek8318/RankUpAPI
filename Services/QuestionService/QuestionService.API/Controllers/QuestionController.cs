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
            var questions = await _service.GetByTopicIdAsync(chapterId);
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

        [HttpPost("admin/topics")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> CreateTopic([FromBody] CreateTopicDto dto)
        {
            var id = await _service.CreateTopicAsync(dto);
            return Ok(new { success = true, topicId = id });
        }

        [HttpGet("admin/topics")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<TopicDto>>> GetTopics([FromQuery] int? subjectId = null, [FromQuery] int? examId = null, [FromQuery] bool includeInactive = true)
        {
            var result = await _service.GetTopicsAsync(subjectId ?? 0);
            return Ok(new { success = true, data = result });
        }

        [HttpPost("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> CreateAdminQuestion([FromBody] CreateQuestionAdminDto dto)
        {
            var question = await _service.CreateAdminQuestionAsync(dto);
            return Ok(new { success = true, questionId = question.Id });
        }

        [HttpPut("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> UpdateAdminQuestion(int id, [FromBody] UpdateQuestionAdminDto dto)
        {
            var updated = await _service.UpdateAdminQuestionAsync(dto);
            return Ok(new { success = true, data = updated });
        }

        [HttpGet("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<QuestionAdminDetailDto>> GetAdminQuestionById(int id)
        {
            var result = await _service.GetAdminQuestionByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { success = false, message = "Question not found." });
            }

            return Ok(new { success = true, data = result });
        }

        [HttpGet("admin/paged")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<QuestionPagedResponseDto>> GetAdminQuestionsPaged([FromQuery] QuestionFilterRequestDto filter)
        {
            var result = await _service.GetAdminQuestionsPagedAsync(filter);
            return Ok(new { success = true, data = result });
        }

        [HttpPatch("admin/{id}/publish")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> SetPublishStatus(int id, [FromBody] PublishQuestionRequestDto dto)
        {
            var updated = await _service.SetPublishStatusAsync(dto);
            if (!updated)
            {
                return NotFound(new { success = false, message = "Question not found." });
            }

            return Ok(new { success = true, message = "Publish status updated." });
        }

        [HttpGet("admin/dashboard/stats")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<QuestionDashboardStatsDto>> GetDashboardStats()
        {
            var result = await _service.GetDashboardStatsAsync();
            return Ok(new { success = true, data = result });
        }

        [HttpPost("admin/bulk")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> BulkUpload([FromBody] BulkQuestionUploadRequestDto request)
        {
            var count = await _service.BulkCreateQuestionsAsync(request);
            return Ok(new { success = true, insertedCount = count });
        }
    }
}
