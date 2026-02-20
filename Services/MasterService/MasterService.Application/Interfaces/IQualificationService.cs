using MasterService.Application.DTOs;

namespace MasterService.Application.Interfaces
{
    public interface IQualificationService
    {
        Task<QualificationDto> CreateQualificationAsync(CreateQualificationDto createDto);
        Task<QualificationDto?> UpdateQualificationAsync(int id, UpdateQualificationDto updateDto);
        Task<bool> DeleteQualificationAsync(int id);
        Task<QualificationDto?> GetQualificationByIdAsync(int id, int? languageId = null);
        Task<IEnumerable<QualificationDto>> GetAllQualificationsAsync(int? languageId = null);
        Task<IEnumerable<QualificationDto>> GetQualificationsByCountryCodeAsync(string countryCode, int? languageId = null);
        Task<bool> ToggleQualificationStatusAsync(int id, bool isActive);
    }
}
