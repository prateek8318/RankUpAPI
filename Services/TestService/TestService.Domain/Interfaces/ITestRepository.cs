using TestService.Domain.Entities;

namespace TestService.Domain.Interfaces
{
    public interface ITestRepository : IRepository<Test>
    {
        Task<IEnumerable<Test>> GetByExamAndPracticeModeAsync(int examId, int practiceModeId);
        Task<IEnumerable<Test>> GetByExamAndPracticeModeWithFiltersAsync(
            int examId, 
            int practiceModeId, 
            int? seriesId = null, 
            int? subjectId = null, 
            int? year = null);
        Task<Test?> GetByIdWithQuestionsAsync(int id);
        Task<IEnumerable<Test>> GetActiveTestsAsync();
        Task<(IEnumerable<UserAvailableTest> Items, int TotalCount)> GetAvailableTestsForUserAsync(int userId, int examId, int? practiceModeId, int? subjectId, int? seriesId, int? year, int pageNumber, int pageSize);
        Task<bool> MapTestToPlanAsync(int testId, int subscriptionPlanId);
        Task<IReadOnlyList<LeaderboardEntry>> GetLeaderboardAsync(int testId, int top = 20);
    }
}
