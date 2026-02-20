using StreamEntity = MasterService.Domain.Entities.Stream;

namespace MasterService.Application.Interfaces
{
    public interface IStreamRepository
    {
        Task<StreamEntity?> GetByIdAsync(int id);
        Task<IEnumerable<StreamEntity>> GetAllAsync();
        Task<IEnumerable<StreamEntity>> GetActiveAsync();
        Task<IEnumerable<StreamEntity>> GetActiveByQualificationIdAsync(int qualificationId);
        Task<StreamEntity> AddAsync(StreamEntity stream);
        Task UpdateAsync(StreamEntity stream);
        Task DeleteAsync(StreamEntity stream);
        Task<int> SaveChangesAsync();
    }
}
