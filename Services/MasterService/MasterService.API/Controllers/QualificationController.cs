using MasterService.Application.DTOs;
using MasterService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Services;
using ILanguageService = Common.Services.ILanguageService;

namespace MasterService.API.Controllers
{
    [Route("api/qualifications")]
    [ApiController]
    public class QualificationController : ControllerBase
    {
        private readonly IQualificationService _qualificationService;
        private readonly ILogger<QualificationController> _logger;
        private readonly ILanguageService _languageService;

        public QualificationController(IQualificationService qualificationService, ILogger<QualificationController> logger, ILanguageService languageService)
        {
            _qualificationService = qualificationService;
            _logger = logger;
            _languageService = languageService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<QualificationDto>>> GetQualifications([FromQuery] int? languageId = null, [FromQuery] string? countryCode = null)
        {
            try
            {
                if (!languageId.HasValue)
                    languageId = GetLanguageIdFromHeader();

                IEnumerable<QualificationDto> list = string.IsNullOrEmpty(countryCode)
                    ? await _qualificationService.GetAllQualificationsAsync(languageId)
                    : await _qualificationService.GetQualificationsByCountryCodeAsync(countryCode, languageId);

                var filtered = FilterByLanguage(list, languageId);
                return Ok(filtered);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving qualifications");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<QualificationDto>> GetQualification(int id, [FromQuery] int? languageId = null)
        {
            try
            {
                if (!languageId.HasValue)
                    languageId = GetLanguageIdFromHeader();

                var qualification = await _qualificationService.GetQualificationByIdAsync(id, languageId);
                if (qualification == null)
                    return NotFound();

                var filtered = FilterByLanguage(new[] { qualification }, languageId).FirstOrDefault();
                return Ok(filtered);
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
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
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

            try
            {
                var result = await _qualificationService.UpdateQualificationAsync(id, updateDto);
                if (result == null)
                    return NotFound();
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating qualification {QualificationId}", id);
                return StatusCode(500, "Internal server error");
            }
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

        private int? GetLanguageIdFromHeader()
        {
            var code = _languageService.GetCurrentLanguage();
            return code switch
            {
                "en" => 50,
                "hi" => 49,
                _ => 50
            };
        }

        private static IEnumerable<QualificationDto> FilterByLanguage(IEnumerable<QualificationDto> list, int? languageId)
        {
            if (!languageId.HasValue)
                return list;
            return list.Select(q => new QualificationDto
            {
                Id = q.Id,
                Name = q.Name,
                Description = q.Description,
                CountryCode = q.CountryCode,
                IsActive = q.IsActive,
                CreatedAt = q.CreatedAt,
                Names = q.Names?.Where(n => n.LanguageId == languageId.Value).ToList() ?? new List<QualificationLanguageDto>()
            });
        }
    }
}
