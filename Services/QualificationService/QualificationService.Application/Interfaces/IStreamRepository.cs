using QualificationService.Domain.Entities;

namespace QualificationService.Application.Interfaces
{
    public interface IStreamRepository
    {
        Task<Domain.Entities.Stream?> GetByIdAsync(int id);
        Task<IEnumerable<Domain.Entities.Stream>> GetAllAsync();
        Task<IEnumerable<Domain.Entities.Stream>> GetActiveAsync();
        Task<Domain.Entities.Stream> AddAsync(Domain.Entities.Stream stream);
        Task UpdateAsync(Domain.Entities.Stream stream);
        Task DeleteAsync(Domain.Entities.Stream stream);
        Task<int> SaveChangesAsync();
    }
}
