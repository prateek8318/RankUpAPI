using RankUpAPI.DTOs;

namespace RankUpAPI.Services.Interfaces
{
    public interface IQuestionService
    {
        Task<QuestionDto> CreateQuestionAsync(CreateQuestionDto createDto);
        Task<QuestionDto?> UpdateQuestionAsync(int id, UpdateQuestionDto updateDto);
        Task<bool> DeleteQuestionAsync(int id);
        Task<QuestionDto?> GetQuestionByIdAsync(int id);
        Task<IEnumerable<QuestionDto>> GetAllQuestionsAsync();
        Task<IEnumerable<QuestionDto>> GetQuestionsByChapterIdAsync(int chapterId);
        Task<IEnumerable<QuestionDto>> GetQuestionsBySubjectIdAsync(int subjectId);
        Task<IEnumerable<QuestionDto>> GetQuestionsByExamIdAsync(int examId);
        Task<bool> ToggleQuestionStatusAsync(int id, bool isActive);
        Task<BulkUploadQuestionDto> BulkUploadQuestionsAsync(IFormFile file);
    }
}
