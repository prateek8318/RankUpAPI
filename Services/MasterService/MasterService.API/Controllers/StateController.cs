using MasterService.Application.DTOs;
using MasterService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterService.API.Controllers
{
    [Route("api/states")]
    [ApiController]
    public class StateController : ControllerBase
    {
        private readonly IStateService _stateService;
        private readonly ILogger<StateController> _logger;

        public StateController(IStateService stateService, ILogger<StateController> logger)
        {
            _stateService = stateService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<StateDto>>> GetStates([FromQuery] int? languageId = null, [FromQuery] string? countryCode = null)
        {
            try
            {
                IEnumerable<StateDto> states;
                
                if (!string.IsNullOrEmpty(countryCode))
                {
                    states = await _stateService.GetStatesByCountryCodeAsync(countryCode, languageId);
                }
                else
                {
                    states = await _stateService.GetAllStatesAsync(languageId);
                }
                
                return Ok(states);
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
                var state = await _stateService.GetStateByIdAsync(id, languageId);
                if (state == null)
                    return NotFound();

                return Ok(state);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving state {StateId}", id);
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
            if (id != updateDto.Id)
                return BadRequest("ID in the URL does not match the ID in the request body.");

            var result = await _stateService.UpdateStateAsync(id, updateDto);
            if (result == null)
                return NotFound();

            return NoContent();
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
    }
}
