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
    public class ChapterController : ControllerBase
    {
        private readonly IChapterService _chapterService;

        public ChapterController(IChapterService chapterService)
        {
            _chapterService = chapterService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChapterDto>>> GetChapters([FromQuery] int? subjectId)
        {
            if (subjectId.HasValue)
            {
                var chapters = await _chapterService.GetChaptersBySubjectIdAsync(subjectId.Value);
                return Ok(chapters);
            }

            var allChapters = await _chapterService.GetAllChaptersAsync();
            return Ok(allChapters);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ChapterDto>> GetChapter(int id)
        {
            var chapter = await _chapterService.GetChapterByIdAsync(id);
            if (chapter == null)
                return NotFound();

            return Ok(chapter);
        }

        [HttpPost]
        public async Task<ActionResult<ChapterDto>> CreateChapter(CreateChapterDto createDto)
        {
            var chapter = await _chapterService.CreateChapterAsync(createDto);
            return CreatedAtAction(nameof(GetChapter), new { id = chapter.Id }, chapter);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateChapter(int id, UpdateChapterDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest("ID in the URL does not match the ID in the request body.");

            var result = await _chapterService.UpdateChapterAsync(id, updateDto);
            if (result == null)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChapter(int id)
        {
            var result = await _chapterService.DeleteChapterAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateChapterStatus(int id, [FromBody] bool isActive)
        {
            var result = await _chapterService.ToggleChapterStatusAsync(id, isActive);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
