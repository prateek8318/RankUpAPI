using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Domain.Interfaces
{
    public interface IChapterRepository : IRepository<Chapter>
    {
        Task<IEnumerable<Chapter>> GetBySubjectIdAsync(int subjectId);
        Task<Chapter?> GetByIdWithQuizzesAsync(int id);
    }
}
