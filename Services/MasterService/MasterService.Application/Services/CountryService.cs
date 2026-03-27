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
                        Iso2 = c.Iso2,
                        Phone = new PhoneDto
                        {
                            CountryCode = c.CountryCode,
                            Length = c.PhoneLength
                        },
                        CurrencyCode = c.CurrencyCode,
                        Image = c.Image,
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
                    Name = createDto.Name,
                    Iso2 = createDto.Iso2,
                    CountryCode = createDto.CountryCode,
                    PhoneLength = createDto.PhoneLength,
                    CurrencyCode = createDto.CurrencyCode,
                    Image = createDto.Image,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var createdCountry = await _countryRepository.AddAsync(country);

                return MapToDto(createdCountry);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Country with this ISO2 code already exists"))
            {
                throw new InvalidOperationException($"Country with ISO2 code '{createDto.Iso2}' already exists", ex);
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

                existingCountry.Name = updateDto.Name;
                existingCountry.Iso2 = updateDto.Iso2;
                existingCountry.CountryCode = updateDto.CountryCode;
                existingCountry.PhoneLength = updateDto.PhoneLength;
                existingCountry.CurrencyCode = updateDto.CurrencyCode;
                existingCountry.Image = updateDto.Image;
                existingCountry.IsActive = updateDto.IsActive;
                existingCountry.UpdatedAt = DateTime.UtcNow;

                var updatedCountry = await _countryRepository.UpdateAsync(existingCountry);

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
                return await _countryRepository.ToggleCountryStatusAsync(id, isActive);
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
                Iso2 = country.Iso2,
                Phone = new PhoneDto
                {
                    CountryCode = country.CountryCode,
                    Length = country.PhoneLength
                },
                CurrencyCode = country.CurrencyCode,
                Image = country.Image,
                IsActive = country.IsActive,
                CreatedAt = country.CreatedAt,
                UpdatedAt = country.UpdatedAt
            };
        }
    }
}
