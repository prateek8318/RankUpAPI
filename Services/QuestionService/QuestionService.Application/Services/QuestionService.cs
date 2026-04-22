using AutoMapper;
using Microsoft.Extensions.Logging;
using QuestionService.Application.DTOs;
using QuestionService.Application.Interfaces;
using QuestionService.Domain.Entities;
using System.Text.Json;
using DomainQuestionRepository = QuestionService.Domain.Interfaces.IQuestionRepository;

namespace QuestionService.Application.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly DomainQuestionRepository _questionRepository;
        private readonly IQuestionRepository _adminRepository;
        private readonly IQuestionFeatureRepository _featureRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<QuestionService> _logger;

        public QuestionService(
            DomainQuestionRepository questionRepository,
            IQuestionRepository adminRepository,
            IQuestionFeatureRepository featureRepository,
            IMapper mapper,
            ILogger<QuestionService> logger)
        {
            _questionRepository = questionRepository;
            _adminRepository = adminRepository;
            _featureRepository = featureRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<QuestionDto> CreateAsync(CreateQuestionDto dto)
        {
            try
            {
                _logger.LogInformation("Creating new question for exam: {ExamId}, subject: {SubjectId}", dto.ExamId, dto.SubjectId);

                var question = await _questionRepository.CreateQuestionAsync(
                    dto.QuestionText,
                    dto.OptionA,
                    dto.OptionB,
                    dto.OptionC,
                    dto.OptionD,
                    dto.CorrectAnswer,
                    dto.Marks,
                    dto.ExamId,
                    dto.SubjectId,
                    dto.TopicId,
                    dto.DifficultyLevel,
                    dto.CreatedBy);
                return _mapper.Map<QuestionDto>(question);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating question");
                throw;
            }
        }

        public async Task<QuestionDto?> GetByIdAsync(int id)
        {
            try
            {
                var question = await _questionRepository.GetByIdAsync(id);
                return question != null ? _mapper.Map<QuestionDto>(question) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question: {QuestionId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionDto>> GetAllAsync()
        {
            try
            {
                var questions = await _questionRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<QuestionDto>>(questions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all questions");
                throw;
            }
        }

        public async Task<QuestionListResponseDto> GetPagedAsync(QuestionListRequestDto request)
        {
            try
            {
                var (questions, totalCount) = await _questionRepository.GetQuestionsPagedAsync(
                    request.PageNumber,
                    request.PageSize,
                    request.ExamId,
                    request.SubjectId,
                    request.TopicId,
                    request.DifficultyLevel,
                    request.IsPublished,
                    request.LanguageCode);
                var questionDtos = _mapper.Map<IEnumerable<QuestionDto>>(questions);

                return new QuestionListResponseDto
                {
                    Items = questionDtos.ToList(),
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paginated questions");
                throw;
            }
        }

        public async Task<QuestionStatisticsDto> GetStatisticsAsync()
        {
            try
            {
                var (totalQuestions, addedToday, negativeMarksCount, unpublishedCount) = await _questionRepository.GetStatisticsAsync();
                
                return new QuestionStatisticsDto
                {
                    TotalQuestions = totalQuestions,
                    AddedToday = addedToday,
                    NegativeMarksCount = negativeMarksCount,
                    UnpublishedCount = unpublishedCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question statistics");
                throw;
            }
        }

        public async Task<QuestionDto> UpdateAsync(int id, UpdateQuestionDto dto)
        {
            try
            {
                _logger.LogInformation("Updating question: {QuestionId}", id);

                var success = await _questionRepository.UpdateQuestionAsync(
                    id,
                    dto.QuestionText,
                    dto.OptionA,
                    dto.OptionB,
                    dto.OptionC,
                    dto.OptionD,
                    dto.CorrectAnswer,
                    dto.Marks,
                    dto.DifficultyLevel);
                if (!success)
                {
                    throw new KeyNotFoundException($"Question with ID {id} not found");
                }

                var updatedQuestion = await _questionRepository.GetByIdAsync(id);
                return _mapper.Map<QuestionDto>(updatedQuestion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating question: {QuestionId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting question: {QuestionId}", id);

                return await _questionRepository.DeleteQuestionAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting question: {QuestionId}", id);
                throw;
            }
        }

        public async Task<bool> PublishQuestionAsync(int questionId, int reviewedBy)
        {
            try
            {
                _logger.LogInformation("Publishing question: {QuestionId} by reviewer: {ReviewedBy}", questionId, reviewedBy);

                return await _questionRepository.TogglePublishStatusAsync(questionId, true, reviewedBy);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing question: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<bool> UnpublishQuestionAsync(int questionId, int reviewedBy)
        {
            try
            {
                _logger.LogInformation("Unpublishing question: {QuestionId} by reviewer: {ReviewedBy}", questionId, reviewedBy);

                return await _questionRepository.TogglePublishStatusAsync(questionId, false, reviewedBy);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unpublishing question: {QuestionId}", questionId);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionDto>> GetByExamIdAsync(int examId)
        {
            try
            {
                var questions = await _questionRepository.GetByExamIdAsync(examId);
                return _mapper.Map<IEnumerable<QuestionDto>>(questions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving questions for exam: {ExamId}", examId);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionDto>> GetBySubjectIdAsync(int subjectId)
        {
            try
            {
                var questions = await _questionRepository.GetBySubjectIdAsync(subjectId);
                return _mapper.Map<IEnumerable<QuestionDto>>(questions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving questions for subject: {SubjectId}", subjectId);
                throw;
            }
        }

        public async Task<IEnumerable<QuestionDto>> GetByTopicIdAsync(int topicId)
        {
            try
            {
                var questions = await _questionRepository.GetByTopicIdAsync(topicId);
                return _mapper.Map<IEnumerable<QuestionDto>>(questions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving questions for topic: {TopicId}", topicId);
                throw;
            }
        }

        public async Task<QuestionDto?> GetByTransactionIdAsync(string transactionId)
        {
            try
            {
                var question = await _questionRepository.GetByTransactionIdAsync(transactionId);
                return question != null ? _mapper.Map<QuestionDto>(question) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving question by transaction ID: {TransactionId}", transactionId);
                throw;
            }
        }

        // Admin-specific method implementations
        public async Task<TopicDto> CreateTopicAsync(CreateTopicDto dto)
        {
            var createdTopicId = await _adminRepository.CreateTopicAsync(dto);
            return new TopicDto
            {
                Id = createdTopicId,
                Name = dto.Name,
                SubjectId = dto.SubjectId,
                Description = dto.Description,
                ParentTopicId = dto.ParentTopicId,
                SortOrder = dto.SortOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public async Task<IEnumerable<TopicDto>> GetTopicsAsync(int subjectId)
        {
            return await _adminRepository.GetTopicsAsync(subjectId, null, true);
        }

        public async Task<QuestionAdminDetailDto> CreateAdminQuestionAsync(CreateQuestionAdminDto dto)
        {
            var createdQuestionId = await _adminRepository.CreateAdminQuestionAsync(dto);
            var createdQuestion = await _adminRepository.GetAdminQuestionByIdAsync(createdQuestionId);
            if (createdQuestion == null)
            {
                throw new Exception($"Question with ID {createdQuestionId} was created but could not be reloaded");
            }

            return createdQuestion;
        }

        public async Task<QuestionAdminDetailDto> UpdateAdminQuestionAsync(UpdateQuestionAdminDto dto)
        {
            var existingQuestion = await _adminRepository.GetAdminQuestionByIdAsync(dto.Id);
            if (existingQuestion == null)
                throw new Exception($"Question with ID {dto.Id} not found");

            var updated = await _adminRepository.UpdateAdminQuestionAsync(dto);
            if (!updated)
                throw new Exception($"Failed to update question with ID {dto.Id}");

            var updatedQuestion = await _adminRepository.GetAdminQuestionByIdAsync(dto.Id);
            if (updatedQuestion == null)
            {
                throw new Exception($"Updated question with ID {dto.Id} could not be reloaded");
            }

            return updatedQuestion;
        }

        public async Task<QuestionAdminDetailDto?> GetAdminQuestionByIdAsync(int id)
        {
            return await _adminRepository.GetAdminQuestionByIdAsync(id);
        }

        public async Task<QuestionPagedResponseDto> GetAdminQuestionsPagedAsync(QuestionFilterRequestDto filter)
        {
            var (questions, totalCount) = await _adminRepository.GetAdminQuestionsPagedAsync(filter);
            return new QuestionPagedResponseDto
            {
                Items = questions.Select(question => new QuestionDto
                {
                    Id = question.Id,
                    ModuleId = question.ModuleId,
                    ExamId = question.ExamId,
                    SubjectId = question.SubjectId,
                    TopicId = question.TopicId,
                    DifficultyLevel = question.DifficultyLevel,
                    Marks = question.Marks,
                    NegativeMarks = question.NegativeMarks,
                    IsPublished = question.IsPublished,
                    IsActive = question.IsActive,
                    CreatedAt = question.CreatedAt,
                    QuestionText = question.DisplayQuestionText ?? string.Empty
                }).ToList(),
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
            };
        }

        public async Task<bool> SetPublishStatusAsync(PublishQuestionRequestDto dto)
        {
            return await _adminRepository.SetPublishStatusAsync(dto.QuestionId, dto.IsPublished);
        }

        public async Task<QuestionDashboardStatsDto> GetDashboardStatsAsync()
        {
            return await _adminRepository.GetDashboardStatsAsync();
        }

        public async Task<object> BulkCreateQuestionsAsync(BulkQuestionUploadRequestDto dto)
        {
            try
            {
                _logger.LogInformation("Starting bulk upload for file: {FileName}", dto.FileName);
                
                var result = await _featureRepository.BulkCreateQuestionsAsync(dto);
                return new { success = true, insertedCount = result.SuccessCount, failedCount = result.FailedCount, batchId = result.BatchId };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk upload");
                throw;
            }
        }

        // Image upload methods
        public async Task<ImageUploadResponseDto> UploadQuestionImageAsync(QuestionImageUploadDto dto)
        {
            try
            {
                _logger.LogInformation("Uploading image for question: {QuestionId}, type: {ImageType}", dto.QuestionId, dto.ImageType);
                
                var imageUrl = await _featureRepository.UploadQuestionImageAsync(dto.Image, dto.ImageType, dto.QuestionId, dto.LanguageCode);
                
                return new ImageUploadResponseDto
                {
                    ImageUrl = imageUrl,
                    ImageType = dto.ImageType,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                return new ImageUploadResponseDto
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        // Quiz functionality methods
        public async Task<QuizSessionDto> StartQuizAsync(QuizStartRequestDto dto)
        {
            try
            {
                _logger.LogInformation("Starting quiz for exam: {ExamId}, user: {UserId}", dto.ExamId, dto.UserId);
                
                var quizSession = await _featureRepository.StartQuizAsync(dto);
                return ConvertTo<QuizSessionDto>(quizSession);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting quiz");
                throw;
            }
        }

        public async Task<QuizSessionDto?> GetQuizSessionAsync(int sessionId, int userId)
        {
            try
            {
                var session = await _featureRepository.GetQuizSessionAsync(sessionId, userId);
                return session != null ? ConvertTo<QuizSessionDto>(session) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quiz session: {SessionId}", sessionId);
                throw;
            }
        }

        public async Task<bool> SaveQuizAnswerAsync(QuizAnswerRequestDto dto)
        {
            try
            {
                _logger.LogInformation("Saving answer for quiz session: {SessionId}, question: {QuestionId}", dto.QuizSessionId, dto.QuestionId);
                
                return await _featureRepository.SaveQuizAnswerAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving quiz answer");
                throw;
            }
        }

        public async Task<QuizResultDto> SubmitQuizAsync(QuizSubmitRequestDto dto)
        {
            try
            {
                _logger.LogInformation("Submitting quiz session: {SessionId}", dto.QuizSessionId);
                
                var result = await _featureRepository.SubmitQuizAsync(dto);
                return ConvertTo<QuizResultDto>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting quiz");
                throw;
            }
        }

        // Subject and Exam listing methods
        public async Task<IEnumerable<SubjectListDto>> GetSubjectsAsync()
        {
            try
            {
                var subjects = await _featureRepository.GetSubjectsAsync();
                return ConvertTo<List<SubjectListDto>>(subjects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subjects");
                throw;
            }
        }

        public async Task<IEnumerable<ExamListDto>> GetExamsAsync(int? subjectId = null)
        {
            try
            {
                var exams = await _featureRepository.GetExamsAsync(subjectId);
                return ConvertTo<List<ExamListDto>>(exams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exams");
                throw;
            }
        }

        // Bulk upload processing methods
        public async Task<BulkUploadProcessDto> GetBulkUploadStatusAsync(int batchId)
        {
            try
            {
                var status = await _featureRepository.GetBulkUploadStatusAsync(batchId);
                return ConvertTo<BulkUploadProcessDto>(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bulk upload status: {BatchId}", batchId);
                throw;
            }
        }

        // Simple Question Creation with Exam Integration
        public async Task<QuestionDto> CreateSimpleQuestionAsync(SimpleQuestionCreateDto dto)
        {
            try
            {
                _logger.LogInformation("Creating simple question for exam: {ExamId}", dto.ExamId);

                // Get exam details to fetch subject and negative marking info
                var examDetails = await _featureRepository.GetExamDetailsAsync(dto.ExamId);
                if (examDetails == null)
                {
                    throw new ArgumentException($"Exam with ID {dto.ExamId} not found");
                }

                var createDto = new CreateQuestionDto
                {
                    ExamId = dto.ExamId,
                    SubjectId = examDetails.SubjectId,
                    QuestionText = dto.QuestionText,
                    OptionA = dto.OptionA,
                    OptionB = dto.OptionB,
                    OptionC = dto.OptionC,
                    OptionD = dto.OptionD,
                    CorrectAnswer = dto.CorrectAnswer,
                    Explanation = dto.Explanation,
                    Marks = examDetails.MarksPerQuestion,
                    NegativeMarks = examDetails.HasNegativeMarking ? examDetails.NegativeMarkingValue ?? 0 : 0,
                    DifficultyLevel = "Medium",
                    QuestionType = "MCQ",
                    CreatedBy = dto.CreatedBy
                };

                return await CreateAsync(createDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating simple question for exam: {ExamId}", dto.ExamId);
                throw;
            }
        }

        // Get Exam Types from ExamService
        public async Task<IEnumerable<ExamTypeDto>> GetExamTypesAsync()
        {
            try
            {
                var examTypes = await _featureRepository.GetExamTypesAsync();
                return ConvertTo<List<ExamTypeDto>>(examTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exam types");
                throw;
            }
        }

        // Get Exam Names by ExamType from ExamService
        public async Task<IEnumerable<ExamNameDto>> GetExamNamesByTypeAsync(string examType)
        {
            try
            {
                var examNames = await _featureRepository.GetExamNamesByTypeAsync(examType);
                return ConvertTo<List<ExamNameDto>>(examNames);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exam names for type: {ExamType}", examType);
                throw;
            }
        }

        // Get All Exam Names
        public async Task<IEnumerable<ExamNameDto>> GetAllExamNamesAsync()
        {
            try
            {
                var examNames = await _featureRepository.GetAllExamNamesAsync();
                return ConvertTo<List<ExamNameDto>>(examNames);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all exam names");
                throw;
            }
        }

        public async Task<List<ExcelQuestionRowDto>> ParseBulkUploadFileAsync(string filePath)
        {
            try
            {
                _logger.LogInformation("Parsing bulk upload file: {FilePath}", filePath);
                
                var questions = await _featureRepository.ParseBulkUploadFileAsync(filePath);
                return ConvertTo<List<ExcelQuestionRowDto>>(questions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing bulk upload file");
                throw;
            }
        }

        private static T ConvertTo<T>(object source)
        {
            if (source is T typed)
            {
                return typed;
            }

            var json = JsonSerializer.Serialize(source);
            var result = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result == null)
            {
                throw new InvalidOperationException($"Unable to convert value to {typeof(T).Name}.");
            }

            return result;
        }
    }

    public interface IQuestionService
    {
        Task<QuestionDto> CreateAsync(CreateQuestionDto dto);
        Task<QuestionDto?> GetByIdAsync(int id);
        Task<IEnumerable<QuestionDto>> GetAllAsync();
        Task<QuestionListResponseDto> GetPagedAsync(QuestionListRequestDto request);
        Task<QuestionStatisticsDto> GetStatisticsAsync();
        Task<QuestionDto> UpdateAsync(int id, UpdateQuestionDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> PublishQuestionAsync(int questionId, int reviewedBy);
        Task<bool> UnpublishQuestionAsync(int questionId, int reviewedBy);
        Task<IEnumerable<QuestionDto>> GetByExamIdAsync(int examId);
        Task<IEnumerable<QuestionDto>> GetBySubjectIdAsync(int subjectId);
        Task<IEnumerable<QuestionDto>> GetByTopicIdAsync(int topicId);
        Task<QuestionDto?> GetByTransactionIdAsync(string transactionId);
        
        // Admin-specific methods
        Task<TopicDto> CreateTopicAsync(CreateTopicDto dto);
        Task<IEnumerable<TopicDto>> GetTopicsAsync(int subjectId);
        Task<QuestionAdminDetailDto> CreateAdminQuestionAsync(CreateQuestionAdminDto dto);
        Task<QuestionAdminDetailDto> UpdateAdminQuestionAsync(UpdateQuestionAdminDto dto);
        Task<QuestionAdminDetailDto?> GetAdminQuestionByIdAsync(int id);
        Task<QuestionPagedResponseDto> GetAdminQuestionsPagedAsync(QuestionFilterRequestDto filter);
        Task<bool> SetPublishStatusAsync(PublishQuestionRequestDto dto);
        Task<QuestionDashboardStatsDto> GetDashboardStatsAsync();
        Task<object> BulkCreateQuestionsAsync(BulkQuestionUploadRequestDto dto);
        
        // Image upload methods
        Task<ImageUploadResponseDto> UploadQuestionImageAsync(QuestionImageUploadDto dto);
        
        // Quiz functionality methods
        Task<QuizSessionDto> StartQuizAsync(QuizStartRequestDto dto);
        Task<QuizSessionDto?> GetQuizSessionAsync(int sessionId, int userId);
        Task<bool> SaveQuizAnswerAsync(QuizAnswerRequestDto dto);
        Task<QuizResultDto> SubmitQuizAsync(QuizSubmitRequestDto dto);
        
        // Subject and Exam listing methods
        Task<IEnumerable<SubjectListDto>> GetSubjectsAsync();
        Task<IEnumerable<ExamListDto>> GetExamsAsync(int? subjectId = null);
        
        // Bulk upload processing methods
        Task<BulkUploadProcessDto> GetBulkUploadStatusAsync(int batchId);
        Task<List<ExcelQuestionRowDto>> ParseBulkUploadFileAsync(string filePath);
        
        // Simple Question Creation with Exam Integration
        Task<QuestionDto> CreateSimpleQuestionAsync(SimpleQuestionCreateDto dto);
        Task<IEnumerable<ExamTypeDto>> GetExamTypesAsync();
        Task<IEnumerable<ExamNameDto>> GetExamNamesByTypeAsync(string examType);
        Task<IEnumerable<ExamNameDto>> GetAllExamNamesAsync();
    }

}
