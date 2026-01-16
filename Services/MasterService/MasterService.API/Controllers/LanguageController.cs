using MasterService.Application.DTOs;
using MasterService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterService.API.Controllers
{
    [Route("api/languages")]
    [ApiController]
    public class LanguageController : ControllerBase
    {
        private readonly ILanguageService _languageService;
        private readonly ILogger<LanguageController> _logger;

        public LanguageController(ILanguageService languageService, ILogger<LanguageController> logger)
        {
            _languageService = languageService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<LanguageDto>>> GetLanguages()
        {
            try
            {
                var languages = await _languageService.GetAllLanguagesAsync();
                return Ok(languages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving languages");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<LanguageDto>> GetLanguage(int id)
        {
            try
            {
                var language = await _languageService.GetLanguageByIdAsync(id);
                if (language == null)
                    return NotFound();

                return Ok(language);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving language {LanguageId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<LanguageDto>> CreateLanguage(CreateLanguageDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var language = await _languageService.CreateLanguageAsync(createDto);
                return CreatedAtAction(nameof(GetLanguage), new { id = language.Id }, language);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating language");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateLanguage(int id, UpdateLanguageDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest("ID in the URL does not match the ID in the request body.");

            var result = await _languageService.UpdateLanguageAsync(id, updateDto);
            if (result == null)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteLanguage(int id)
        {
            var result = await _languageService.DeleteLanguageAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateLanguageStatus(int id, [FromBody] bool isActive)
        {
            var result = await _languageService.ToggleLanguageStatusAsync(id, isActive);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
