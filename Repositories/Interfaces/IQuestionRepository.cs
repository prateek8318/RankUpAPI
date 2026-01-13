using RankUpAPI.Models;

namespace RankUpAPI.Repositories.Interfaces
{
    public interface IQuestionRepository : IRepository<Question>
    {
        Task<Question?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Question>> GetActiveAsync();
        Task<IEnumerable<Question>> GetByChapterIdAsync(int chapterId);
        Task<IEnumerable<Question>> GetBySubjectIdAsync(int subjectId);
        Task<IEnumerable<Question>> GetByExamIdAsync(int examId);
    }
}
