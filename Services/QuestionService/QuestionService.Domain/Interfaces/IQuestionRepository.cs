using QuestionService.Domain.Entities;

namespace QuestionService.Domain.Interfaces
{
    public interface IQuestionRepository
    {
        Task<Question?> GetByIdAsync(int id);
        Task<IEnumerable<Question>> GetAllAsync();
        Task<Question?> GetByTransactionIdAsync(string transactionId);
        Task<(IEnumerable<Question> Questions, int TotalCount)> GetQuestionsPagedAsync(int pageNumber, int pageSize, int? examId = null, int? subjectId = null, int? topicId = null, string? difficultyLevel = null, bool? isPublished = null, string languageCode = "en");
        Task<(IEnumerable<Question> Questions, int TotalCount, string? NextCursor, string? PreviousCursor, bool HasNextPage, bool HasPreviousPage)> GetQuestionsCursorAsync(string? cursor, int pageSize, string direction = "next", int? examId = null, int? subjectId = null, int? topicId = null, string? difficultyLevel = null, bool? isPublished = null, string languageCode = "en");
        Task<(int TotalQuestions, int AddedToday, int NegativeMarksCount, int UnpublishedCount)> GetStatisticsAsync();
        Task<Question> CreateQuestionAsync(string questionText, string? optionA, string? optionB, string? optionC, string? optionD, string correctAnswer, decimal marks, int examId, int subjectId, int? topicId = null, string difficultyLevel = "Medium", int createdBy = 0);
        Task<bool> UpdateQuestionAsync(int id, string? questionText, string? optionA, string? optionB, string? optionC, string? optionD, string? correctAnswer, decimal? marks, string? difficultyLevel);
        Task<bool> DeleteQuestionAsync(int id);
        Task<bool> TogglePublishStatusAsync(int questionId, bool isPublished, int reviewedBy);
        Task<IEnumerable<Question>> GetByExamIdAsync(int examId);
        Task<IEnumerable<Question>> GetBySubjectIdAsync(int subjectId);
        Task<IEnumerable<Question>> GetByTopicIdAsync(int topicId);
    }

    public interface ITopicRepository
    {
        Task<Topic?> GetBySubjectIdAsync(int subjectId);
        Task<IEnumerable<Topic>> GetTopicsBySubjectAsync(int subjectId, bool includeInactive = false);
        Task<Topic> CreateTopicAsync(string name, int subjectId, string? description = null, int? parentTopicId = null, int sortOrder = 0);
        Task<bool> UpdateTopicAsync(int id, string? name, string? description, int? parentTopicId, int? sortOrder, bool? isActive);
        Task<bool> DeleteTopicAsync(int id);
        Task<IEnumerable<Topic>> GetHierarchicalTopicsAsync(int subjectId);
    }

    public interface IQuestionBatchRepository
    {
        Task<QuestionBatch> CreateBatchAsync(string batchName, string? description, string fileName, string filePath, int uploadedBy);
        Task<QuestionBatch?> GetBatchByIdAsync(int batchId);
        Task<(IEnumerable<QuestionBatch> Batches, int TotalCount)> GetBatchesPagedAsync(int pageNumber, int pageSize, BatchStatus? status = null);
        Task<bool> UpdateBatchAsync(int batchId, int processedQuestions, int failedQuestions, string? errorMessage, BatchStatus status);
        Task<QuestionError> AddErrorAsync(int batchId, int rowNumber, string errorMessage, string? rawData);
        Task<IEnumerable<QuestionError>> GetBatchErrorsAsync(int batchId);
    }

    public interface IQuestionTranslationRepository
    {
        Task<IEnumerable<QuestionTranslation>> GetByQuestionIdAsync(int questionId);
        Task<QuestionTranslation?> GetByQuestionAndLanguageAsync(int questionId, string languageCode);
        Task<bool> CreateTranslationAsync(QuestionTranslation translation);
        Task<bool> UpdateTranslationAsync(int id, string? questionText, string? optionA, string? optionB, string? optionC, string? optionD, string? explanation);
        Task<bool> DeleteTranslationAsync(int id);
        Task<bool> DeleteTranslationsByQuestionIdAsync(int questionId);
    }
}
