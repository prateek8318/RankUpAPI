using RankUpAPI.Models;

namespace RankUpAPI.Repositories.Interfaces
{
    public interface ISubjectRepository : IRepository<Subject>
    {
        Task<Subject?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Subject>> GetActiveAsync();
        Task<IEnumerable<Subject>> GetByExamIdAsync(int examId);
    }
}
