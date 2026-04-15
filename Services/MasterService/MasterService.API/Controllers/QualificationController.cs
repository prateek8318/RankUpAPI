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
        public async Task<ActionResult<IEnumerable<QualificationDto>>> GetQualifications([FromQuery] string? language = null, [FromQuery] int? languageId = null, [FromQuery] string? countryCode = null, [FromQuery] bool includeInactive = false)
        {
            try
            {
                var isAdmin = User.IsInRole("Admin");
                var shouldIncludeInactive = includeInactive || isAdmin;
                IEnumerable<QualificationDto> qualifications;
                
                if (!string.IsNullOrEmpty(language))
                {
                    qualifications = shouldIncludeInactive
                        ? (string.IsNullOrEmpty(countryCode)
                            ? await _qualificationService.GetAllQualificationsIncludingInactiveAsync(language)
                            : await _qualificationService.GetQualificationsByCountryCodeIncludingInactiveAsync(countryCode, language))
                        : (string.IsNullOrEmpty(countryCode)
                            ? await _qualificationService.GetAllQualificationsAsync(language)
                            : await _qualificationService.GetQualificationsByCountryCodeAsync(countryCode, language));
                }
                else
                {
                    var finalLanguageId = languageId ?? GetLanguageIdFromHeader();
                    qualifications = shouldIncludeInactive
                        ? (string.IsNullOrEmpty(countryCode)
                            ? await _qualificationService.GetAllQualificationsIncludingInactiveAsync(finalLanguageId)
                            : await _qualificationService.GetQualificationsByCountryCodeIncludingInactiveAsync(countryCode, finalLanguageId))
                        : (string.IsNullOrEmpty(countryCode)
                            ? await _qualificationService.GetAllQualificationsAsync(finalLanguageId)
                            : await _qualificationService.GetQualificationsByCountryCodeAsync(countryCode, finalLanguageId));
                }

                return Ok(qualifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving qualifications");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<QualificationDto>>> GetAllQualificationsForAdmin([FromQuery] string? language = null, [FromQuery] int? languageId = null, [FromQuery] string? countryCode = null)
        {
            try
            {
                IEnumerable<QualificationDto> qualifications;
                
                if (!string.IsNullOrEmpty(language))
                {
                    qualifications = string.IsNullOrEmpty(countryCode)
                        ? await _qualificationService.GetAllQualificationsIncludingInactiveAsync(language)
                        : await _qualificationService.GetQualificationsByCountryCodeIncludingInactiveAsync(countryCode, language);
                }
                else
                {
                    var finalLanguageId = languageId ?? GetLanguageIdFromHeader();
                    qualifications = string.IsNullOrEmpty(countryCode)
                        ? await _qualificationService.GetAllQualificationsIncludingInactiveAsync(finalLanguageId)
                        : await _qualificationService.GetQualificationsByCountryCodeIncludingInactiveAsync(countryCode, finalLanguageId);
                }

                return Ok(new
                {
                    success = true,
                    data = qualifications,
                    message = "All qualifications (including inactive) fetched successfully for admin"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all qualifications for admin");
                return StatusCode(500, new { success = false, message = "Error fetching qualifications" });
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<QualificationDto>> GetQualification(int id, [FromQuery] string? language = null, [FromQuery] int? languageId = null)
        {
            try
            {
                int? finalLanguageId = languageId;
                
                if (!string.IsNullOrEmpty(language))
                {
                    finalLanguageId = language.ToLower() switch
                    {
                        "hi" => 49,
                        "en" => 50,
                        _ => 50
                    };
                }
                else if (!finalLanguageId.HasValue)
                {
                    finalLanguageId = GetLanguageIdFromHeader();
                }

                var qualification = await _qualificationService.GetQualificationByIdAsync(id, finalLanguageId);
                if (qualification == null)
                    return NotFound();

                // Don't filter here - service already handles multilingual logic
                return Ok(qualification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving qualification {QualificationId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [AllowAnonymous]
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
        [AllowAnonymous]
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
