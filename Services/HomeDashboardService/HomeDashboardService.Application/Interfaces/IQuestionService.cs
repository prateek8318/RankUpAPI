using HomeDashboardService.Application.DTOs;

namespace HomeDashboardService.Application.Interfaces
{
    public interface IQuestionService
    {
        Task<QuestionDto> CreateQuestionAsync(CreateQuestionDto createQuestionDto);
        Task<QuestionDto?> UpdateQuestionAsync(int id, UpdateQuestionDto updateQuestionDto);
        Task<bool> DeleteQuestionAsync(int id);
        Task<bool> EnableDisableQuestionAsync(int id, bool isActive);
        Task<QuestionDto?> GetQuestionByIdAsync(int id);
        Task<IEnumerable<QuestionDto>> GetQuestionsByQuizIdAsync(int quizId);
        Task<BulkUploadResultDto> BulkUploadQuestionsAsync(int quizId, Stream fileStream, string fileName, int userId);
        Task<BulkUploadLogDto?> GetBulkUploadLogByIdAsync(int id);
        Task<List<BulkUploadErrorDto>> GetBulkUploadErrorsAsync(int bulkUploadLogId);
    }
}
