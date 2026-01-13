using QuizService.Domain.Entities;

namespace QuizService.Application.Interfaces
{
    public interface ISubjectRepository
    {
        Task<Subject?> GetByIdAsync(int id);
        Task<Subject?> GetByIdWithChaptersAsync(int id);
        Task<IEnumerable<Subject>> GetAllAsync();
        Task<IEnumerable<Subject>> GetByExamIdAsync(int examId);
        Task<Subject> AddAsync(Subject subject);
        Task UpdateAsync(Subject subject);
        Task DeleteAsync(Subject subject);
        Task<int> SaveChangesAsync();
    }
}
