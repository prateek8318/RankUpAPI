using TestService.Domain.Entities;

namespace TestService.Domain.Interfaces
{
    public interface ITestSeriesRepository : IRepository<TestSeries>
    {
        Task<IEnumerable<TestSeries>> GetByExamIdAsync(int examId);
        Task<TestSeries?> GetByIdWithTestsAsync(int id);
        Task<IEnumerable<TestSeries>> GetActiveSeriesAsync();
    }
}
