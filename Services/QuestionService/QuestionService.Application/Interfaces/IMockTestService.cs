using QuestionService.Application.DTOs;

namespace QuestionService.Application.Interfaces
{
    public interface IMockTestService
    {
        // Admin operations
        Task<MockTestDto> CreateMockTestAsync(CreateMockTestDto dto);
        Task<MockTestDto> UpdateMockTestAsync(UpdateMockTestDto dto);
        Task<bool> DeleteMockTestAsync(int id);
        Task<MockTestDto?> GetMockTestByIdAsync(int id);
        Task<MockTestListResponseDto> GetMockTestsAsync(int pageNumber = 1, int pageSize = 20, int? examId = null, int? subjectId = null, bool? isActive = null);
        
        // User operations
        Task<MockTestListResponseDto> GetMockTestsForUserAsync(int userId, int pageNumber = 1, int pageSize = 20, int? examId = null, int? subjectId = null);
        Task<MockTestDetailDto?> GetMockTestDetailForUserAsync(int userId, int mockTestId);
        Task<MockTestAccessResponseDto> CheckMockTestAccessAsync(int userId, int mockTestId);
        Task<MockTestSessionDto> StartMockTestAsync(StartMockTestDto dto);
        Task<MockTestSessionDto?> GetMockTestSessionAsync(int sessionId, int userId);
        Task<MockTestAttemptDto> SubmitMockTestAsync(int sessionId, int userId, List<QuizAnswerRequestDto> answers);
        
        // Question management for mock tests
        Task<bool> AddQuestionToMockTestAsync(int mockTestId, int questionId, int questionNumber, decimal marks, decimal negativeMarks);
        Task<bool> RemoveQuestionFromMockTestAsync(int mockTestId, int questionId);
        Task<bool> UpdateQuestionInMockTestAsync(int mockTestId, int questionId, int questionNumber, decimal marks, decimal negativeMarks);
        Task<List<MockTestQuestionDto>> GetMockTestQuestionsAsync(int mockTestId);
        
        // Statistics and analytics
        Task<object> GetMockTestStatisticsAsync(int? examId = null, int? subjectId = null);
        Task<List<object>> GetMockTestPerformanceAsync(int userId, int? examId = null);
    }
}
