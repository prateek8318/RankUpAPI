using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Domain.Interfaces
{
    public interface IExamRepository : IRepository<Exam>
    {
        Task<IEnumerable<Exam>> GetActiveExamsAsync();
        Task<Exam?> GetByIdWithSubjectsAsync(int id);
    }
}
