using ExamService.Domain.Entities;

namespace ExamService.Application.Interfaces
{
    public interface IExamSessionRepository
    {
        Task<ExamSession?> GetByIdAsync(int id);
        Task<ExamSession> AddAsync(ExamSession entity);
        Task<ExamSession> UpdateAsync(ExamSession entity);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ExamSession>> GetAllAsync();
        Task<ExamSession?> GetActiveSessionByUserIdAsync(int userId);
        Task<IEnumerable<ExamSession>> GetSessionsByUserIdAsync(int userId);
        Task<IEnumerable<ExamSession>> GetSessionsByExamIdAsync(int examId);
    }
}
