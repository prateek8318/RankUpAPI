using QuestionService.Application.DTOs;

namespace QuestionService.Application.Interfaces
{
    public interface IMockTestService
    {
        // Admin operations
        Task<MockTestDto> CreateMockTestWithImageAsync(MockTestCreateWithImageDto dto);
        Task<MockTestDto> CreateMockTestDraftAsync(MockTestCreateWithImageDto dto);
        Task<MockTestDto> CreateMockTestAsync(CreateMockTestDto dto);
        Task<MockTestDto> UpdateMockTestAsync(UpdateMockTestDto dto);
        Task<bool> DeleteMockTestAsync(int id);
        Task<MockTestDto?> GetMockTestByIdAsync(int id);
        Task<MockTestListResponseDto> GetMockTestsAsync(int pageNumber = 1, int pageSize = 20, int? examId = null, int? subjectId = null, bool? isActive = null, MockTestListRequestDto? request = null);
        Task<MockTestListResponseDto> GetMockTestsListAsync(MockTestListRequestDto request);
        Task<MockTestSummaryDto> GetMockTestSummaryAsync();
        Task<List<ExamListDto>> GetExamsForUserAsync(int userId);
        Task<ExamListDto> GetExamDetailsAsync(int userId, int examId);
        Task<List<SubjectListDto>> GetSubjectsForExamAsync(int userId, int examId);
        Task<SubjectListDto> GetSubjectDetailsAsync(int userId, int examId, int subjectId);
        Task<SubjectMockTestsResponseDto> GetMockTestsForSubjectAsync(int userId, int examId, int subjectId);
        Task<SimpleMockTestDto> GetMockTestDetailsAsync(int userId, int examId, int subjectId, int mockTestId);
        Task<object> StartMockTestAsync(int userId, int examId, int subjectId, int mockTestId);
        Task<ExamListDto> CreateExamAsync(CreateExamDto dto);
        Task<ExamListDto> UpdateExamAsync(int examId, UpdateExamDto dto);
        Task<bool> DeleteExamAsync(int examId);
        Task<SubjectWiseMockTestResponseDto> GetSubjectWiseMockTestsAsync(int userId, MockTestListRequestDto request);
        Task<MockTestFilterOptionsDto> GetMockTestFilterOptionsAsync(int userId);
        Task<MockTestListResponseDto> GetMockTestsForUserListAsync(int userId, MockTestListRequestDto request);
        Task<MockTestListResponseDto> GetMockTestsForUserAsync(int userId, int? examId, int? subjectId, string? testCategory, string? access, string? difficulty);
        Task<MockTestListResponseDto> GetMockTestsForExamAsync(int userId, int examId);
        Task<MockTestListResponseDto> GetMockTestsForSubjectSimpleAsync(int userId, int subjectId);
        Task<MockTestDetailDto?> GetMockTestDetailsSimpleAsync(int userId, int mockTestId);
        Task<MockTestSessionDto> StartMockTestSessionSimpleAsync(int userId, int mockTestId);
        
        // User operations
        Task<MockTestListResponseDto> GetMockTestsForUserAsync(int userId, int pageNumber = 1, int pageSize = 20, int? examId = null, int? subjectId = null);
        Task<MockTestDetailDto?> GetMockTestDetailForUserAsync(int userId, int mockTestId);
        Task<MockTestAccessResponseDto> CheckMockTestAccessAsync(int userId, int mockTestId);
        Task<MockTestSessionDto> StartMockTestAsync(StartMockTestDto dto);
        Task<MockTestSessionDto?> GetMockTestSessionAsync(int sessionId, int userId);
        Task<MockTestAttemptDto?> GetMockTestSessionResultAsync(int sessionId, int userId);
        Task<MockTestSolutionDto?> GetMockTestSolutionAsync(int sessionId, int userId);
        Task<bool> SaveMockTestAnswerAsync(int sessionId, int userId, SaveMockTestAnswerDto answer);
        Task<MockTestQuestionActionResultDto> ReportMockTestQuestionAsync(int sessionId, int userId, ReportMockTestQuestionDto request);
        Task<MockTestQuestionActionResultDto> BookmarkMockTestQuestionAsync(int sessionId, int userId, BookmarkMockTestQuestionDto request);
        Task<MockTestAttemptDto> SubmitMockTestAsync(int sessionId, int userId, List<QuizAnswerRequestDto> answers);
        Task<MockTestAttemptDto> SubmitMockTestAsync(int sessionId, int userId);
        
        // Question management for mock tests
        Task<bool> AddQuestionToMockTestAsync(int mockTestId, int questionId, int questionNumber, decimal marks, decimal negativeMarks);
        Task<bool> RemoveQuestionFromMockTestAsync(int mockTestId, int questionId);
        Task<bool> UpdateQuestionInMockTestAsync(int mockTestId, int questionId, int questionNumber, decimal marks, decimal negativeMarks);
        Task<List<MockTestQuestionDto>> GetMockTestQuestionsAsync(int mockTestId);
        Task<MockTestQuestionDto?> GetQuestionByIdAsync(int mockTestId, int questionId);
        
        // Statistics and analytics
        Task<MockTestStatisticsDto> GetMockTestStatisticsAsync(MockTestStatisticsRequestDto? request = null);
        Task<object> GetMockTestStatisticsLegacyAsync(int? examId = null, int? subjectId = null);
        Task<List<object>> GetMockTestPerformanceAsync(int userId, int? examId = null);
    }
}
