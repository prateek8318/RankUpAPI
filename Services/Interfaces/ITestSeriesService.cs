using RankUpAPI.DTOs;

namespace RankUpAPI.Services.Interfaces
{
    public interface ITestSeriesService
    {
        Task<TestSeriesDto> CreateTestSeriesAsync(CreateTestSeriesDto createDto);
        Task<TestSeriesDto?> UpdateTestSeriesAsync(int id, UpdateTestSeriesDto updateDto);
        Task<bool> DeleteTestSeriesAsync(int id);
        Task<TestSeriesDto?> GetTestSeriesByIdAsync(int id);
        Task<IEnumerable<TestSeriesDto>> GetAllTestSeriesAsync();
        Task<IEnumerable<TestSeriesDto>> GetTestSeriesByExamIdAsync(int examId);
        Task<bool> ToggleTestSeriesStatusAsync(int id, bool isActive);
        Task<bool> AddQuestionsToTestSeriesAsync(AddQuestionsToTestSeriesDto dto);
        Task<bool> RemoveQuestionFromTestSeriesAsync(int testSeriesId, int questionId);
    }
}
