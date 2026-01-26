using TestService.Domain.Entities;

namespace TestService.Domain.Interfaces
{
    public interface ITestQuestionRepository : IRepository<TestQuestion>
    {
        Task<IEnumerable<TestQuestion>> GetByTestIdAsync(int testId);
        Task<IEnumerable<TestQuestion>> GetByQuestionIdAsync(int questionId);
    }
}
