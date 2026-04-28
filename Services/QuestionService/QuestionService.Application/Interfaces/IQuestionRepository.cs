using QuestionService.Domain.Entities;
using QuestionService.Application.DTOs;

namespace QuestionService.Application.Interfaces
{
    public interface IQuestionRepository
    {
        Task<Question?> GetByIdAsync(int id);
        Task<IEnumerable<Question>> GetAllAsync();
        Task<IEnumerable<Question>> GetByChapterIdAsync(int chapterId);
        Task<Question> AddAsync(Question question);
        Task UpdateAsync(Question question);
        Task DeleteAsync(Question question);
        Task<int> SaveChangesAsync();

        Task<int> CreateTopicAsync(CreateTopicDto dto);
        Task<IEnumerable<TopicDto>> GetTopicsAsync(int? subjectId, int? examId, bool includeInactive);
        Task<int> CreateAdminQuestionAsync(CreateQuestionRequestDto dto);
        Task<bool> UpdateAdminQuestionAsync(UpdateQuestionAdminDto dto);
        Task<QuestionAdminDetailDto?> GetAdminQuestionByIdAsync(int id);
        Task<(IEnumerable<QuestionAdminListItemDto> Items, int TotalCount)> GetAdminQuestionsPagedAsync(QuestionFilterRequestDto filter);
        Task<QuestionDashboardStatsDto> GetDashboardStatsAsync();
        Task<bool> SetPublishStatusAsync(int id, bool isPublished);
        Task<(IEnumerable<QuestionAdminListItemDto> Items, int TotalCount)> GetAllAdminQuestionsAsync(QuestionFilterRequestDto filter);
        Task<IEnumerable<QuestionAdminListItemDto>> GetAllAdminQuestionsGroupedByMockTestAsync(QuestionFilterRequestDto filter);
        Task<int> BulkCreateQuestionsAsync(IEnumerable<CreateQuestionAdminDto> questions);
        Task<bool> UpdateQuestionImageUrlsAsync(int questionId, string? questionImageUrl, string? optionAImageUrl, string? optionBImageUrl, string? optionCImageUrl, string? optionDImageUrl, string? explanationImageUrl);
    }
}
