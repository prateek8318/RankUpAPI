using StreamEntity = MasterService.Domain.Entities.Stream;
using Common.DTOs;

namespace MasterService.Application.Interfaces
{
    public interface IStreamRepository
    {
        Task<StreamEntity?> GetByIdAsync(int id);
        Task<StreamEntity?> GetByIdLocalizedAsync(int id, string? languageCode);
        Task<IEnumerable<StreamEntity>> GetAllAsync();
        Task<PaginatedResponse<StreamEntity>> GetAllAsync(PaginationRequest pagination);
        Task<IEnumerable<StreamEntity>> GetActiveAsync();
        Task<PaginatedResponse<StreamEntity>> GetActiveAsync(PaginationRequest pagination);
        Task<IEnumerable<StreamEntity>> GetActiveLocalizedAsync(string? languageCode);
        Task<IEnumerable<StreamEntity>> GetActiveByQualificationIdAsync(int qualificationId);
        Task<IEnumerable<StreamEntity>> GetActiveByQualificationIdLocalizedAsync(int qualificationId, string? languageCode);
        Task<StreamEntity> AddAsync(StreamEntity stream, string? namesJson = null);
        Task UpdateAsync(StreamEntity stream, string? namesJson = null);
        Task DeleteAsync(StreamEntity stream);
        Task<bool> SoftDeleteByIdAsync(int id);
        Task<bool> SetActiveAsync(int id, bool isActive);
        Task<int> SaveChangesAsync();
    }
}
