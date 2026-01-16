using QualificationService.Application.DTOs;
using QualificationService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QualificationService.API.Controllers
{
    [Route("api/qualifications")]
    [ApiController]
    public class QualificationController : ControllerBase
    {
        private readonly IQualificationService _qualificationService;
        private readonly ILogger<QualificationController> _logger;

        public QualificationController(IQualificationService qualificationService, ILogger<QualificationController> logger)
        {
            _qualificationService = qualificationService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<QualificationDto>>> GetQualifications()
        {
            try
            {
                var qualifications = await _qualificationService.GetAllQualificationsAsync();
                return Ok(qualifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving qualifications");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<QualificationDto>> GetQualification(int id)
        {
            try
            {
                var qualification = await _qualificationService.GetQualificationByIdAsync(id);
                if (qualification == null)
                    return NotFound();

                return Ok(qualification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving qualification {QualificationId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<QualificationDto>> CreateQualification(CreateQualificationDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var qualification = await _qualificationService.CreateQualificationAsync(createDto);
                return CreatedAtAction(nameof(GetQualification), new { id = qualification.Id }, qualification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating qualification");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateQualification(int id, UpdateQualificationDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest("ID in the URL does not match the ID in the request body.");

            var result = await _qualificationService.UpdateQualificationAsync(id, updateDto);
            if (result == null)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteQualification(int id)
        {
            var result = await _qualificationService.DeleteQualificationAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateQualificationStatus(int id, [FromBody] bool isActive)
        {
            var result = await _qualificationService.ToggleQualificationStatusAsync(id, isActive);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
