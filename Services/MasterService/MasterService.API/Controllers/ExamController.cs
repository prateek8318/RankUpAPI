using MasterService.Application.DTOs;
using MasterService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Services;
using ILanguageServiceCommon = Common.Services.ILanguageService;

namespace MasterService.API.Controllers
{
    [Route("api/exams")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;
        private readonly ILogger<ExamController> _logger;
        private readonly ILanguageServiceCommon _languageService;

        public ExamController(IExamService examService, ILogger<ExamController> logger, ILanguageServiceCommon languageService)
        {
            _examService = examService;
            _logger = logger;
            _languageService = languageService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ExamDto>>> GetExams(
            [FromQuery] string? language = null,
            [FromQuery] int? languageId = null,
            [FromQuery] string? countryCode = null,
            [FromQuery] int? qualificationId = null,
            [FromQuery] int? streamId = null,
            [FromQuery] int? minAge = null,
            [FromQuery] int? maxAge = null)
        {
            try
            {
                // Priority: language parameter > languageId > header
                string? selectedLanguage = null;
                
                if (!string.IsNullOrEmpty(language))
                {
                    selectedLanguage = language;
                }
                else if (!languageId.HasValue)
                {
                    languageId = GetLanguageIdFromHeader();
                }
                else
                {
                    languageId = GetLanguageIdFromHeader();
                }

                IEnumerable<ExamDto> list;
                
                if (!string.IsNullOrEmpty(selectedLanguage))
                {
                    // Use new language-based method
                    if (!string.IsNullOrEmpty(countryCode) || qualificationId.HasValue || streamId.HasValue || minAge.HasValue || maxAge.HasValue)
                    {
                        list = await _examService.GetExamsByFilterAsync(selectedLanguage, countryCode, qualificationId, streamId, minAge, maxAge);
                    }
                    else
                    {
                        list = await _examService.GetAllExamsAsync(selectedLanguage);
                    }
                }
                else
                {
                    // Use existing languageId-based method
                    if (!string.IsNullOrEmpty(countryCode) || qualificationId.HasValue || streamId.HasValue || minAge.HasValue || maxAge.HasValue)
                    {
                        list = await _examService.GetExamsByFilterAsync(countryCode, qualificationId, streamId, minAge, maxAge, languageId);
                    }
                    else
                    {
                        list = await _examService.GetAllExamsAsync(languageId);
                    }
                }

                var filtered = FilterByLanguage(list, languageId);
                return Ok(filtered);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exams");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ExamDto>> GetExam(int id, [FromQuery] int? languageId = null)
        {
            try
            {
                if (!languageId.HasValue)
                    languageId = GetLanguageIdFromHeader();

                var exam = await _examService.GetExamByIdAsync(id, languageId);
                if (exam == null)
                    return NotFound();

                var filtered = FilterByLanguage(new[] { exam }, languageId).FirstOrDefault();
                return Ok(filtered);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exam {ExamId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ExamDto>> CreateExam(CreateExamDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

                var exam = await _examService.CreateExamAsync(createDto);
                return CreatedAtAction(nameof(GetExam), new { id = exam.Id }, new { success = true, data = exam });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating exam");
                return StatusCode(500, new { success = false, message = "Internal server error while creating exam." });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateExam(int id, UpdateExamDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest(new { success = false, message = "ID in the URL does not match the ID in the request body." });

            try
            {
                var result = await _examService.UpdateExamAsync(id, updateDto);
                if (result == null)
                    return NotFound(new { success = false, message = "Exam not found." });
                    
                return Ok(new { success = true, message = "Exam updated successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating exam {ExamId}", id);
                return StatusCode(500, new { success = false, message = "Internal server error while updating exam." });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> DeleteExam(int id)
        {
            try
            {
                _logger.LogInformation("DELETE request received for exam ID {ExamId}", id);
                
                var result = await _examService.DeleteExamAsync(id);
                if (!result)
                {
                    _logger.LogWarning("Failed to delete exam with ID {ExamId}", id);
                    return NotFound(new { success = false, message = "Exam not found" });
                }

                _logger.LogInformation("Exam with ID {ExamId} deleted successfully", id);
                return Ok(new { success = true, message = "Exam deleted permanently from database" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting exam");
                return StatusCode(500, new { success = false, message = "Error deleting exam" });
            }
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> UpdateExamStatus(int id, [FromBody] bool isActive)
        {
            try
            {
                var result = await _examService.ToggleExamStatusAsync(id, isActive);
                if (!result)
                    return NotFound(new { success = false, message = "Exam not found" });

                var message = isActive ? "Exam activated successfully" : "Exam deactivated successfully";
                return Ok(new
                {
                    success = true,
                    message = message,
                    data = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating exam status");
                return StatusCode(500, new { success = false, message = "Error updating exam status" });
            }
        }

        /// <summary>
        /// Get all exams including inactive ones (Admin only)
        /// </summary>
        /// <param name="language">Optional language code (e.g., 'en', 'hi')</param>
        /// <param name="languageId">Optional language ID</param>
        /// <param name="countryCode">Optional country code filter</param>
        /// <param name="qualificationId">Optional qualification ID filter</param>
        /// <param name="streamId">Optional stream ID filter</param>
        /// <param name="minAge">Optional minimum age filter</param>
        /// <param name="maxAge">Optional maximum age filter</param>
        /// <returns>List of all exams including inactive</returns>
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> GetAllExamsForAdmin(
            [FromQuery] string? language = null,
            [FromQuery] int? languageId = null,
            [FromQuery] string? countryCode = null,
            [FromQuery] int? qualificationId = null,
            [FromQuery] int? streamId = null,
            [FromQuery] int? minAge = null,
            [FromQuery] int? maxAge = null)
        {
            try
            {
                // Priority: language parameter > languageId > header
                string? selectedLanguage = null;
                
                if (!string.IsNullOrEmpty(language))
                {
                    selectedLanguage = language;
                }
                else if (!languageId.HasValue)
                {
                    languageId = GetLanguageIdFromHeader();
                }

                IEnumerable<ExamDto> exams;
                
                if (countryCode != null || qualificationId != null || streamId != null || minAge != null || maxAge != null)
                {
                    // Use filter method when filters are applied
                    exams = await _examService.GetExamsByFilterAsync(selectedLanguage ?? "en", countryCode, qualificationId, streamId, minAge, maxAge);
                }
                else
                {
                    // Use new method to get all including inactive
                    exams = await _examService.GetAllExamsIncludingInactiveAsync(selectedLanguage, languageId);
                }
                
                return Ok(new
                {
                    success = true,
                    data = exams,
                    language = selectedLanguage,
                    message = "All exams fetched successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all exams for admin");
                return StatusCode(500, new { success = false, message = "Error fetching exams" });
            }
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

        private static IEnumerable<ExamDto> FilterByLanguage(IEnumerable<ExamDto> list, int? languageId)
        {
            if (!languageId.HasValue)
                return list;

            return list.Select(e => {
                var localizedExam = e.Names?.FirstOrDefault(n => n.LanguageId == languageId.Value);
                
                return new ExamDto
                {
                    Id = e.Id,
                    Name = localizedExam?.Name ?? e.Name, // Use localized name if available, fallback to base name
                    Description = localizedExam?.Description ?? e.Description, // Use localized description if available
                    CountryCode = e.CountryCode,
                    MinAge = e.MinAge,
                    MaxAge = e.MaxAge,
                    ImageUrl = e.ImageUrl,
                    IsInternational = e.IsInternational,
                    IsActive = e.IsActive,
                    CreatedAt = e.CreatedAt,
                    QualificationIds = e.QualificationIds,
                    StreamIds = e.StreamIds,
                    Names = e.Names?.Where(n => n.LanguageId == languageId.Value).ToList() ?? new List<ExamLanguageDto>()
                };
            });
        }
    }
}

