using MasterService.Application.DTOs;
using MasterService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Common.Services;
using Common.Language;
using ILanguageService = Common.Services.ILanguageService;
using ILanguageDataService = Common.Language.ILanguageDataService;

namespace MasterService.API.Controllers
{
    [Route("api/countries")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;
        private readonly ILogger<CountryController> _logger;
        private readonly ILanguageService _languageService;
        private readonly IImageService _imageService;

        public CountryController(ICountryService countryService, ILogger<CountryController> logger, ILanguageService languageService, IImageService imageService)
        {
            _countryService = countryService;
            _logger = logger;
            _languageService = languageService;
            _imageService = imageService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetCountries([FromQuery] string? language = null)
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
                
                return Ok(new
                {
                    success = true,
                    message = "Countries fetched successfully",
                    data = countries
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving countries");
                return StatusCode(500, new { success = false, message = "Error fetching countries" });
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetCountry(int id, [FromQuery] string? language = null)
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
                    return NotFound(new { success = false, message = "Country not found" });

                return Ok(new
                {
                    success = true,
                    message = "Country fetched successfully",
                    data = country
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving country {CountryId}", id);
                return StatusCode(500, new { success = false, message = "Error fetching country" });
            }
        }

        [HttpGet("code/{code}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetCountryByCode(string code, [FromQuery] string? language = null)
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
                    return NotFound(new { success = false, message = "Country not found" });

                return Ok(new
                {
                    success = true,
                    message = "Country fetched successfully",
                    data = country
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving country by code {CountryCode}", code);
                return StatusCode(500, new { success = false, message = "Error fetching country" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> CreateCountry([FromForm] CreateCountryWithImageDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Invalid model data", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

                // Handle image upload if provided
                string? imagePath = null;
                if (createDto.ImageFile != null)
                {
                    imagePath = await _imageService.UploadCountryImageAsync(createDto.ImageFile, createDto.Iso2);
                }

                // Create country DTO with image path
                var countryDto = new CreateCountryDto
                {
                    Name = createDto.Name,
                    Iso2 = createDto.Iso2,
                    CountryCode = createDto.CountryCode,
                    PhoneLength = createDto.PhoneLength,
                    CurrencyCode = createDto.CurrencyCode,
                    Image = imagePath
                };

                var country = await _countryService.CreateCountryAsync(countryDto);
                return CreatedAtAction(nameof(GetCountry), new { id = country.Id }, new
                {
                    success = true,
                    message = "Country created successfully",
                    data = country
                });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
            {
                return BadRequest(new 
                { 
                    success = false, 
                    message = ex.Message,
                    error = "DUPLICATE_ISO2"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating country");
                return StatusCode(500, new { success = false, message = "Error creating country" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> UpdateCountry(int id, [FromForm] UpdateCountryWithImageDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                    return BadRequest(new { success = false, message = "ID in URL does not match the ID in the request body" });

                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Invalid model data", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

                // Handle image upload if provided
                string? imagePath = null;
                if (updateDto.ImageFile != null)
                {
                    imagePath = await _imageService.UploadCountryImageAsync(updateDto.ImageFile, updateDto.Iso2);
                }

                // Create update DTO with image path
                var countryDto = new UpdateCountryDto
                {
                    Id = updateDto.Id,
                    Name = updateDto.Name,
                    Iso2 = updateDto.Iso2,
                    CountryCode = updateDto.CountryCode,
                    PhoneLength = updateDto.PhoneLength,
                    CurrencyCode = updateDto.CurrencyCode,
                    Image = imagePath,
                    IsActive = updateDto.IsActive
                };

                var result = await _countryService.UpdateCountryAsync(id, countryDto);
                if (result == null)
                    return NotFound(new { success = false, message = "Country not found" });

                return Ok(new
                {
                    success = true,
                    message = "Country updated successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating country");
                return StatusCode(500, new { success = false, message = "Error updating country" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> DeleteCountry(int id)
        {
            try
            {
                var result = await _countryService.DeleteCountryAsync(id);
                if (!result)
                    return NotFound(new { success = false, message = "Country not found" });

                return Ok(new
                {
                    success = true,
                    message = "Country deleted successfully",
                    data = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting country");
                return StatusCode(500, new { success = false, message = "Error deleting country" });
            }
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> UpdateCountryStatus(int id, [FromBody] bool isActive)
        {
            try
            {
                var result = await _countryService.ToggleCountryStatusAsync(id, isActive);
                if (!result)
                    return NotFound(new { success = false, message = "Country not found" });

                var message = isActive ? "Country activated successfully" : "Country deactivated successfully";
                return Ok(new
                {
                    success = true,
                    message = message,
                    data = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating country status");
                return StatusCode(500, new { success = false, message = "Error updating country status" });
            }
        }
    }
}
