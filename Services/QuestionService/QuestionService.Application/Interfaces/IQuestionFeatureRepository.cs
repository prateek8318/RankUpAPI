using Microsoft.AspNetCore.Http;
using QuestionService.Application.DTOs;
using QuestionService.Domain.Entities;

namespace QuestionService.Application.Interfaces
{
    public interface IQuestionFeatureRepository
    {
        Task<string> UploadQuestionImageAsync(IFormFile image, string imageType, int? questionId, string? languageCode);
        Task<BulkUploadResultDto> BulkCreateQuestionsAsync(BulkQuestionUploadRequestDto dto);
        Task<object> GetBulkUploadStatusAsync(int batchId);
        Task<List<object>> ParseBulkUploadFileAsync(string filePath);
        Task<object> StartQuizAsync(QuizStartRequestDto dto);
        Task<object> GetQuizSessionAsync(int sessionId, int userId);
        Task<bool> SaveQuizAnswerAsync(QuizAnswerRequestDto dto);
        Task<object> SubmitQuizAsync(QuizSubmitRequestDto dto);
        Task<IEnumerable<object>> GetSubjectsAsync();
        Task<IEnumerable<object>> GetExamsAsync(int? subjectId = null);
        Task<bool> IsTopicMappedToExamSubjectAsync(int topicId, int examId, int subjectId);
        Task<bool> IsSubjectMappedToExamAsync(int subjectId, int examId);
        
        // Exam Integration Methods
        Task<ExamNameDto> GetExamDetailsAsync(int examId);
        Task<IEnumerable<ExamTypeDto>> GetExamTypesAsync();
        Task<IEnumerable<ExamNameDto>> GetExamNamesByTypeAsync(string examType);
        Task<IEnumerable<ExamNameDto>> GetAllExamNamesAsync();
    }
}
