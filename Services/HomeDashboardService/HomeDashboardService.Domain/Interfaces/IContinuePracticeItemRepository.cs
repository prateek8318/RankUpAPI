using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Domain.Interfaces
{
    public interface IContinuePracticeItemRepository : IRepository<ContinuePracticeItem>
    {
        Task<IEnumerable<ContinuePracticeItem>> GetUserActiveItemsAsync(int userId);
        Task<ContinuePracticeItem?> GetByQuizAttemptIdAsync(int userId, int quizAttemptId);
    }
}
