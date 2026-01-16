using MasterService.Application.DTOs;

namespace MasterService.Application.Interfaces
{
    public interface ILanguageService
    {
        Task<LanguageDto> CreateLanguageAsync(CreateLanguageDto createDto);
        Task<LanguageDto?> UpdateLanguageAsync(int id, UpdateLanguageDto updateDto);
        Task<bool> DeleteLanguageAsync(int id);
        Task<LanguageDto?> GetLanguageByIdAsync(int id);
        Task<IEnumerable<LanguageDto>> GetAllLanguagesAsync();
        Task<bool> ToggleLanguageStatusAsync(int id, bool isActive);
    }
}
