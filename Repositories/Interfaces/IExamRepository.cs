using RankUpAPI.Models;

namespace RankUpAPI.Repositories.Interfaces
{
    public interface IExamRepository : IRepository<Exam>
    {
        Task<Exam?> GetByIdWithQualificationsAsync(int id);
        Task<IEnumerable<Exam>> GetActiveAsync();
        Task<IEnumerable<Exam>> GetByQualificationIdAsync(int qualificationId);
    }
}
