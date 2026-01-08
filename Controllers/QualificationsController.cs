using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RankUpAPI.DTOs;
using RankUpAPI.Services.Interfaces;

namespace RankUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class QualificationsController : ControllerBase
    {
        private readonly IQualificationService _qualificationService;

        public QualificationsController(IQualificationService qualificationService)
        {
            _qualificationService = qualificationService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<QualificationDto>>> GetQualifications()
        {
            var qualifications = await _qualificationService.GetAllQualificationsAsync();
            return Ok(qualifications);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<QualificationDto>> GetQualification(int id)
        {
            var qualification = await _qualificationService.GetQualificationByIdAsync(id);
            if (qualification == null)
                return NotFound();

            return Ok(qualification);
        }

        [HttpPost]
        public async Task<ActionResult<QualificationDto>> CreateQualification(CreateQualificationDto createDto)
        {
            var qualification = await _qualificationService.CreateQualificationAsync(createDto);
            return CreatedAtAction(nameof(GetQualification), new { id = qualification.Id }, qualification);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQualification(int id, UpdateQualificationDto updateDto)
        {
            var result = await _qualificationService.UpdateQualificationAsync(id, updateDto);
            if (result == null)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQualification(int id)
        {
            var result = await _qualificationService.DeleteQualificationAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateQualificationStatus(int id, [FromBody] bool isActive)
        {
            var result = await _qualificationService.ToggleQualificationStatusAsync(id, isActive);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
