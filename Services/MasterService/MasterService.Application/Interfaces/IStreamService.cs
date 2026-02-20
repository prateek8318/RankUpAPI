using MasterService.Application.DTOs;

namespace MasterService.Application.Interfaces
{
    public interface IStreamService
    {
        Task<StreamDto> CreateStreamAsync(CreateStreamDto createDto);
        Task<StreamDto?> UpdateStreamAsync(int id, UpdateStreamDto updateDto);
        Task<bool> DeleteStreamAsync(int id);
        Task<StreamDto?> GetStreamByIdAsync(int id, int? languageId = null);
        Task<IEnumerable<StreamDto>> GetAllStreamsAsync(int? languageId = null);
        Task<IEnumerable<StreamDto>> GetStreamsByQualificationIdAsync(int qualificationId, int? languageId = null);
        Task<bool> ToggleStreamStatusAsync(int id, bool isActive);
    }
}
