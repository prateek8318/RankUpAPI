using QuizService.Domain.Entities;

namespace QuizService.Application.Interfaces
{
    public interface ITestSeriesRepository
    {
        Task<TestSeries?> GetByIdAsync(int id);
        Task<IEnumerable<TestSeries>> GetAllAsync();
        Task<IEnumerable<TestSeries>> GetByExamIdAsync(int examId);
        Task<TestSeries> AddAsync(TestSeries testSeries);
        Task UpdateAsync(TestSeries testSeries);
        Task DeleteAsync(TestSeries testSeries);
        Task<int> SaveChangesAsync();
    }
}
