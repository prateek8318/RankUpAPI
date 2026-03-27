using MasterService.Application.DTOs;
using MasterService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Services;
using Common.Language;
using ILanguageService = Common.Services.ILanguageService;

namespace MasterService.API.Controllers
{
    [Route("api/states")]
    [ApiController]
    public class StateController : ControllerBase
    {
        private readonly IStateService _stateService;
        private readonly ILogger<StateController> _logger;
        private readonly ILanguageService _languageService;

        public StateController(IStateService stateService, ILogger<StateController> logger, ILanguageService languageService)
        {
            _stateService = stateService;
            _logger = logger;
            _languageService = languageService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<StateDto>>> GetStates(
            [FromQuery] string? language = null,
            [FromQuery] int? languageId = null,
            [FromQuery] string? countryCode = null,
            [FromQuery] bool includeInactive = false)
        {
            try
            {
                // Priority: language parameter > languageId > header
                string? selectedLanguage = null;
                int? effectiveLanguageId = null;
                
                if (!string.IsNullOrEmpty(language))
                {
                    selectedLanguage = language;
                }
                else
                {
                    effectiveLanguageId = languageId ?? GetLanguageIdFromHeader();
                }

                IEnumerable<StateDto> states;
                
                if (!string.IsNullOrEmpty(selectedLanguage))
                {
                    // Use new language-based method
                    if (!string.IsNullOrEmpty(countryCode))
                    {
                        states = await _stateService.GetStatesByCountryCodeAsync(countryCode, selectedLanguage);
                    }
                    else
                    {
                        states = await _stateService.GetAllStatesAsync(selectedLanguage);
                    }
                }
                else
                {
                    // Use existing languageId-based method
                    if (!string.IsNullOrEmpty(countryCode))
                    {
                        states = await _stateService.GetStatesByCountryCodeAsync(countryCode, effectiveLanguageId);
                    }
                    else
                    {
                        states = await _stateService.GetAllStatesAsync(effectiveLanguageId);
                    }
                }

                // Role-based filtering: Admins can see inactive states, regular users only see active
                var isAdmin = User.IsInRole("Admin");
                var shouldIncludeInactive = includeInactive || isAdmin;

                // Filter by status if not including inactive
                if (!shouldIncludeInactive)
                {
                    states = states.Where(s => s.IsActive);
                }

                return Ok(states);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving states");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<StateDto>>> GetAllStatesForAdmin(
            [FromQuery] string? language = null,
            [FromQuery] int? languageId = null,
            [FromQuery] string? countryCode = null)
        {
            try
            {
                // Priority: language parameter > languageId > header
                string? selectedLanguage = null;
                int? effectiveLanguageId = null;
                
                if (!string.IsNullOrEmpty(language))
                {
                    selectedLanguage = language;
                }
                else
                {
                    effectiveLanguageId = languageId ?? GetLanguageIdFromHeader();
                }

                IEnumerable<StateDto> states;
                
                if (!string.IsNullOrEmpty(selectedLanguage))
                {
                    // Use new language-based method
                    if (!string.IsNullOrEmpty(countryCode))
                    {
                        states = await _stateService.GetStatesByCountryCodeAsync(countryCode, selectedLanguage);
                    }
                    else
                    {
                        states = await _stateService.GetAllStatesAsync(selectedLanguage);
                    }
                }
                else
                {
                    // Use existing languageId-based method
                    if (!string.IsNullOrEmpty(countryCode))
                    {
                        states = await _stateService.GetStatesByCountryCodeAsync(countryCode, effectiveLanguageId);
                    }
                    else
                    {
                        states = await _stateService.GetAllStatesAsync(effectiveLanguageId);
                    }
                }

                // Admin sees all states (both active and inactive)
                return Ok(states);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving states for admin");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<StateDto>> GetState(int id, [FromQuery] int? languageId = null)
        {
            try
            {
                // Get language from header if not provided in query
                if (!languageId.HasValue)
                {
                    languageId = GetLanguageIdFromHeader();
                }

                var state = await _stateService.GetStateByIdAsync(id, languageId);
                if (state == null)
                    return NotFound();

                return Ok(state);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving state with id {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<StateDto>> CreateState(CreateStateDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var state = await _stateService.CreateStateAsync(createDto);
                return CreatedAtAction(nameof(GetState), new { id = state.Id }, state);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating state");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateState(int id, UpdateStateDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != updateDto.Id)
                return BadRequest("ID in the URL does not match the ID in the request body.");

            try
            {
                var result = await _stateService.UpdateStateAsync(id, updateDto);
                if (result == null)
                    return NotFound();

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating state with id {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteState(int id)
        {
            var result = await _stateService.DeleteStateAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStateStatus(int id, [FromBody] bool isActive)
        {
            var result = await _stateService.ToggleStateStatusAsync(id, isActive);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPost("seed-languages")]
        [AllowAnonymous]
        public async Task<IActionResult> SeedLanguages()
        {
            try
            {
                await _stateService.SeedStateLanguagesAsync();
                return Ok(new { message = "State languages seeded successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding state languages");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("empty-names")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStatesWithEmptyNames()
        {
            try
            {
                var deletedCount = await _stateService.DeleteStatesWithEmptyNamesAsync();
                return Ok(new { message = $"Deleted {deletedCount} states with empty names" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting states with empty names");
                return StatusCode(500, "Internal server error");
            }
        }

        private int? GetLanguageIdFromHeader()
        {
            var languageCode = _languageService.GetCurrentLanguage();
            return languageCode switch
            {
                "en" => 50, // English
                "hi" => 49, // Hindi
                "ta" => null, // Tamil - add ID when available
                "gu" => null, // Gujarati - add ID when available
                _ => 50 // Default to English
            };
        }
    }
}
