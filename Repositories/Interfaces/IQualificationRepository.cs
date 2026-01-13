using RankUpAPI.Models;

namespace RankUpAPI.Repositories.Interfaces
{
    public interface IQualificationRepository : IRepository<Qualification>
    {
        Task<Qualification?> GetByNameAsync(string name);
        Task<IEnumerable<Qualification>> GetActiveAsync();
    }
}
