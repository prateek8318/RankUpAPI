using MasterService.Application.DTOs;

namespace MasterService.Application.Interfaces
{
    public interface IStateService
    {
        Task<StateDto> CreateStateAsync(CreateStateDto createDto);
        Task<StateDto?> UpdateStateAsync(int id, UpdateStateDto updateDto);
        Task<bool> DeleteStateAsync(int id);
        Task<StateDto?> GetStateByIdAsync(int id);
        Task<IEnumerable<StateDto>> GetAllStatesAsync();
        Task<bool> ToggleStateStatusAsync(int id, bool isActive);
    }
}
