using RankUpAPI.Models;

namespace RankUpAPI.Repositories.Interfaces
{
    public interface IChapterRepository : IRepository<Chapter>
    {
        Task<Chapter?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Chapter>> GetActiveAsync();
        Task<IEnumerable<Chapter>> GetBySubjectIdAsync(int subjectId);
    }
}
