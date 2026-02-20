using MasterService.Domain.Entities;

namespace MasterService.Application.Interfaces
{
    public interface IQualificationRepository
    {
        Task<Qualification?> GetByIdAsync(int id);
        Task<IEnumerable<Qualification>> GetAllAsync();
        Task<IEnumerable<Qualification>> GetActiveAsync();
        Task<IEnumerable<Qualification>> GetActiveByCountryCodeAsync(string countryCode);
        Task<Qualification> AddAsync(Qualification qualification);
        Task UpdateAsync(Qualification qualification);
        Task DeleteAsync(Qualification qualification);
        Task<int> SaveChangesAsync();
    }
}
