using MasterService.Domain.Entities;
using Common.DTOs;

namespace MasterService.Application.Interfaces
{
    public interface IQualificationRepository
    {
        Task<Qualification?> GetByIdAsync(int id);
        Task<Qualification?> GetByIdLocalizedAsync(int id, string? languageCode);
        Task<IEnumerable<Qualification>> GetAllAsync();
        Task<PaginatedResponse<Qualification>> GetAllAsync(PaginationRequest pagination);
        Task<IEnumerable<Qualification>> GetActiveAsync();
        Task<PaginatedResponse<Qualification>> GetActiveAsync(PaginationRequest pagination);
        Task<IEnumerable<Qualification>> GetActiveLocalizedAsync(string? languageCode);
        Task<IEnumerable<Qualification>> GetActiveByCountryCodeAsync(string countryCode);
        Task<IEnumerable<Qualification>> GetActiveByCountryCodeLocalizedAsync(string countryCode, string? languageCode);
        Task<Qualification> AddAsync(Qualification qualification, string? namesJson = null);
        Task UpdateAsync(Qualification qualification, string? namesJson = null);
        Task DeleteAsync(Qualification qualification);
        Task<bool> HardDeleteByIdAsync(int id);
        Task<bool> SoftDeleteByIdAsync(int id);
        Task<bool> SetActiveAsync(int id, bool isActive);
        Task<bool> HasRelatedStreamsAsync(int qualificationId);
        Task<int> SaveChangesAsync();
    }
}
