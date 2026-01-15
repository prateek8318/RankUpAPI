using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Domain.Interfaces
{
    public interface IQuizAttemptRepository : IRepository<QuizAttempt>
    {
        Task<IEnumerable<QuizAttempt>> GetOngoingByUserIdAsync(int userId);
        Task<IEnumerable<QuizAttempt>> GetRecentByUserIdAsync(int userId, int limit = 10);
        Task<QuizAttempt?> GetByIdWithQuizAsync(int id);
        Task<int> GetTotalAttemptsCountAsync(int quizId);
        Task<IEnumerable<QuizAttempt>> GetCompletedByUserIdAsync(int userId);
    }
}
