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

        public Task<MockTestDto> CreateMockTestWithImageAsync(MockTestCreateWithImageDto dto)
        {
            var createDto = new CreateMockTestDto
            {
                Name = dto.Name,
                Description = dto.Description,
                MockTestType = dto.MockTestType,
                ExamId = dto.ExamId,
                SubjectId = dto.SubjectId,
                TopicId = dto.TopicId,
                DurationInMinutes = dto.DurationInMinutes,
                TotalQuestions = dto.TotalQuestions,
                TotalMarks = dto.TotalMarks,
                PassingMarks = dto.PassingMarks,
                MarksPerQuestion = dto.MarksPerQuestion,
                HasNegativeMarking = dto.HasNegativeMarking,
                NegativeMarkingValue = dto.NegativeMarkingValue,
                SubscriptionPlanId = dto.SubscriptionPlanId,
                AccessType = dto.AccessType,
                AttemptsAllowed = dto.AttemptsAllowed,
                Year = dto.Year,
                Difficulty = dto.Difficulty,
                PaperCode = dto.PaperCode,
                ExamDate = dto.ExamDate,
                PublishDateTime = dto.PublishDateTime,
                ValidTill = dto.ValidTill,
                ShowResultType = dto.ShowResultType,
                CreatedBy = dto.CreatedBy,
                QuestionIds = dto.QuestionIds
            };

            return CreateMockTestAsync(createDto);
        }

        public async Task<MockTestDto> CreateMockTestDraftAsync(MockTestCreateWithImageDto dto)
        {
            try
            {
                _logger.LogInformation("Creating new {MockTestType} draft: {Name} for exam: {ExamId}", dto.MockTestType, dto.Name, dto.ExamId);

                // Validate based on mock test type
                ValidateMockTestType(dto);

                // Get exam details to validate and populate exam-related data
                var examDetails = await _questionFeatureRepository.GetExamDetailsAsync(dto.ExamId);
                if (examDetails == null)
                {
                    throw new ArgumentException($"Exam with ID {dto.ExamId} not found");
                }

                // Set default values based on mock test type and scheduling
                SetDefaultsForMockTestType(dto, examDetails);
                
                // Force status to Draft
                var createDto = new CreateMockTestDto
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    MockTestType = dto.MockTestType,
                    ExamId = dto.ExamId,
                    SubjectId = dto.SubjectId,
                    TopicId = dto.TopicId,
                    DurationInMinutes = dto.DurationInMinutes,
                    TotalQuestions = dto.TotalQuestions,
                    TotalMarks = dto.TotalMarks,
                    PassingMarks = dto.PassingMarks,
                    MarksPerQuestion = dto.MarksPerQuestion,
                    HasNegativeMarking = dto.HasNegativeMarking,
                    NegativeMarkingValue = dto.NegativeMarkingValue,
                    SubscriptionPlanId = dto.SubscriptionPlanId,
                    AccessType = dto.AccessType,
                    AttemptsAllowed = dto.AttemptsAllowed,
                    Year = dto.Year,
                    Difficulty = dto.Difficulty,
                    PaperCode = dto.PaperCode,
                    ExamDate = dto.ExamDate,
                    PublishDateTime = dto.PublishDateTime,
                    ValidTill = dto.ValidTill,
                    ShowResultType = dto.ShowResultType,
                    CreatedBy = dto.CreatedBy,
                    QuestionIds = dto.QuestionIds,
                    Status = MockTestStatus.Draft // Force draft status
                };

                // Create the mock test
                var mockTest = await _mockTestRepository.CreateAsync(createDto);

                // If specific questions are provided, add them to the mock test
                if (dto.QuestionIds != null && dto.QuestionIds.Any())
                {
                    for (int i = 0; i < dto.QuestionIds.Count; i++)
                    {
                        var questionId = dto.QuestionIds[i];
                        var marks = dto.MarksPerQuestion > 0 ? dto.MarksPerQuestion : examDetails.MarksPerQuestion;
                        var negativeMarks = dto.HasNegativeMarking ? dto.NegativeMarkingValue ?? 0 : examDetails.HasNegativeMarking ? examDetails.NegativeMarkingValue ?? 0 : 0;
                        
                        await _mockTestRepository.AddQuestionAsync(mockTest.Id, questionId, i + 1, marks, negativeMarks);
                    }
                }

                _logger.LogInformation("{MockTestType} draft created successfully: {MockTestId}", dto.MockTestType, mockTest.Id);
                return mockTest;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating {MockTestType} draft: {Name}", dto.MockTestType, dto.Name);
                throw;
            }
        }

        public Task<MockTestListResponseDto> GetMockTestsListAsync(MockTestListRequestDto request)
            => GetMockTestsAsync(request.PageNumber, request.PageSize, request.ExamId, request.SubjectId, request.IsActive);

        public async Task<MockTestSummaryDto> GetMockTestSummaryAsync()
        {
            var result = await GetMockTestsAsync(1, 1000);
            return new MockTestSummaryDto
            {
                TotalMockTests = result.TotalCount,
                ActiveMockTests = result.MockTests.Count(x => x.IsUnlocked || x.AttemptsAllowed > 0),
                PaidMockTests = result.MockTests.Count(x => string.Equals(x.AccessType, "Paid", StringComparison.OrdinalIgnoreCase)),
                FreeMockTests = result.MockTests.Count(x => string.Equals(x.AccessType, "Free", StringComparison.OrdinalIgnoreCase))
            };
        }

        public async Task<List<ExamListDto>> GetExamsForUserAsync(int userId)
        {
            var exams = await _questionFeatureRepository.GetExamsAsync(null);
            return exams.Select(x => new ExamListDto
            {
                Id = (int)(x.GetType().GetProperty("Id")?.GetValue(x) ?? 0),
                Name = x.GetType().GetProperty("Name")?.GetValue(x)?.ToString() ?? string.Empty,
                Description = x.GetType().GetProperty("Description")?.GetValue(x)?.ToString(),
                QuestionCount = (int)(x.GetType().GetProperty("QuestionCount")?.GetValue(x) ?? 0),
                Duration = (int)(x.GetType().GetProperty("Duration")?.GetValue(x) ?? 0),
                IsActive = ToBool(x.GetType().GetProperty("IsActive")?.GetValue(x), true),
                IsLocked = ToBool(x.GetType().GetProperty("IsLocked")?.GetValue(x), false)
            }).ToList();
        }

        private static bool ToBool(object? value, bool defaultValue)
        {
            if (value == null) return defaultValue;
            if (value is bool b) return b;
            if (value is byte by) return by != 0;
            if (value is short s) return s != 0;
            if (value is int i) return i != 0;
            if (value is long l) return l != 0;

            var str = value.ToString();
            if (string.IsNullOrWhiteSpace(str)) return defaultValue;
            if (bool.TryParse(str, out var parsedBool)) return parsedBool;
            if (long.TryParse(str, out var parsedNum)) return parsedNum != 0;
            return defaultValue;
        }

        public async Task<ExamListDto> GetExamDetailsAsync(int userId, int examId)
        {
            var exams = await GetExamsForUserAsync(userId);
            return exams.FirstOrDefault(x => x.Id == examId)
                   ?? throw new KeyNotFoundException($"Exam with ID {examId} not found");
        }

        public async Task<List<SubjectListDto>> GetSubjectsForExamAsync(int userId, int examId)
        {
            try
            {
                // Get exam-specific subjects from master database
                var examSubjects = await _mockTestRepository.GetSubjectsForExamAsync(examId);
                return examSubjects.Select(x => new SubjectListDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    QuestionCount = x.QuestionCount,
                    IsActive = x.IsActive
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subjects for exam {ExamId}", examId);
                throw;
            }
        }

        public async Task<SubjectListDto> GetSubjectDetailsAsync(int userId, int examId, int subjectId)
        {
            var subjects = await GetSubjectsForExamAsync(userId, examId);
            return subjects.FirstOrDefault(x => x.Id == subjectId)
                   ?? throw new KeyNotFoundException($"Subject with ID {subjectId} not found");
        }

        public async Task<SubjectMockTestsResponseDto> GetMockTestsForSubjectAsync(int userId, int examId, int subjectId)
        {
            var list = await GetMockTestsForUserAsync(userId, 1, 200, examId, subjectId);
            return new SubjectMockTestsResponseDto
            {
                SubjectId = subjectId,
                SubjectName = list.MockTests.FirstOrDefault()?.SubjectName ?? string.Empty,
                MockTests = list.MockTests.Select(x => new SimpleMockTestDto
                {
                    MockTestId = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    TotalQuestions = x.TotalQuestions,
                    TotalMarks = x.TotalMarks,
                    DurationInMinutes = x.DurationInMinutes,
                    AccessType = x.AccessType
                }).ToList()
            };
        }

        public async Task<SimpleMockTestDto> GetMockTestDetailsAsync(int userId, int examId, int subjectId, int mockTestId)
        {
            var detail = await GetMockTestDetailForUserAsync(userId, mockTestId)
                         ?? throw new KeyNotFoundException($"Mock test with ID {mockTestId} not found");
            return new SimpleMockTestDto
            {
                MockTestId = detail.Id,
                Name = detail.Name,
                Description = detail.Description,
                TotalQuestions = detail.TotalQuestions,
                TotalMarks = detail.TotalMarks,
                DurationInMinutes = detail.DurationInMinutes,
                AccessType = detail.AccessType
            };
        }

        public Task<object> StartMockTestAsync(int userId, int examId, int subjectId, int mockTestId)
            => StartMockTestAsync(new StartMockTestDto { UserId = userId, MockTestId = mockTestId })
                .ContinueWith(t => (object)t.Result);

        public Task<ExamListDto> CreateExamAsync(CreateExamDto dto)
            => throw new NotSupportedException("Exam creation is not supported in this service.");

        public Task<ExamListDto> UpdateExamAsync(int examId, UpdateExamDto dto)
            => throw new NotSupportedException("Exam update is not supported in this service.");

        public Task<bool> DeleteExamAsync(int examId)
            => throw new NotSupportedException("Exam deletion is not supported in this service.");

        public async Task<SubjectWiseMockTestResponseDto> GetSubjectWiseMockTestsAsync(int userId, MockTestListRequestDto request)
        {
            var list = await GetMockTestsForUserAsync(userId, request.PageNumber, request.PageSize, request.ExamId, request.SubjectId);
            return new SubjectWiseMockTestResponseDto
            {
                Subjects = list.MockTests
                    .GroupBy(x => new { x.SubjectId, x.SubjectName })
                    .Select(g => new SubjectMockTestsResponseDto
                    {
                        SubjectId = g.Key.SubjectId ?? 0,
                        SubjectName = g.Key.SubjectName ?? string.Empty,
                        MockTests = g.Select(x => new SimpleMockTestDto
                        {
                            MockTestId = x.Id,
                            Name = x.Name,
                            Description = x.Description,
                            TotalQuestions = x.TotalQuestions,
                            TotalMarks = x.TotalMarks,
                            DurationInMinutes = x.DurationInMinutes,
                            AccessType = x.AccessType
                        }).ToList()
                    }).ToList()
            };
        }

        public async Task<MockTestFilterOptionsDto> GetMockTestFilterOptionsAsync(int userId)
        {
            return new MockTestFilterOptionsDto
            {
                Exams = (await _questionFeatureRepository.GetAllExamNamesAsync()).ToList(),
                Subjects = (await _questionFeatureRepository.GetSubjectsAsync()).Select(x => new SubjectListDto
                {
                    Id = (int)(x.GetType().GetProperty("Id")?.GetValue(x) ?? 0),
                    Name = x.GetType().GetProperty("Name")?.GetValue(x)?.ToString() ?? string.Empty,
                    Description = x.GetType().GetProperty("Description")?.GetValue(x)?.ToString(),
                    QuestionCount = (int)(x.GetType().GetProperty("QuestionCount")?.GetValue(x) ?? 0),
                    IsActive = (bool)(x.GetType().GetProperty("IsActive")?.GetValue(x) ?? true)
                }).ToList()
            };
        }

        public Task<MockTestListResponseDto> GetMockTestsForUserListAsync(int userId, MockTestListRequestDto request)
            => GetMockTestsForUserAsync(userId, request.PageNumber, request.PageSize, request.ExamId, request.SubjectId);

        public Task<MockTestListResponseDto> GetMockTestsForUserAsync(int userId, int? examId, int? subjectId, string? testCategory, string? access, string? difficulty)
            => GetMockTestsForUserAsync(userId, 1, 50, examId, subjectId);

        public Task<MockTestListResponseDto> GetMockTestsForExamAsync(int userId, int examId)
            => GetMockTestsForUserAsync(userId, 1, 50, examId, null);

        public Task<MockTestListResponseDto> GetMockTestsForSubjectSimpleAsync(int userId, int subjectId)
            => GetMockTestsForUserAsync(userId, 1, 50, null, subjectId);

        public Task<MockTestDetailDto?> GetMockTestDetailsSimpleAsync(int userId, int mockTestId)
            => GetMockTestDetailForUserAsync(userId, mockTestId);

        public Task<MockTestSessionDto> StartMockTestSessionSimpleAsync(int userId, int mockTestId)
            => StartMockTestAsync(new StartMockTestDto { UserId = userId, MockTestId = mockTestId });

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

                // Set default values based on mock test type and scheduling
                SetDefaultsForMockTestType(dto, examDetails);
                
                // Set status based on scheduling
                if (!dto.Status.Equals(MockTestStatus.Draft))
                {
                    SetMockTestStatus(dto);
                }

                // Create the mock test
                var mockTest = await _mockTestRepository.CreateAsync(dto);

                // If specific questions are provided, add them to the mock test
                if (dto.QuestionIds != null && dto.QuestionIds.Any())
                {
                    for (int i = 0; i < dto.QuestionIds.Count; i++)
                    {
                        var questionId = dto.QuestionIds[i];
                        var marks = dto.MarksPerQuestion > 0 ? dto.MarksPerQuestion : examDetails.MarksPerQuestion;
                        var negativeMarks = dto.HasNegativeMarking ? dto.NegativeMarkingValue ?? 0 : examDetails.HasNegativeMarking ? examDetails.NegativeMarkingValue ?? 0 : 0;
                        
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
                        MarksPerQuestion = mt.MarksPerQuestion,
                        HasNegativeMarking = mt.HasNegativeMarking,
                        NegativeMarkingValue = mt.NegativeMarkingValue,
                        AccessType = mt.AccessType,
                        SubscriptionPlanId = mt.SubscriptionPlanId,
                        IsUnlocked = false, // Default for admin view
                        AttemptsUsed = 0, // Default for admin view
                        AttemptsAllowed = mt.AttemptsAllowed,
                        Status = mt.Status?.ToString() ?? string.Empty,
                        CreatedAt = mt.CreatedAt,
                        Year = mt.Year,
                        Difficulty = mt.Difficulty,
                        PaperCode = mt.PaperCode,
                        MockTestTypeDisplay = GetMockTestTypeDisplayName(mt.MockTestType),
                        ExamDate = mt.ExamDate,
                        PublishDateTime = mt.PublishDateTime,
                        ValidTill = mt.ValidTill,
                        ShowResultType = mt.ShowResultType,
                        ImageUrl = mt.ImageUrl
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

                // Always compute lock/unlock server-side from one rule set.
                foreach (var mockTest in mockTests)
                {
                    var (canAccess, _) = EvaluateAccess(mockTest, userSubscription);
                    mockTest.IsUnlocked = canAccess;
                }

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
                var (canAccess, reason) = EvaluateAccess(mockTest, userSubscription);

                return new MockTestAccessResponseDto
                {
                    CanAccess = canAccess,
                    Reason = reason,
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
                    if (string.Equals(accessCheck.Reason, "Mock test not found", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new KeyNotFoundException(accessCheck.Reason);
                    }

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

        public async Task<bool> SaveMockTestAnswerAsync(int sessionId, int userId, SaveMockTestAnswerDto answer)
        {
            try
            {
                _logger.LogInformation("Saving answer for mock test session: {SessionId}, question: {QuestionId}", sessionId, answer.QuestionId);
                return await _mockTestRepository.SaveSessionAnswerAsync(sessionId, userId, answer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving answer for mock test session: {SessionId}", sessionId);
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

        public async Task<MockTestAttemptDto> SubmitMockTestAsync(int sessionId, int userId)
        {
            try
            {
                _logger.LogInformation("Submitting mock test session from saved answers: {SessionId} for user: {UserId}", sessionId, userId);
                return await _mockTestRepository.SubmitSessionAsync(sessionId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting mock test session from saved answers: {SessionId}", sessionId);
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
        public async Task<MockTestStatisticsDto> GetMockTestStatisticsAsync(MockTestStatisticsRequestDto? request = null)
        {
            try
            {
                var examId = request?.ExamId;
                var subjectId = request?.SubjectId;
                return await _mockTestRepository.GetMockTestStatisticsAsync(examId, subjectId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock test statistics");
                throw;
            }
        }

        public async Task<object> GetMockTestStatisticsLegacyAsync(int? examId = null, int? subjectId = null)
        {
            try
            {
                return await _mockTestRepository.GetStatisticsLegacyAsync(examId, subjectId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mock test statistics (legacy)");
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
                    dto.Difficulty = dto.Difficulty ?? "Medium";
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
                dto.TotalMarks = dto.TotalQuestions * (dto.MarksPerQuestion > 0 ? dto.MarksPerQuestion : examDetails.MarksPerQuestion);
            }

            if (dto.PassingMarks == 0)
            {
                dto.PassingMarks = dto.TotalMarks * 0.35m; // 35% passing
            }
            
            // Set default negative marking if not specified
            if (!dto.HasNegativeMarking && examDetails.HasNegativeMarking)
            {
                dto.HasNegativeMarking = examDetails.HasNegativeMarking;
                dto.NegativeMarkingValue = examDetails.NegativeMarkingValue;
            }
            
            // Set default marks per question if not specified
            if (dto.MarksPerQuestion == 0)
            {
                dto.MarksPerQuestion = examDetails.MarksPerQuestion;
            }
        }
        
        private void SetMockTestStatus(CreateMockTestDto dto)
        {
            // If PublishDateTime is set, check if it should be active, scheduled, or inactive
            if (dto.PublishDateTime.HasValue)
            {
                if (dto.PublishDateTime.Value > DateTime.UtcNow)
                {
                    // Scheduled for future - set to Scheduled
                    dto.Status = MockTestStatus.Scheduled;
                }
                else
                {
                    // PublishDateTime is in the past or now - set to Active
                    dto.Status = MockTestStatus.Active;
                }
            }
            else
            {
                // No PublishDateTime set - default to Active
                dto.Status = MockTestStatus.Active;
            }
            
            // Check if ValidTill is expired (but don't override Scheduled status)
            if (dto.ValidTill.HasValue && dto.ValidTill.Value < DateTime.UtcNow && dto.Status != MockTestStatus.Scheduled)
            {
                dto.Status = MockTestStatus.Inactive;
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
                    var questionIdValue = question.GetType().GetProperty("Id")?.GetValue(question);
                    if (questionIdValue == null)
                    {
                        _logger.LogWarning("Question ID is null for question at index {Index}", i);
                        continue; // Skip this question
                    }
                    var questionId = Convert.ToInt32(questionIdValue);
                    
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

        private static (bool CanAccess, string Reason) EvaluateAccess(MockTestDto mockTest, UserSubscriptionDto? userSubscription)
            => EvaluateAccess(mockTest.AccessType, mockTest.SubscriptionPlanId, userSubscription);

        private static (bool CanAccess, string Reason) EvaluateAccess(MockTestListDto mockTest, UserSubscriptionDto? userSubscription)
            => EvaluateAccess(mockTest.AccessType, mockTest.SubscriptionPlanId, userSubscription);

        private static (bool CanAccess, string Reason) EvaluateAccess(string accessType, int? requiredSubscriptionPlanId, UserSubscriptionDto? userSubscription)
        {
            if (string.Equals(accessType, "Free", StringComparison.OrdinalIgnoreCase))
            {
                return (true, "Free mock test");
            }

            if (userSubscription == null || !userSubscription.IsActive || userSubscription.IsExpired)
            {
                return (false, "Active subscription required");
            }

            if (requiredSubscriptionPlanId.HasValue && requiredSubscriptionPlanId != userSubscription.SubscriptionPlanId)
            {
                return (false, "This mock test is not included in your current plan");
            }

            // Plan-level quota enforcement: when quota is configured, block after limit.
            if (userSubscription.TestsTotal > 0 && userSubscription.TestsUsed >= userSubscription.TestsTotal)
            {
                return (false, "Mock test quota exhausted for your subscription");
            }

            if (userSubscription.TestsTotal <= 0)
            {
                return (false, "No mock test quota configured for your subscription");
            }

            return (true, "Access granted");
        }

        private static string GetMockTestTypeDisplayName(MockTestType mockTestType)
        {
            return mockTestType switch
            {
                MockTestType.MockTest => "Mock Test",
                MockTestType.TestSeries => "Test Series",
                MockTestType.DeepPractice => "Deep Practice",
                MockTestType.PreviousYear => "Previous Year",
                _ => "Unknown"
            };
        }

        private static MockTestStatus GetStatusFromString(string? statusValue)
        {
            return statusValue switch
            {
                "Active" => MockTestStatus.Active,
                "Inactive" => MockTestStatus.Inactive,
                "Draft" => MockTestStatus.Draft,
                _ => MockTestStatus.Active
            };
        }
    }
}
