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
        public async Task<ActionResult<IEnumerable<StateDto>>> GetStates([FromQuery] int? languageId = null, [FromQuery] string? countryCode = null)
        {
            try
            {
                // Get language from header if not provided in query
                if (!languageId.HasValue)
                {
                    languageId = GetLanguageIdFromHeader();
                }

                IEnumerable<StateDto> states;
                
                if (!string.IsNullOrEmpty(countryCode))
                {
                    states = await _stateService.GetStatesByCountryCodeAsync(countryCode, languageId);
                }
                else
                {
                    states = await _stateService.GetAllStatesAsync(languageId);
                }

                // Filter names array to show only selected language
                var filteredStates = FilterStatesByLanguage(states, languageId);
                
                return Ok(filteredStates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving states");
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

                // Filter names array to show only selected language
                var filteredState = FilterStatesByLanguage(new[] { state }, languageId).FirstOrDefault();
                
                return Ok(filteredState);
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

        private IEnumerable<StateDto> FilterStatesByLanguage(IEnumerable<StateDto> states, int? languageId)
        {
            if (!languageId.HasValue)
                return states;

            return states.Select(state => new StateDto
            {
                Id = state.Id,
                Name = state.Name,
                Code = state.Code,
                CountryCode = state.CountryCode,
                IsActive = state.IsActive,
                CreatedAt = state.CreatedAt,
                Names = state.Names?.Where(n => n.LanguageId == languageId.Value).ToList() ?? new List<StateLanguageDto>()
            });
        }
    }
}
