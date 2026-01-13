using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RankUpAPI.Areas.Admin.HomeContent.DTOs;
using RankUpAPI.Areas.Admin.HomeContent.Services.Interfaces;
using RankUpAPI.Models;

namespace RankUpAPI.Areas.Admin.HomeContent.Controllers
{
    [ApiController]
    [Area("Admin")]
    [Route("api/admin/home-content")]
    [Authorize(Roles = "Admin")]
    public class HomeContentController : ControllerBase
    {
        private readonly IHomeContentService _homeContentService;

        public HomeContentController(IHomeContentService homeContentService)
        {
            _homeContentService = homeContentService;
        }

        /// <summary>
        /// Create a single home card for any section (Mock Test, Test Series, etc.).
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<HomeSectionItemDto>> Create([FromBody] HomeSectionItemCreateDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _homeContentService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetBySection), new { sectionType = created.SectionType, examId = created.ExamId }, created);
        }

        /// <summary>
        /// Update an existing home card.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<HomeSectionItemDto>> Update(int id, [FromBody] HomeSectionItemUpdateDto dto, CancellationToken cancellationToken)
        {
            if (id != dto.Id)
                return BadRequest("ID in URL and body must match.");

            var updated = await _homeContentService.UpdateAsync(dto, cancellationToken);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        /// <summary>
        /// Delete a home card.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var success = await _homeContentService.DeleteAsync(id, cancellationToken);
            if (!success)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Get all cards of a specific section for a given exam (optional).
        /// Example: sectionType=MockTest.
        /// </summary>
        [HttpGet("section/{sectionType}")]
        public async Task<ActionResult<IList<HomeSectionItemDto>>> GetBySection(HomeSectionType sectionType, [FromQuery] int? examId, CancellationToken cancellationToken)
        {
            var items = await _homeContentService.GetBySectionAsync(sectionType, examId, cancellationToken);
            return Ok(items);
        }
    }
}
