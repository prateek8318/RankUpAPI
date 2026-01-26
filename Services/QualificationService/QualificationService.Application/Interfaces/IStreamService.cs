using QualificationService.Application.DTOs;

namespace QualificationService.Application.Interfaces
{
    public interface IStreamService
    {
        Task<StreamDto> CreateStreamAsync(CreateStreamDto createDto);
        Task<StreamDto?> UpdateStreamAsync(int id, UpdateStreamDto updateDto);
        Task<bool> DeleteStreamAsync(int id);
        Task<StreamDto?> GetStreamByIdAsync(int id);
        Task<IEnumerable<StreamDto>> GetAllStreamsAsync();
        Task<bool> ToggleStreamStatusAsync(int id, bool isActive);
    }
}
