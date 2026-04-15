using TestService.Domain.Entities;

namespace TestService.Domain.Interfaces
{
    public interface IAttemptAnswerRepository
    {
        Task SaveAsync(int attemptId, int questionId, string? answer, bool isMarkedForReview, bool isAnswered);
        Task<IReadOnlyList<AttemptAnswer>> GetByAttemptIdAsync(int attemptId);
    }
}

