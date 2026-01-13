using RankUpAPI.Models;

namespace RankUpAPI.Repositories.Interfaces
{
    public interface ITestSeriesRepository : IRepository<TestSeries>
    {
        Task<TestSeries?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<TestSeries>> GetActiveAsync();
        Task<IEnumerable<TestSeries>> GetByExamIdAsync(int examId);
    }
}
