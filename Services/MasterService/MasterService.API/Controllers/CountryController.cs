using MasterService.Application.DTOs;
using MasterService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterService.API.Controllers
{
    [Route("api/countries")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;
        private readonly ILogger<CountryController> _logger;

        public CountryController(ICountryService countryService, ILogger<CountryController> logger)
        {
            _countryService = countryService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CountryDto>>> GetCountries([FromQuery] string? language = null)
        {
            try
            {
                IEnumerable<CountryDto> countries;
                
                if (!string.IsNullOrEmpty(language))
                {
                    countries = await _countryService.GetCountriesByLanguageAsync(language);
                }
                else
                {
                    countries = await _countryService.GetAllCountriesAsync();
                }
                
                return Ok(countries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving countries");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CountryDto>> GetCountry(int id, [FromQuery] string? language = null)
        {
            try
            {
                CountryDto? country;
                
                if (!string.IsNullOrEmpty(language))
                {
                    country = await _countryService.GetCountryByIdAsync(id, language);
                }
                else
                {
                    country = await _countryService.GetCountryByIdAsync(id);
                }
                
                if (country == null)
                    return NotFound();

                return Ok(country);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving country {CountryId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("code/{code}")]
        [AllowAnonymous]
        public async Task<ActionResult<CountryDto>> GetCountryByCode(string code, [FromQuery] string? language = null)
        {
            try
            {
                CountryDto? country;
                
                if (!string.IsNullOrEmpty(language))
                {
                    country = await _countryService.GetCountryByCodeAsync(code, language);
                }
                else
                {
                    country = await _countryService.GetCountryByCodeAsync(code);
                }
                
                if (country == null)
                    return NotFound();

                return Ok(country);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving country by code {CountryCode}", code);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CountryDto>> CreateCountry(CreateCountryDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var country = await _countryService.CreateCountryAsync(createDto);
                return CreatedAtAction(nameof(GetCountry), new { id = country.Id }, country);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating country");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCountry(int id, UpdateCountryDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                    return BadRequest("ID in URL does not match the ID in the request body.");

                var result = await _countryService.UpdateCountryAsync(id, updateDto);
                if (result == null)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating country");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            try
            {
                var result = await _countryService.DeleteCountryAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting country");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCountryStatus(int id, [FromBody] bool isActive)
        {
            try
            {
                var result = await _countryService.ToggleCountryStatusAsync(id, isActive);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating country status");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
