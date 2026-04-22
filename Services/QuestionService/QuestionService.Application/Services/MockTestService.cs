using AutoMapper;
using Microsoft.Extensions.Logging;
using QuestionService.Application.DTOs;
using QuestionService.Application.Interfaces;

namespace QuestionService.Application.Services
{
    public class MockTestService : IMockTestService
    {
        private readonly IMockTestRepository _mockTestRepository;
        private readonly IQuestionFeatureRepository _questionFeatureRepository;
        private readonly ILogger<MockTestService> _logger;

        public MockTestService(
            IMockTestRepository mockTestRepository,
            IQuestionFeatureRepository questionFeatureRepository,
            ILogger<MockTestService> logger)
        {
            _mockTestRepository = mockTestRepository;
            _questionFeatureRepository = questionFeatureRepository;
            _logger = logger;
        }

        // Admin Operations
        public async Task<MockTestDto> CreateMockTestAsync(CreateMockTestDto dto)
        {
            try
            {
                _logger.LogInformation("Creating new {MockTestType}: {Name} for exam: {ExamId}", dto.MockTestType, dto.Name, dto.ExamId);

                // Validate based on mock test type
                ValidateMockTestType(dto);

                // Get exam details to validate and populate exam-related data
                var examDetails = await _questionFeatureRepository.GetExamDetailsAsync(dto.ExamId);
                if (examDetails == null)
                {
                    throw new ArgumentException($"Exam with ID {dto.ExamId} not found");
                }

                // Set default values based on mock test type
                SetDefaultsForMockTestType(dto, examDetails);

                // Create the mock test
                var mockTest = await _mockTestRepository.CreateAsync(dto);

                // If specific questions are provided, add them to the mock test
                if (dto.QuestionIds != null && dto.QuestionIds.Any())
                {
                    for (int i = 0; i < dto.QuestionIds.Count; i++)
                    {
                        var questionId = dto.QuestionIds[i];
                        var marks = examDetails.MarksPerQuestion;
                        var negativeMarks = examDetails.HasNegativeMarking ? examDetails.NegativeMarkingValue ?? 0 : 0;
                        
                        await _mockTestRepository.AddQuestionAsync(mockTest.Id, questionId, i + 1, marks, negativeMarks);
                    }
                }
                else
                {
                    // Auto-select questions based on exam criteria and mock test type
                    await AutoSelectQuestionsForMockTestType(mockTest.Id, dto, examDetails);
                }

                _logger.LogInformation("{MockTestType} created successfully: {MockTestId}", dto.MockTestType, mockTest.Id);
                return mockTest;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating {MockTestType}: {Name}", dto.MockTestType, dto.Name);
                throw;
            }
        }

        public async Task<MockTestDto> UpdateMockTestAsync(UpdateMockTestDto dto)
        {
            try
            {
                _logger.LogInformation("Updating mock test: {Id}", dto.Id);

                var existingMockTest = await _mockTestRepository.GetByIdAsync(dto.Id);
                if (existingMockTest == null)
                {
                    throw new KeyNotFoundException($"Mock test with ID {dto.Id} not found");
                }

                return await _mockTestRepository.UpdateAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating mock test: {Id}", dto.Id);
                throw;
            }
        }

        public async Task<bool> DeleteMockTestAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting mock test: {Id}", id);

                return await _mockTestRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting mock test: {Id}", id);
                throw;
            }
        }

        public async Task<MockTestDto?> GetMockTestByIdAsync(int id)
        {
            try
            {
                return await _mockTestRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock test: {Id}", id);
                throw;
            }
        }

        public async Task<MockTestListResponseDto> GetMockTestsAsync(int pageNumber = 1, int pageSize = 20, int? examId = null, int? subjectId = null, bool? isActive = null)
        {
            try
            {
                var (mockTests, totalCount) = await _mockTestRepository.GetPagedAsync(pageNumber, pageSize, examId, subjectId, isActive);

                return new MockTestListResponseDto
                {
                    MockTests = mockTests.Select(mt => new MockTestListDto
                    {
                        Id = mt.Id,
                        Name = mt.Name,
                        Description = mt.Description,
                        MockTestType = mt.MockTestType,
                        ExamId = mt.ExamId,
                        ExamName = mt.ExamName,
                        ExamType = mt.ExamType,
                        SubjectId = mt.SubjectId,
                        SubjectName = mt.SubjectName,
                        TopicId = mt.TopicId,
                        TopicName = mt.TopicName,
                        DurationInMinutes = mt.DurationInMinutes,
                        TotalQuestions = mt.TotalQuestions,
                        TotalMarks = mt.TotalMarks,
                        HasNegativeMarking = mt.HasNegativeMarking,
                        AccessType = mt.AccessType,
                        SubscriptionPlanId = mt.SubscriptionPlanId,
                        CreatedAt = mt.CreatedAt
                    }).ToList(),
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock tests");
                throw;
            }
        }

        // User Operations
        public async Task<MockTestListResponseDto> GetMockTestsForUserAsync(int userId, int pageNumber = 1, int pageSize = 20, int? examId = null, int? subjectId = null)
        {
            try
            {
                var mockTests = await _mockTestRepository.GetForUserAsync(userId, pageNumber, pageSize, examId, subjectId);
                var userSubscription = await _mockTestRepository.GetUserSubscriptionAsync(userId);

                return new MockTestListResponseDto
                {
                    MockTests = mockTests,
                    TotalCount = mockTests.Count,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)mockTests.Count / pageSize),
                    UserSubscription = userSubscription
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock tests for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<MockTestDetailDto?> GetMockTestDetailForUserAsync(int userId, int mockTestId)
        {
            try
            {
                return await _mockTestRepository.GetDetailForUserAsync(userId, mockTestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock test detail for user: {UserId}, mock test: {MockTestId}", userId, mockTestId);
                throw;
            }
        }

        public async Task<MockTestAccessResponseDto> CheckMockTestAccessAsync(int userId, int mockTestId)
        {
            try
            {
                var mockTest = await _mockTestRepository.GetByIdAsync(mockTestId);
                if (mockTest == null)
                {
                    return new MockTestAccessResponseDto
                    {
                        CanAccess = false,
                        Reason = "Mock test not found"
                    };
                }

                var userSubscription = await _mockTestRepository.GetUserSubscriptionAsync(userId);

                // Check access based on subscription
                if (mockTest.AccessType == "Free")
                {
                    return new MockTestAccessResponseDto
                    {
                        CanAccess = true,
                        Reason = "Free mock test",
                        UserSubscription = userSubscription,
                        MockTest = mockTest
                    };
                }

                // Check if user has active subscription
                if (userSubscription == null || !userSubscription.IsActive || userSubscription.IsExpired)
                {
                    return new MockTestAccessResponseDto
                    {
                        CanAccess = false,
                        Reason = "Active subscription required",
                        UserSubscription = userSubscription,
                        MockTest = mockTest
                    };
                }

                // Check if subscription plan matches mock test requirements
                if (mockTest.SubscriptionPlanId.HasValue && mockTest.SubscriptionPlanId != userSubscription.SubscriptionPlanId)
                {
                    return new MockTestAccessResponseDto
                    {
                        CanAccess = false,
                        Reason = "Higher subscription plan required",
                        UserSubscription = userSubscription,
                        MockTest = mockTest
                    };
                }

                return new MockTestAccessResponseDto
                {
                    CanAccess = true,
                    Reason = "Access granted",
                    UserSubscription = userSubscription,
                    MockTest = mockTest
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking mock test access for user: {UserId}, mock test: {MockTestId}", userId, mockTestId);
                throw;
            }
        }

        public async Task<MockTestSessionDto> StartMockTestAsync(StartMockTestDto dto)
        {
            try
            {
                _logger.LogInformation("Starting mock test: {MockTestId} for user: {UserId}", dto.MockTestId, dto.UserId);

                // Check access first
                var accessCheck = await CheckMockTestAccessAsync(dto.UserId, dto.MockTestId);
                if (!accessCheck.CanAccess)
                {
                    throw new UnauthorizedAccessException(accessCheck.Reason);
                }

                return await _mockTestRepository.CreateSessionAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting mock test: {MockTestId} for user: {UserId}", dto.MockTestId, dto.UserId);
                throw;
            }
        }

        public async Task<MockTestSessionDto?> GetMockTestSessionAsync(int sessionId, int userId)
        {
            try
            {
                return await _mockTestRepository.GetSessionAsync(sessionId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock test session: {SessionId}", sessionId);
                throw;
            }
        }

        public async Task<MockTestAttemptDto> SubmitMockTestAsync(int sessionId, int userId, List<QuizAnswerRequestDto> answers)
        {
            try
            {
                _logger.LogInformation("Submitting mock test session: {SessionId} for user: {UserId}", sessionId, userId);

                return await _mockTestRepository.SubmitSessionAsync(sessionId, userId, answers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting mock test session: {SessionId}", sessionId);
                throw;
            }
        }

        // Question Management
        public async Task<bool> AddQuestionToMockTestAsync(int mockTestId, int questionId, int questionNumber, decimal marks, decimal negativeMarks)
        {
            try
            {
                return await _mockTestRepository.AddQuestionAsync(mockTestId, questionId, questionNumber, marks, negativeMarks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding question to mock test: {MockTestId}", mockTestId);
                throw;
            }
        }

        public async Task<bool> RemoveQuestionFromMockTestAsync(int mockTestId, int questionId)
        {
            try
            {
                return await _mockTestRepository.RemoveQuestionAsync(mockTestId, questionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing question from mock test: {MockTestId}", mockTestId);
                throw;
            }
        }

        public async Task<bool> UpdateQuestionInMockTestAsync(int mockTestId, int questionId, int questionNumber, decimal marks, decimal negativeMarks)
        {
            try
            {
                return await _mockTestRepository.UpdateQuestionAsync(mockTestId, questionId, questionNumber, marks, negativeMarks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating question in mock test: {MockTestId}", mockTestId);
                throw;
            }
        }

        public async Task<List<MockTestQuestionDto>> GetMockTestQuestionsAsync(int mockTestId)
        {
            try
            {
                return await _mockTestRepository.GetQuestionsAsync(mockTestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock test questions: {MockTestId}", mockTestId);
                throw;
            }
        }

        // Statistics
        public async Task<object> GetMockTestStatisticsAsync(int? examId = null, int? subjectId = null)
        {
            try
            {
                return await _mockTestRepository.GetStatisticsAsync(examId, subjectId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock test statistics");
                throw;
            }
        }

        public async Task<List<object>> GetMockTestPerformanceAsync(int userId, int? examId = null)
        {
            try
            {
                return await _mockTestRepository.GetUserPerformanceAsync(userId, examId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock test performance for user: {UserId}", userId);
                throw;
            }
        }

        // Private helper methods
        private void ValidateMockTestType(CreateMockTestDto dto)
        {
            switch (dto.MockTestType)
            {
                case MockTestType.MockTest:
                    if (!dto.SubjectId.HasValue)
                        throw new ArgumentException("SubjectId is required for Mock Test");
                    break;

                case MockTestType.TestSeries:
                    if (dto.DurationInMinutes < 60)
                        throw new ArgumentException("Test Series should be at least 60 minutes");
                    if (dto.TotalQuestions < 50)
                        throw new ArgumentException("Test Series should have at least 50 questions");
                    break;

                case MockTestType.DeepPractice:
                    if (!dto.SubjectId.HasValue)
                        throw new ArgumentException("SubjectId is required for Deep Practice");
                    if (!dto.TopicId.HasValue)
                        throw new ArgumentException("TopicId is required for Deep Practice");
                    if (string.IsNullOrEmpty(dto.Difficulty))
                        throw new ArgumentException("Difficulty is required for Deep Practice");
                    if (!new[] { "Easy", "Medium", "Hard" }.Contains(dto.Difficulty))
                        throw new ArgumentException("Difficulty must be Easy, Medium, or Hard");
                    break;

                case MockTestType.PreviousYear:
                    if (!dto.Year.HasValue)
                        throw new ArgumentException("Year is required for Previous Year papers");
                    if (dto.Year < 2000 || dto.Year > DateTime.Now.Year)
                        throw new ArgumentException("Year must be between 2000 and current year");
                    break;

                default:
                    throw new ArgumentException("Invalid mock test type");
            }
        }

        private void SetDefaultsForMockTestType(CreateMockTestDto dto, ExamNameDto examDetails)
        {
            switch (dto.MockTestType)
            {
                case MockTestType.MockTest:
                    dto.DurationInMinutes = dto.DurationInMinutes > 0 ? dto.DurationInMinutes : 30;
                    dto.TotalQuestions = dto.TotalQuestions > 0 ? dto.TotalQuestions : 25;
                    break;

                case MockTestType.TestSeries:
                    dto.DurationInMinutes = dto.DurationInMinutes > 0 ? dto.DurationInMinutes : 90;
                    dto.TotalQuestions = dto.TotalQuestions > 0 ? dto.TotalQuestions : 100;
                    dto.PaperCode = dto.PaperCode ?? GeneratePaperCode(examDetails.ExamType);
                    break;

                case MockTestType.DeepPractice:
                    dto.DurationInMinutes = dto.DurationInMinutes > 0 ? dto.DurationInMinutes : 15;
                    dto.TotalQuestions = dto.TotalQuestions > 0 ? dto.TotalQuestions : 20;
                    break;

                case MockTestType.PreviousYear:
                    dto.DurationInMinutes = dto.DurationInMinutes > 0 ? dto.DurationInMinutes : 90;
                    dto.TotalQuestions = dto.TotalQuestions > 0 ? dto.TotalQuestions : 100;
                    dto.PaperCode = dto.PaperCode ?? GeneratePreviousYearCode(examDetails.ExamType, dto.Year!.Value);
                    break;
            }

            // Set default marks based on exam details
            if (dto.TotalMarks == 0)
            {
                dto.TotalMarks = dto.TotalQuestions * examDetails.MarksPerQuestion;
            }

            if (dto.PassingMarks == 0)
            {
                dto.PassingMarks = dto.TotalMarks * 0.35m; // 35% passing
            }
        }

        private async Task AutoSelectQuestionsForMockTestType(int mockTestId, CreateMockTestDto dto, ExamNameDto examDetails)
        {
            try
            {
                IEnumerable<object> questions = new List<object>();

                switch (dto.MockTestType)
                {
                    case MockTestType.MockTest:
                        // Get questions for specific subject
                        questions = new List<object>(); // TODO: Implement question retrieval logic
                        break;

                    case MockTestType.TestSeries:
                        // Get questions for full exam (all subjects)
                        questions = new List<object>(); // TODO: Implement question retrieval logic
                        break;

                    case MockTestType.DeepPractice:
                        // Get questions for specific topic
                        questions = new List<object>(); // TODO: Implement question retrieval logic
                        break;

                    case MockTestType.PreviousYear:
                        // Get questions for exam (can be filtered by year if needed)
                        questions = new List<object>(); // TODO: Implement question retrieval logic
                        break;
                }

                var questionList = questions.ToList();
                
                if (!questionList.Any())
                {
                    _logger.LogWarning("No questions found for {MockTestType}: {ExamId}", dto.MockTestType, dto.ExamId);
                    return;
                }

                // Select questions based on difficulty for Deep Practice
                var selectedQuestions = dto.MockTestType == MockTestType.DeepPractice
                    ? SelectQuestionsByDifficulty(questionList, dto.Difficulty!, dto.TotalQuestions)
                    : SelectRandomQuestions(questionList, dto.TotalQuestions);

                for (int i = 0; i < selectedQuestions.Count; i++)
                {
                    var question = selectedQuestions[i];
                    var marks = examDetails.MarksPerQuestion;
                    var negativeMarks = examDetails.HasNegativeMarking ? examDetails.NegativeMarkingValue ?? 0 : 0;
                    
                    // Get question ID using reflection since question is of type object
                    var questionId = question.GetType().GetProperty("Id")?.GetValue(question) ?? 0;
                    
                    await _mockTestRepository.AddQuestionAsync(mockTestId, (int)questionId, i + 1, marks, negativeMarks);
                }

                _logger.LogInformation("Auto-selected {Count} questions for {MockTestType}: {MockTestId}", selectedQuestions.Count, dto.MockTestType, mockTestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error auto-selecting questions for {MockTestType}: {MockTestId}", dto.MockTestType, mockTestId);
                throw;
            }
        }

        private List<object> SelectRandomQuestions(List<object> questions, int count)
        {
            var random = new Random();
            return questions.OrderBy(x => random.Next()).Take(Math.Min(count, questions.Count)).ToList();
        }

        private List<object> SelectQuestionsByDifficulty(List<object> questions, string difficulty, int count)
        {
            // For now, random selection (can be enhanced with actual difficulty filtering)
            var random = new Random();
            return questions.OrderBy(x => random.Next()).Take(Math.Min(count, questions.Count)).ToList();
        }

        private string GeneratePaperCode(string examType)
        {
            var random = new Random();
            return $"{examType.ToUpper()}-{DateTime.Now.Year}-{random.Next(1000, 9999)}";
        }

        private string GeneratePreviousYearCode(string examType, int year)
        {
            return $"{examType.ToUpper()}-{year}-ACT";
        }
    }
}
