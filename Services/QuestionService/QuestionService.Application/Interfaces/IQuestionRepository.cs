using QuestionService.Domain.Entities;

namespace QuestionService.Application.Interfaces
{
    public interface IQuestionRepository
    {
        Task<Question?> GetByIdAsync(int id);
        Task<IEnumerable<Question>> GetAllAsync();
        Task<IEnumerable<Question>> GetByChapterIdAsync(int chapterId);
        Task<Question> AddAsync(Question question);
        Task UpdateAsync(Question question);
        Task DeleteAsync(Question question);
        Task<int> SaveChangesAsync();
    }
}
