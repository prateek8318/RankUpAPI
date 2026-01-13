using RankUpAPI.Models;

namespace RankUpAPI.Repositories.Interfaces
{
    public interface IExamQualificationRepository : IRepository<ExamQualification>
    {
        Task<IEnumerable<ExamQualification>> GetByExamIdAsync(int examId);
        Task<IEnumerable<ExamQualification>> GetByQualificationIdAsync(int qualificationId);
        Task<bool> ExistsAsync(int examId, int qualificationId);
        Task<bool> HasExamsForQualificationAsync(int qualificationId);
        Task DeleteByExamIdAsync(int examId);
    }
}
