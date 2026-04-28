using QuestionService.Application.DTOs;

namespace QuestionService.Application.Interfaces
{
    public interface IMockTestRepository
    {
        // Mock test CRUD operations
        Task<MockTestDto> CreateAsync(CreateMockTestDto dto);
        Task<MockTestDto?> GetByIdAsync(int id);
        Task<MockTestDto> UpdateAsync(UpdateMockTestDto dto);
        Task<bool> DeleteAsync(int id);
        Task<(List<MockTestDto> MockTests, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, int? examId = null, int? subjectId = null, bool? isActive = null, MockTestListRequestDto? request = null);
        
        // Question management
        Task<bool> AddQuestionAsync(int mockTestId, int questionId, int questionNumber, decimal marks, decimal negativeMarks);
        Task<bool> RemoveQuestionAsync(int mockTestId, int questionId);
        Task<bool> UpdateQuestionAsync(int mockTestId, int questionId, int questionNumber, decimal marks, decimal negativeMarks);
        Task<List<MockTestQuestionDto>> GetQuestionsAsync(int mockTestId);
        Task<MockTestQuestionDto?> GetQuestionByIdAsync(int mockTestId, int questionId);
        
        // User specific operations
        Task<List<MockTestListDto>> GetForUserAsync(int userId, int pageNumber, int pageSize, int? examId = null, int? subjectId = null);
        Task<MockTestDetailDto?> GetDetailForUserAsync(int userId, int mockTestId);
        Task<UserSubscriptionDto?> GetUserSubscriptionAsync(int userId);
        
        // Session management
        Task<MockTestSessionDto> CreateSessionAsync(StartMockTestDto dto);
        Task<MockTestSessionDto?> GetSessionAsync(int sessionId, int userId);
        Task<bool> SaveSessionAnswerAsync(int sessionId, int userId, SaveMockTestAnswerDto answer);
        Task<MockTestAttemptDto> SubmitSessionAsync(int sessionId, int userId, List<QuizAnswerRequestDto> answers);
        Task<MockTestAttemptDto> SubmitSessionAsync(int sessionId, int userId);
        
        // Statistics
        Task<MockTestStatisticsDto> GetMockTestStatisticsAsync(int? examId = null, int? subjectId = null);
        Task<object> GetStatisticsLegacyAsync(int? examId = null, int? subjectId = null);
        Task<List<object>> GetUserPerformanceAsync(int userId, int? examId = null);
        
        // Exam-specific subjects
        Task<IEnumerable<SubjectListDto>> GetSubjectsForExamAsync(int examId);
    }
}
