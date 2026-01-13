using RankUpAPI.Models;

namespace RankUpAPI.Repositories.Interfaces
{
    public interface ITestSeriesQuestionRepository : IRepository<TestSeriesQuestion>
    {
        Task<IEnumerable<TestSeriesQuestion>> GetByTestSeriesIdAsync(int testSeriesId);
        Task<IEnumerable<TestSeriesQuestion>> GetByQuestionIdAsync(int questionId);
        Task<TestSeriesQuestion?> GetByTestSeriesAndQuestionIdAsync(int testSeriesId, int questionId);
        Task<int> GetQuestionCountByTestSeriesIdAsync(int testSeriesId);
        Task<int> GetMaxOrderByTestSeriesIdAsync(int testSeriesId);
        Task DeleteByTestSeriesIdAsync(int testSeriesId);
    }
}
