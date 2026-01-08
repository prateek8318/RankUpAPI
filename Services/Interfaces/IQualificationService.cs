using RankUpAPI.DTOs;

namespace RankUpAPI.Services.Interfaces
{
    public interface IQualificationService
    {
        Task<QualificationDto> CreateQualificationAsync(CreateQualificationDto createDto);
        Task<QualificationDto?> UpdateQualificationAsync(int id, UpdateQualificationDto updateDto);
        Task<bool> DeleteQualificationAsync(int id);
        Task<QualificationDto?> GetQualificationByIdAsync(int id);
        Task<IEnumerable<QualificationDto>> GetAllQualificationsAsync();
        Task<bool> ToggleQualificationStatusAsync(int id, bool isActive);
    }
}
