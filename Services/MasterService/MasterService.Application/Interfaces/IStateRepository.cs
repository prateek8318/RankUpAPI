using MasterService.Domain.Entities;

namespace MasterService.Application.Interfaces
{
    public interface IStateRepository
    {
        Task<State?> GetByIdAsync(int id);
        Task<State?> GetByIdLocalizedAsync(int id, string? languageCode);
        Task<IEnumerable<State>> GetAllAsync();
        Task<IEnumerable<State>> GetActiveAsync();
        Task<IEnumerable<State>> GetActiveLocalizedAsync(string? languageCode);
        Task<IEnumerable<State>> GetActiveByCountryCodeAsync(string countryCode);
        Task<IEnumerable<State>> GetActiveByCountryCodeLocalizedAsync(string countryCode, string? languageCode);
        Task<State> AddAsync(State state, string? namesJson = null);
        Task UpdateAsync(State state, string? namesJson = null);
        Task DeleteAsync(State state);
        Task<bool> SoftDeleteByIdAsync(int id);
        Task<bool> SetActiveAsync(int id, bool isActive);
        Task<int> SaveChangesAsync();
        Task<IEnumerable<State>> GetStatesWithEmptyNamesAsync();
    }
}
