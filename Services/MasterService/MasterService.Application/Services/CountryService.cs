using MasterService.Application.DTOs;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace MasterService.Application.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;
        private readonly ILogger<CountryService> _logger;

        public CountryService(ICountryRepository countryRepository, ILogger<CountryService> logger)
        {
            _countryRepository = countryRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<CountryDto>> GetAllCountriesAsync()
        {
            try
            {
                var countries = await _countryRepository.GetAllAsync();
                return countries.Select(c => MapToDto(c));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all countries");
                throw;
            }
        }

        public async Task<IEnumerable<CountryDto>> GetCountriesByLanguageAsync(string language)
        {
            try
            {
                var countries = await _countryRepository.GetAllAsync();
                
                if (language.ToLower() == "hi")
                {
                    // For Hindi, we would need to implement multi-language support
                    // For now, return English names
                    return countries.Select(c => new CountryDto
                    {
                        Id = c.Id,
                        Name = c.Name, // Would be Hindi name in proper implementation
                        Code = c.Code,
                        SubdivisionLabelEn = c.SubdivisionLabelEn,
                        SubdivisionLabelHi = c.SubdivisionLabelHi ?? c.SubdivisionLabelEn,
                        IsActive = c.IsActive,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    });
                }
                
                return countries.Select(c => MapToDto(c));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting countries by language {Language}", language);
                throw;
            }
        }

        public async Task<CountryDto?> GetCountryByIdAsync(int id, string? language = null)
        {
            try
            {
                var country = await _countryRepository.GetByIdAsync(id);
                if (country == null)
                    return null;

                return MapToDto(country, language);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting country by id {CountryId}", id);
                throw;
            }
        }

        public async Task<CountryDto?> GetCountryByCodeAsync(string code, string? language = null)
        {
            try
            {
                var country = await _countryRepository.GetByCodeAsync(code);
                if (country == null)
                    return null;

                return MapToDto(country, language);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting country by code {CountryCode}", code);
                throw;
            }
        }

        public async Task<CountryDto> CreateCountryAsync(CreateCountryDto createDto)
        {
            try
            {
                var country = new Country
                {
                    Name = createDto.NameEn, // Store English name for now
                    Code = createDto.Code,
                    SubdivisionLabelEn = createDto.SubdivisionLabelEn,
                    SubdivisionLabelHi = createDto.SubdivisionLabelHi,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var createdCountry = await _countryRepository.AddAsync(country);
                await _countryRepository.SaveChangesAsync();

                return MapToDto(createdCountry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating country");
                throw;
            }
        }

        public async Task<CountryDto?> UpdateCountryAsync(int id, UpdateCountryDto updateDto)
        {
            try
            {
                var existingCountry = await _countryRepository.GetByIdAsync(id);
                if (existingCountry == null)
                    return null;

                existingCountry.Name = updateDto.NameEn;
                existingCountry.Code = updateDto.Code;
                existingCountry.SubdivisionLabelEn = updateDto.SubdivisionLabelEn;
                existingCountry.SubdivisionLabelHi = updateDto.SubdivisionLabelHi;
                existingCountry.IsActive = updateDto.IsActive;
                existingCountry.UpdatedAt = DateTime.UtcNow;

                var updatedCountry = await _countryRepository.UpdateAsync(existingCountry);
                await _countryRepository.SaveChangesAsync();

                return MapToDto(updatedCountry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating country {CountryId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteCountryAsync(int id)
        {
            try
            {
                var country = await _countryRepository.GetByIdAsync(id);
                if (country == null)
                    return false;

                await _countryRepository.DeleteAsync(country);
                await _countryRepository.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting country {CountryId}", id);
                throw;
            }
        }

        public async Task<bool> ToggleCountryStatusAsync(int id, bool isActive)
        {
            try
            {
                var country = await _countryRepository.GetByIdAsync(id);
                if (country == null)
                    return false;

                country.IsActive = isActive;
                country.UpdatedAt = DateTime.UtcNow;

                await _countryRepository.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling country status {CountryId}", id);
                throw;
            }
        }

        private static CountryDto MapToDto(Country country, string? language = null)
        {
            return new CountryDto
            {
                Id = country.Id,
                Name = country.Name, // In proper implementation, this would be localized
                Code = country.Code,
                SubdivisionLabelEn = country.SubdivisionLabelEn,
                SubdivisionLabelHi = country.SubdivisionLabelHi,
                IsActive = country.IsActive,
                CreatedAt = country.CreatedAt,
                UpdatedAt = country.UpdatedAt
            };
        }
    }
}
