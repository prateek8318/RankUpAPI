using MasterService.Domain.Entities;

namespace MasterService.Application.Interfaces
{
    public interface IQualificationRepository
    {
        Task<Qualification?> GetByIdAsync(int id);
        Task<Qualification?> GetByIdLocalizedAsync(int id, string? languageCode);
        Task<IEnumerable<Qualification>> GetAllAsync();
        Task<IEnumerable<Qualification>> GetActiveAsync();
        Task<IEnumerable<Qualification>> GetActiveLocalizedAsync(string? languageCode);
        Task<IEnumerable<Qualification>> GetActiveByCountryCodeAsync(string countryCode);
        Task<IEnumerable<Qualification>> GetActiveByCountryCodeLocalizedAsync(string countryCode, string? languageCode);
        Task<Qualification> AddAsync(Qualification qualification);
        Task UpdateAsync(Qualification qualification);
        Task DeleteAsync(Qualification qualification);
        Task<bool> HardDeleteByIdAsync(int id);
        Task<bool> SoftDeleteByIdAsync(int id);
        Task<bool> SetActiveAsync(int id, bool isActive);
        Task<bool> HasRelatedStreamsAsync(int qualificationId);
        Task<int> SaveChangesAsync();
    }
}
