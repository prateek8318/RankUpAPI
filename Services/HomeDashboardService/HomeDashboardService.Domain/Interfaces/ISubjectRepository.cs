using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Domain.Interfaces
{
    public interface ISubjectRepository : IRepository<Subject>
    {
        Task<IEnumerable<Subject>> GetByExamIdAsync(int examId);
        Task<Subject?> GetByIdWithChaptersAsync(int id);
    }
}
