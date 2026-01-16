using MasterService.Domain.Entities;

namespace MasterService.Application.Interfaces
{
    public interface ILanguageRepository
    {
        Task<Language?> GetByIdAsync(int id);
        Task<IEnumerable<Language>> GetAllAsync();
        Task<IEnumerable<Language>> GetActiveAsync();
        Task<Language> AddAsync(Language language);
        Task UpdateAsync(Language language);
        Task DeleteAsync(Language language);
        Task<int> SaveChangesAsync();
    }
}
