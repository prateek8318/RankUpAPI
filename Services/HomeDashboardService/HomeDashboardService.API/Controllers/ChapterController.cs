using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HomeDashboardService.Application.DTOs;
using HomeDashboardService.Application.Interfaces;

namespace HomeDashboardService.API.Controllers
{
    /// <summary>
    /// Chapter Management Controller (Admin)
    /// </summary>
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ChapterController : ControllerBase
    {
        private readonly IChapterService _chapterService;
        private readonly ILogger<ChapterController> _logger;

        public ChapterController(
            IChapterService chapterService,
            ILogger<ChapterController> logger)
        {
            _chapterService = chapterService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<ChapterDto>> CreateChapter([FromBody] CreateChapterDto createChapterDto)
        {
            try
            {
                var result = await _chapterService.CreateChapterAsync(createChapterDto);
                return CreatedAtAction(nameof(GetChapterById), new { id = result.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating chapter");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ChapterDto>> UpdateChapter(int id, [FromBody] UpdateChapterDto updateChapterDto)
        {
            try
            {
                var result = await _chapterService.UpdateChapterAsync(id, updateChapterDto);
                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating chapter: {ChapterId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteChapter(int id)
        {
            try
            {
                var result = await _chapterService.DeleteChapterAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting chapter: {ChapterId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("{id}/enable-disable")]
        public async Task<ActionResult> EnableDisableChapter(int id, [FromBody] bool isActive)
        {
            try
            {
                var result = await _chapterService.EnableDisableChapterAsync(id, isActive);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling/disabling chapter: {ChapterId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ChapterDto>> GetChapterById(int id)
        {
            try
            {
                var result = await _chapterService.GetChapterByIdAsync(id);
                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting chapter: {ChapterId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("subject/{subjectId}")]
        public async Task<ActionResult<IEnumerable<ChapterDto>>> GetChaptersBySubjectId(int subjectId)
        {
            try
            {
                var result = await _chapterService.GetChaptersBySubjectIdAsync(subjectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting chapters for subject: {SubjectId}", subjectId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
