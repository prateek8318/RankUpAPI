using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Domain.Interfaces
{
    public interface IQuestionRepository : IRepository<Question>
    {
        Task<IEnumerable<Question>> GetByQuizIdAsync(int quizId);
        Task<Question?> GetByIdWithOptionsAsync(int id);
        Task<int> BulkInsertAsync(IEnumerable<Question> questions);
    }
}
