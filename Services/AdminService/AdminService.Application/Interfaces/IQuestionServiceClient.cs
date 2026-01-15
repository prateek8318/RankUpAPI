namespace AdminService.Application.Interfaces
{
    public interface IQuestionServiceClient
    {
        Task<object?> GetQuestionsByQuizIdAsync(int quizId);
        Task<object?> GetQuestionByIdAsync(int id);
        Task<object?> CreateQuestionAsync(object createDto);
        Task<object?> UpdateQuestionAsync(int id, object updateDto);
        Task<bool> DeleteQuestionAsync(int id);
        Task<object?> BulkUploadQuestionsAsync(int quizId, Stream fileStream, string fileName);
    }
}
