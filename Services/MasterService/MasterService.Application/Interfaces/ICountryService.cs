using MasterService.Application.DTOs;

namespace MasterService.Application.Interfaces
{
    public interface ICountryService
    {
        Task<IEnumerable<CountryDto>> GetAllCountriesAsync();
        Task<IEnumerable<CountryDto>> GetCountriesByLanguageAsync(string language);
        Task<CountryDto?> GetCountryByIdAsync(int id, string? language = null);
        Task<CountryDto?> GetCountryByCodeAsync(string code, string? language = null);
        Task<CountryDto> CreateCountryAsync(CreateCountryDto createDto);
        Task<CountryDto?> UpdateCountryAsync(int id, UpdateCountryDto updateDto);
        Task<bool> DeleteCountryAsync(int id);
        Task<bool> ToggleCountryStatusAsync(int id, bool isActive);
    }
}
