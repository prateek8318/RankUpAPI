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
    }
}
