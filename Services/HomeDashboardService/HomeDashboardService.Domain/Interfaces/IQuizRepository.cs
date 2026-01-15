using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Domain.Interfaces
{
    public interface IQuizRepository : IRepository<Quiz>
    {
        Task<IEnumerable<Quiz>> GetByChapterIdAsync(int chapterId);
        Task<IEnumerable<Quiz>> GetActiveQuizzesAsync();
        Task<IEnumerable<Quiz>> GetByTypeAsync(QuizType type);
        Task<IEnumerable<Quiz>> GetTrendingQuizzesAsync(int limit = 10);
        Task<Quiz?> GetByIdWithQuestionsAsync(int id);
    }
}
