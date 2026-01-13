using QuizService.Domain.Entities;

namespace QuizService.Application.Interfaces
{
    public interface IChapterRepository
    {
        Task<Chapter?> GetByIdAsync(int id);
        Task<IEnumerable<Chapter>> GetAllAsync();
        Task<IEnumerable<Chapter>> GetBySubjectIdAsync(int subjectId);
        Task<Chapter> AddAsync(Chapter chapter);
        Task UpdateAsync(Chapter chapter);
        Task DeleteAsync(Chapter chapter);
        Task<int> SaveChangesAsync();
    }
}
