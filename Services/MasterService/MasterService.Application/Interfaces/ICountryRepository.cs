using MasterService.Domain.Entities;

namespace MasterService.Application.Interfaces
{
    public interface ICountryRepository
    {
        Task<IEnumerable<Country>> GetAllAsync();
        Task<Country?> GetByIdAsync(int id);
        Task<Country?> GetByCodeAsync(string code);
        Task<Country> AddAsync(Country country);
        Task<Country> UpdateAsync(Country country);
        Task DeleteAsync(Country country);
        Task<int> SaveChangesAsync();
    }
}
