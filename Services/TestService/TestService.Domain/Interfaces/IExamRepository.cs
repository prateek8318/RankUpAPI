using TestService.Domain.Entities;

namespace TestService.Domain.Interfaces
{
    public interface IExamRepository : IRepository<ExamMaster>
    {
        Task<IEnumerable<ExamMaster>> GetActiveExamsAsync();
        Task<ExamMaster?> GetByIdWithSubjectsAsync(int id);
    }
}
