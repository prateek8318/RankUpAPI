using TestService.Domain.Entities;

namespace TestService.Domain.Interfaces
{
    public interface IPracticeModeRepository : IRepository<PracticeMode>
    {
        Task<PracticeMode?> GetByIdAsync(int id);
        Task<IEnumerable<PracticeMode>> GetActiveModesAsync();
    }
}
