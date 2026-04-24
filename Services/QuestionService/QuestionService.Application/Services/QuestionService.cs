using AutoMapper;
using Microsoft.AspNetCore.Http;
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
        private readonly IMockTestRepository _mockTestRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<QuestionService> _logger;

        public QuestionService(
            DomainQuestionRepository questionRepository,
            IQuestionRepository adminRepository,
            IQuestionFeatureRepository featureRepository,
            IMockTestRepository mockTestRepository,
            IMapper mapper,
            ILogger<QuestionService> logger)
        {
            _questionRepository = questionRepository;
            _adminRepository = adminRepository;
            _featureRepository = featureRepository;
            _mockTestRepository = mockTestRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<QuestionDto> CreateAsync(CreateQuestionDto dto)
        {
            try
            {
                _logger.LogInformation("Creating new question for exam: {ExamId}, subject: {SubjectId}", dto.ExamId, dto.SubjectId);
                await ValidateExamSubjectTopicMappingAsync(dto.ExamId, dto.SubjectId, dto.TopicId, dto.ModuleId);

                var requestCreateDto = new CreateQuestionRequestDto
                {
                    ModuleId = dto.ModuleId ?? 0,
                    ExamId = dto.ExamId,
                    SubjectId = dto.SubjectId,
                    TopicId = dto.TopicId ?? 0,
                    QuestionText = dto.QuestionText ?? "",
                    OptionA = dto.OptionA ?? "",
                    OptionB = dto.OptionB ?? "",
                    OptionC = dto.OptionC ?? "",
                    OptionD = dto.OptionD ?? "",
                    CorrectAnswer = dto.CorrectAnswer,
                    Explanation = dto.Explanation ?? "",
                    Marks = (int)dto.Marks,
                    NegativeMarks = dto.NegativeMarks,
                    DifficultyLevel = dto.DifficultyLevel,
                    QuestionType = dto.QuestionType ?? "MCQ",
                    SameExplanationForAllLanguages = dto.SameExplanationForAllLanguages,
                    Reference = dto.Reference,
                    Tags = dto.Tags,
                    CreatedBy = dto.CreatedBy,
                    Translations = BuildAdminTranslations(dto)
                };

                var createdQuestionId = await _adminRepository.CreateAdminQuestionAsync(requestCreateDto);
                var createdQuestion = await _adminRepository.GetAdminQuestionByIdAsync(createdQuestionId);

                if (createdQuestion == null)
                {
                    throw new InvalidOperationException($"Question with ID {createdQuestionId} was created but could not be reloaded.");
                }

                return MapAdminDetailToQuestionDto(createdQuestion, dto);
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

        public async Task<QuestionCursorResponseDto> GetQuestionsCursorAsync(QuestionCursorRequestDto request)
        {
            try
            {
                var (questions, totalCount, nextCursor, previousCursor, hasNextPage, hasPreviousPage) = await _questionRepository.GetQuestionsCursorAsync(
                    request.Cursor,
                    request.PageSize,
                    request.Direction,
                    request.ExamId,
                    request.SubjectId,
                    request.TopicId,
                    request.DifficultyLevel,
                    request.IsPublished,
                    request.LanguageCode);
                
                var questionDtos = _mapper.Map<IEnumerable<QuestionDto>>(questions);

                return new QuestionCursorResponseDto
                {
                    Data = questionDtos.ToList(),
                    NextCursor = nextCursor,
                    PreviousCursor = previousCursor,
                    HasNextPage = hasNextPage,
                    HasPreviousPage = hasPreviousPage,
                    TotalCount = totalCount,
                    PageSize = request.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cursor paginated questions");
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

        public async Task<QuestionAdminDetailDto> CreateAdminQuestionAsync(CreateQuestionRequestDto dto)
        {
            await ValidateExamSubjectTopicMappingAsync(dto.ExamId, dto.SubjectId, dto.TopicId, dto.ModuleId);
            var createdQuestionId = await _adminRepository.CreateAdminQuestionAsync(dto);
            var createdQuestion = await _adminRepository.GetAdminQuestionByIdAsync(createdQuestionId);
            if (createdQuestion == null)
            {
                throw new Exception($"Question with ID {createdQuestionId} was created but could not be reloaded");
            }

            return createdQuestion;
        }

        public async Task<QuestionAdminDetailDto> CreateAdminQuestionWithImagesAsync(CreateQuestionFormDataDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (_mockTestRepository == null)
            {
                throw new InvalidOperationException("Mock test repository is not configured.");
            }

            if (dto.MockTestId <= 0)
            {
                throw new ArgumentException("A valid mock test id is required.", nameof(dto.MockTestId));
            }

            // Get ExamId from MockTestId
            var mockTest = await _mockTestRepository.GetByIdAsync(dto.MockTestId);
            if (mockTest == null)
            {
                throw new ArgumentException($"Mock test with ID {dto.MockTestId} not found.");
            }
            
            await ValidateExamSubjectTopicMappingAsync(mockTest.ExamId, dto.SubjectId, dto.TopicId, dto.ModuleId);
            
            // Upload images and get URLs
            var questionImageUrl = await UploadImageAsync(dto.QuestionImage, "question-images");
            var optionAImageUrl = await UploadImageAsync(dto.OptionAImage, "option-images");
            var optionBImageUrl = await UploadImageAsync(dto.OptionBImage, "option-images");
            var optionCImageUrl = await UploadImageAsync(dto.OptionCImage, "option-images");
            var optionDImageUrl = await UploadImageAsync(dto.OptionDImage, "option-images");
            var explanationImageUrl = await UploadImageAsync(dto.ExplanationImage, "explanation-images");
            
            // Convert FormData to RequestDto with image URLs
            var requestDto = new CreateQuestionRequestDto
            {
                ModuleId = dto.ModuleId,
                ExamId = mockTest.ExamId,
                SubjectId = dto.SubjectId,
                TopicId = dto.TopicId ?? 0,
                QuestionText = dto.QuestionText,
                OptionA = dto.OptionA,
                OptionB = dto.OptionB,
                OptionC = dto.OptionC,
                OptionD = dto.OptionD,
                CorrectAnswer = dto.CorrectAnswer,
                Explanation = dto.Explanation,
                Marks = dto.Marks,
                NegativeMarks = dto.NegativeMarks,
                DifficultyLevel = dto.DifficultyLevel,
                QuestionType = dto.QuestionType,
                QuestionImageUrl = questionImageUrl,
                OptionAImageUrl = optionAImageUrl,
                OptionBImageUrl = optionBImageUrl,
                OptionCImageUrl = optionCImageUrl,
                OptionDImageUrl = optionDImageUrl,
                ExplanationImageUrl = explanationImageUrl,
                SameExplanationForAllLanguages = dto.SameExplanationForAllLanguages,
                Reference = dto.Reference,
                Tags = dto.Tags,
                CreatedBy = dto.CreatedBy,
                Translations = ParseTranslationsJson(dto.TranslationsJson)
            };
            
            var createdQuestionId = await _adminRepository.CreateAdminQuestionAsync(requestDto);
            var createdQuestion = await _adminRepository.GetAdminQuestionByIdAsync(createdQuestionId);
            if (createdQuestion == null)
            {
                throw new Exception($"Question with ID {createdQuestionId} was created but could not be reloaded");
            }

            return createdQuestion;
        }

        private async Task<string?> UploadImageAsync(IFormFile? imageFile, string folder)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            try
            {
                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", folder, fileName);
                
                // Ensure directory exists
                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                
                // Return URL
                return $"/images/{folder}/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image to folder: {Folder}", folder);
                return null;
            }
        }

        private List<QuestionTranslationUpsertDto> ParseTranslationsJson(string translationsJson)
        {
            try
            {
                if (string.IsNullOrEmpty(translationsJson) || translationsJson == "[]")
                    return new List<QuestionTranslationUpsertDto>();
                
                return System.Text.Json.JsonSerializer.Deserialize<List<QuestionTranslationUpsertDto>>(translationsJson) ?? new List<QuestionTranslationUpsertDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing translations JSON: {Json}", translationsJson);
                return new List<QuestionTranslationUpsertDto>();
            }
        }

        public async Task<QuestionAdminDetailDto> UpdateAdminQuestionAsync(UpdateQuestionAdminDto dto)
        {
            await ValidateExamSubjectTopicMappingAsync(dto.ExamId, dto.SubjectId, dto.TopicId, dto.ModuleId);
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
                if (dto.SubjectId.HasValue)
                {
                    await ValidateExamSubjectTopicMappingAsync(dto.ExamId, dto.SubjectId.Value, dto.TopicId);
                }
                
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
                await ValidateExamSubjectTopicMappingAsync(dto.ExamId, examDetails.SubjectId, null);

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
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new Application.Serialization.BoolIntJsonConverter());

            var result = JsonSerializer.Deserialize<T>(json, options);

            if (result == null)
            {
                throw new InvalidOperationException($"Unable to convert value to {typeof(T).Name}.");
            }

            return result;
        }

        private static List<QuestionTranslationUpsertDto> BuildAdminTranslations(CreateQuestionDto dto)
        {
            var translations = new List<QuestionTranslationUpsertDto>
            {
                new()
                {
                    LanguageCode = "en",
                    QuestionText = dto.QuestionText,
                    OptionA = dto.OptionA,
                    OptionB = dto.OptionB,
                    OptionC = dto.OptionC,
                    OptionD = dto.OptionD,
                    Explanation = dto.Explanation,
                    QuestionImageUrl = dto.QuestionImageUrl,
                    OptionAImageUrl = dto.OptionAImageUrl,
                    OptionBImageUrl = dto.OptionBImageUrl,
                    OptionCImageUrl = dto.OptionCImageUrl,
                    OptionDImageUrl = dto.OptionDImageUrl
                }
            };

            if (dto.Translations == null)
            {
                return translations;
            }

            foreach (var translation in dto.Translations)
            {
                if (string.IsNullOrWhiteSpace(translation.LanguageCode))
                {
                    continue;
                }

                translations.Add(new QuestionTranslationUpsertDto
                {
                    LanguageCode = translation.LanguageCode,
                    QuestionText = translation.QuestionText,
                    OptionA = translation.OptionA,
                    OptionB = translation.OptionB,
                    OptionC = translation.OptionC,
                    OptionD = translation.OptionD,
                    Explanation = translation.Explanation
                });
            }

            return translations;
        }

        private static QuestionDto MapAdminDetailToQuestionDto(QuestionAdminDetailDto adminQuestion, CreateQuestionDto sourceDto)
        {
            var englishTranslation = adminQuestion.Translations
                .FirstOrDefault(t => string.Equals(t.LanguageCode, "en", StringComparison.OrdinalIgnoreCase));

            return new QuestionDto
            {
                Id = adminQuestion.Id,
                ModuleId = adminQuestion.ModuleId,
                ExamId = adminQuestion.ExamId,
                SubjectId = adminQuestion.SubjectId,
                TopicId = adminQuestion.TopicId,
                QuestionText = englishTranslation?.QuestionText ?? sourceDto.QuestionText,
                OptionA = englishTranslation?.OptionA ?? sourceDto.OptionA,
                OptionB = englishTranslation?.OptionB ?? sourceDto.OptionB,
                OptionC = englishTranslation?.OptionC ?? sourceDto.OptionC,
                OptionD = englishTranslation?.OptionD ?? sourceDto.OptionD,
                CorrectAnswer = adminQuestion.CorrectAnswer,
                Explanation = englishTranslation?.Explanation ?? sourceDto.Explanation,
                Marks = adminQuestion.Marks,
                NegativeMarks = adminQuestion.NegativeMarks,
                DifficultyLevel = adminQuestion.DifficultyLevel,
                QuestionType = sourceDto.QuestionType,
                QuestionImageUrl = sourceDto.QuestionImageUrl,
                OptionAImageUrl = sourceDto.OptionAImageUrl,
                OptionBImageUrl = sourceDto.OptionBImageUrl,
                OptionCImageUrl = sourceDto.OptionCImageUrl,
                OptionDImageUrl = sourceDto.OptionDImageUrl,
                ExplanationImageUrl = sourceDto.ExplanationImageUrl,
                SameExplanationForAllLanguages = adminQuestion.SameExplanationForAllLanguages,
                Reference = sourceDto.Reference,
                Tags = sourceDto.Tags,
                CreatedBy = sourceDto.CreatedBy,
                IsPublished = adminQuestion.IsPublished,
                CreatedAt = DateTime.UtcNow,
                IsActive = adminQuestion.IsActive,
                Translations = adminQuestion.Translations
                    .Select(t => new QuestionTranslationDto
                    {
                        Id = t.Id,
                        QuestionId = t.QuestionId,
                        LanguageCode = t.LanguageCode,
                        QuestionText = t.QuestionText,
                        OptionA = t.OptionA,
                        OptionB = t.OptionB,
                        OptionC = t.OptionC,
                        OptionD = t.OptionD,
                        Explanation = t.Explanation,
                        CreatedAt = t.CreatedAt,
                        UpdatedAt = t.UpdatedAt
                    })
                    .ToList()
            };
        }

        private async Task ValidateExamSubjectTopicMappingAsync(int examId, int subjectId, int? topicId, int? moduleId = null)
        {
            // Treat 0 / negative as "not provided" (common for form-data default ints)
            if (topicId.HasValue && topicId.Value <= 0)
            {
                topicId = null;
            }

            // Check if subject is mapped to exam
            var isSubjectMappedToExam = await _featureRepository.IsSubjectMappedToExamAsync(subjectId, examId);
            if (!isSubjectMappedToExam)
            {
                throw new ArgumentException($"Subject {subjectId} is not mapped to exam {examId}.");
            }

            // Topic is only required for ModuleId 3 (Deep Practice)
            // For ModuleId 1, 2, 4 - Topic is optional
            if (moduleId == 3 && (!topicId.HasValue || topicId.Value <= 0))
            {
                throw new ArgumentException($"Topic is required for ModuleId 3 (Deep Practice).");
            }

            // Only validate topic mapping if topic is provided
            if (topicId.HasValue)
            {
                var topicMapped = await _featureRepository.IsTopicMappedToExamSubjectAsync(topicId.Value, examId, subjectId);
                if (!topicMapped)
                {
                    throw new ArgumentException($"Topic {topicId.Value} is not mapped to exam {examId} and subject {subjectId}.");
                }
            }
        }

        public async Task<QuestionDto> CreateQuestionWithImagesAsync(CreateQuestionWithImagesDto dto)
        {
            try
            {
                _logger.LogInformation("Creating question with images for exam: {ExamId}, subject: {SubjectId}", dto.ExamId, dto.SubjectId);
                await ValidateExamSubjectTopicMappingAsync(dto.ExamId, dto.SubjectId, dto.TopicId, dto.ModuleId);

                // Upload images if provided
                var questionImageUrl = dto.QuestionImageUrl;
                var optionAImageUrl = dto.OptionAImageUrl;
                var optionBImageUrl = dto.OptionBImageUrl;
                var optionCImageUrl = dto.OptionCImageUrl;
                var optionDImageUrl = dto.OptionDImageUrl;
                var explanationImageUrl = dto.ExplanationImageUrl;

                if (dto.QuestionImage != null)
                {
                    questionImageUrl = await _featureRepository.UploadQuestionImageAsync(dto.QuestionImage, "question", null, "en");
                }

                if (dto.OptionAImage != null)
                {
                    optionAImageUrl = await _featureRepository.UploadQuestionImageAsync(dto.OptionAImage, "optiona", null, "en");
                }

                if (dto.OptionBImage != null)
                {
                    optionBImageUrl = await _featureRepository.UploadQuestionImageAsync(dto.OptionBImage, "optionb", null, "en");
                }

                if (dto.OptionCImage != null)
                {
                    optionCImageUrl = await _featureRepository.UploadQuestionImageAsync(dto.OptionCImage, "optionc", null, "en");
                }

                if (dto.OptionDImage != null)
                {
                    optionDImageUrl = await _featureRepository.UploadQuestionImageAsync(dto.OptionDImage, "optiond", null, "en");
                }

                if (dto.ExplanationImage != null)
                {
                    explanationImageUrl = await _featureRepository.UploadQuestionImageAsync(dto.ExplanationImage, "explanation", null, "en");
                }

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

                // Update question with image URLs
                if (!string.IsNullOrEmpty(questionImageUrl) || !string.IsNullOrEmpty(optionAImageUrl) || 
                    !string.IsNullOrEmpty(optionBImageUrl) || !string.IsNullOrEmpty(optionCImageUrl) || 
                    !string.IsNullOrEmpty(optionDImageUrl) || !string.IsNullOrEmpty(explanationImageUrl))
                {
                    await _questionRepository.UpdateQuestionAsync(question.Id, dto.QuestionText, dto.OptionA, dto.OptionB, dto.OptionC, dto.OptionD, dto.CorrectAnswer, dto.Marks, dto.DifficultyLevel);
                    
                    // Update image URLs using the repository
                    await _adminRepository.UpdateQuestionImageUrlsAsync(question.Id, questionImageUrl, optionAImageUrl, optionBImageUrl, optionCImageUrl, optionDImageUrl, explanationImageUrl);
                }

                return await GetByIdAsync(question.Id) ?? throw new InvalidOperationException($"Unable to load question {question.Id} after creation.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating question with images");
                throw;
            }
        }

        public async Task<QuestionDto> UpdateQuestionWithImagesAsync(UpdateQuestionWithImagesDto dto)
        {
            try
            {
                _logger.LogInformation("Updating question with images: {QuestionId}", dto.Id);
                await ValidateExamSubjectTopicMappingAsync(dto.ExamId, dto.SubjectId, dto.TopicId, dto.ModuleId);

                var existingQuestion = await _questionRepository.GetByIdAsync(dto.Id);
                if (existingQuestion == null)
                    throw new KeyNotFoundException($"Question with ID {dto.Id} not found");

                // Upload new images if provided
                var questionImageUrl = dto.QuestionImageUrl ?? existingQuestion.QuestionImageUrl;
                var optionAImageUrl = dto.OptionAImageUrl ?? existingQuestion.OptionAImageUrl;
                var optionBImageUrl = dto.OptionBImageUrl ?? existingQuestion.OptionBImageUrl;
                var optionCImageUrl = dto.OptionCImageUrl ?? existingQuestion.OptionCImageUrl;
                var optionDImageUrl = dto.OptionDImageUrl ?? existingQuestion.OptionDImageUrl;
                var explanationImageUrl = dto.ExplanationImageUrl ?? existingQuestion.ExplanationImageUrl;

                if (dto.QuestionImage != null)
                {
                    questionImageUrl = await _featureRepository.UploadQuestionImageAsync(dto.QuestionImage, "question", dto.Id, "en");
                }

                if (dto.OptionAImage != null)
                {
                    optionAImageUrl = await _featureRepository.UploadQuestionImageAsync(dto.OptionAImage, "optiona", dto.Id, "en");
                }

                if (dto.OptionBImage != null)
                {
                    optionBImageUrl = await _featureRepository.UploadQuestionImageAsync(dto.OptionBImage, "optionb", dto.Id, "en");
                }

                if (dto.OptionCImage != null)
                {
                    optionCImageUrl = await _featureRepository.UploadQuestionImageAsync(dto.OptionCImage, "optionc", dto.Id, "en");
                }

                if (dto.OptionDImage != null)
                {
                    optionDImageUrl = await _featureRepository.UploadQuestionImageAsync(dto.OptionDImage, "optiond", dto.Id, "en");
                }

                if (dto.ExplanationImage != null)
                {
                    explanationImageUrl = await _featureRepository.UploadQuestionImageAsync(dto.ExplanationImage, "explanation", dto.Id, "en");
                }

                var success = await _questionRepository.UpdateQuestionAsync(
                    dto.Id,
                    dto.QuestionText,
                    dto.OptionA,
                    dto.OptionB,
                    dto.OptionC,
                    dto.OptionD,
                    dto.CorrectAnswer,
                    dto.Marks,
                    dto.DifficultyLevel);

                if (!success)
                    throw new KeyNotFoundException($"Question with ID {dto.Id} not found");

                // Update image URLs using the repository
                await _adminRepository.UpdateQuestionImageUrlsAsync(dto.Id, questionImageUrl, optionAImageUrl, optionBImageUrl, optionCImageUrl, optionDImageUrl, explanationImageUrl);

                return await GetByIdAsync(dto.Id) ?? throw new InvalidOperationException($"Unable to load question {dto.Id} after update.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating question with images: {QuestionId}", dto.Id);
                throw;
            }
        }
    }

    public interface IQuestionService
    {
        Task<QuestionDto> CreateAsync(CreateQuestionDto dto);
        Task<QuestionDto?> GetByIdAsync(int id);
        Task<IEnumerable<QuestionDto>> GetAllAsync();
        Task<QuestionListResponseDto> GetPagedAsync(QuestionListRequestDto request);
        Task<QuestionCursorResponseDto> GetQuestionsCursorAsync(QuestionCursorRequestDto request);
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
        Task<QuestionAdminDetailDto> CreateAdminQuestionAsync(CreateQuestionRequestDto dto);
        Task<QuestionAdminDetailDto> CreateAdminQuestionWithImagesAsync(CreateQuestionFormDataDto dto);
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
        
        // Create/Update Question with Images
        Task<QuestionDto> CreateQuestionWithImagesAsync(CreateQuestionWithImagesDto dto);
        Task<QuestionDto> UpdateQuestionWithImagesAsync(UpdateQuestionWithImagesDto dto);
    }

}
