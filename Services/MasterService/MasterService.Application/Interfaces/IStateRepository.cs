using MasterService.Domain.Entities;

namespace MasterService.Application.Interfaces
{
    public interface IStateRepository
    {
        Task<State?> GetByIdAsync(int id);
        Task<IEnumerable<State>> GetAllAsync();
        Task<IEnumerable<State>> GetActiveAsync();
        Task<IEnumerable<State>> GetActiveByCountryCodeAsync(string countryCode);
        Task<State> AddAsync(State state);
        Task UpdateAsync(State state);
        Task DeleteAsync(State state);
        Task<int> SaveChangesAsync();
        Task<IEnumerable<State>> GetStatesWithEmptyNamesAsync();
    }
}
