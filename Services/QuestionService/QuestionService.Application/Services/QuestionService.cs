using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using QuestionService.Application.DTOs;
using QuestionService.Application.Interfaces;
using QuestionService.Domain.Entities;
using System.Globalization;
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
            await AttachQuestionToMockTestIfRequiredAsync(createdQuestionId, dto.MockTestId, dto.Marks, dto.NegativeMarks);
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
                QuestionText = dto.QuestionText ?? string.Empty,
                OptionA = dto.OptionA ?? string.Empty,
                OptionB = dto.OptionB ?? string.Empty,
                OptionC = dto.OptionC ?? string.Empty,
                OptionD = dto.OptionD ?? string.Empty,
                CorrectAnswer = dto.CorrectAnswer ?? string.Empty,
                Explanation = dto.Explanation ?? string.Empty,
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
            await AttachQuestionToMockTestIfRequiredAsync(createdQuestionId, dto.MockTestId, dto.Marks, dto.NegativeMarks);
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

            await AttachQuestionToMockTestIfRequiredAsync(dto.Id, dto.MockTestId, dto.Marks, dto.NegativeMarks);

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
                    ModuleName = question.ModuleName,
                    ExamId = question.ExamId,
                    ExamName = question.ExamName,
                    SubjectId = question.SubjectId,
                    SubjectName = question.SubjectName,
                    TopicId = question.TopicId,
                    TopicName = question.TopicName,
                    QuestionText = string.IsNullOrWhiteSpace(question.QuestionText) ? (question.DisplayQuestionText ?? string.Empty) : question.QuestionText,
                    OptionA = question.OptionA,
                    OptionB = question.OptionB,
                    OptionC = question.OptionC,
                    OptionD = question.OptionD,
                    CorrectAnswer = string.IsNullOrWhiteSpace(question.CorrectAnswer) ? string.Empty : question.CorrectAnswer,
                    Explanation = question.Explanation,
                    Marks = question.Marks,
                    NegativeMarks = question.NegativeMarks,
                    DifficultyLevel = question.DifficultyLevel,
                    QuestionType = question.QuestionType,
                    QuestionImageUrl = question.QuestionImageUrl,
                    OptionAImageUrl = question.OptionAImageUrl,
                    OptionBImageUrl = question.OptionBImageUrl,
                    OptionCImageUrl = question.OptionCImageUrl,
                    OptionDImageUrl = question.OptionDImageUrl,
                    ExplanationImageUrl = question.ExplanationImageUrl,
                    SameExplanationForAllLanguages = question.SameExplanationForAllLanguages,
                    Reference = question.Reference,
                    Tags = question.Tags,
                    CreatedBy = question.CreatedBy,
                    ReviewedBy = question.ReviewedBy,
                    IsPublished = question.IsPublished,
                    PublishDate = question.PublishDate,
                    IsActive = question.IsActive,
                    CreatedAt = question.CreatedAt,
                    UpdatedAt = question.UpdatedAt,
                    MockTestId = question.MockTestId,
                    TranslatedOptionA = question.TranslatedOptionA,
                    TranslatedOptionB = question.TranslatedOptionB,
                    TranslatedOptionC = question.TranslatedOptionC,
                    TranslatedOptionD = question.TranslatedOptionD,
                    TranslatedExplanation = question.TranslatedExplanation
                }).ToList(),
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
            };
        }

        public async Task<ModuleGroupedQuestionResponseDto> GetAdminQuestionsGroupedByModuleAsync(QuestionFilterRequestDto filter)
        {
            // Get ALL questions without pagination first
            var (allQuestions, totalCount) = await _adminRepository.GetAllAdminQuestionsAsync(filter);
            
            // Apply pagination AFTER grouping
            var groupedQuestions = allQuestions
                .GroupBy(q => q.ModuleId)
                .SelectMany(group => group.Select(q => new { Question = q, ModuleId = group.Key }))
                .ToList();

            // Apply pagination to the flattened list
            var pagedQuestions = groupedQuestions
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            // Group the paginated questions by module and then by subject for drill-down UI
            var moduleGroups = pagedQuestions
                .GroupBy(x => x.ModuleId)
                .Select(group => new ModuleQuestionGroupDto
                {
                    ModuleId = group.Key,
                    ModuleName = group.FirstOrDefault()?.Question.ModuleName ?? string.Empty,
                    QuestionCount = group.Count(),
                    Questions = group.Select(x => new QuestionDto
                    {
                        Id = x.Question.Id,
                        ModuleId = x.Question.ModuleId,
                        ModuleName = x.Question.ModuleName,
                        ExamId = x.Question.ExamId,
                        ExamName = x.Question.ExamName,
                        SubjectId = x.Question.SubjectId,
                        SubjectName = x.Question.SubjectName,
                        TopicId = x.Question.TopicId,
                        TopicName = x.Question.TopicName,
                        QuestionText = string.IsNullOrWhiteSpace(x.Question.QuestionText) ? (x.Question.DisplayQuestionText ?? string.Empty) : x.Question.QuestionText,
                        OptionA = x.Question.OptionA,
                        OptionB = x.Question.OptionB,
                        OptionC = x.Question.OptionC,
                        OptionD = x.Question.OptionD,
                        CorrectAnswer = string.IsNullOrWhiteSpace(x.Question.CorrectAnswer) ? string.Empty : x.Question.CorrectAnswer,
                        Explanation = x.Question.Explanation,
                        Marks = x.Question.Marks,
                        NegativeMarks = x.Question.NegativeMarks,
                        DifficultyLevel = x.Question.DifficultyLevel,
                        QuestionType = x.Question.QuestionType,
                        QuestionImageUrl = x.Question.QuestionImageUrl,
                        OptionAImageUrl = x.Question.OptionAImageUrl,
                        OptionBImageUrl = x.Question.OptionBImageUrl,
                        OptionCImageUrl = x.Question.OptionCImageUrl,
                        OptionDImageUrl = x.Question.OptionDImageUrl,
                        ExplanationImageUrl = x.Question.ExplanationImageUrl,
                        SameExplanationForAllLanguages = x.Question.SameExplanationForAllLanguages,
                        Reference = x.Question.Reference,
                        Tags = x.Question.Tags,
                        CreatedBy = x.Question.CreatedBy,
                        ReviewedBy = x.Question.ReviewedBy,
                        IsPublished = x.Question.IsPublished,
                        PublishDate = x.Question.PublishDate,
                        UpdatedAt = x.Question.UpdatedAt,
                        MockTestId = x.Question.MockTestId,
                        MockTestName = x.Question.MockTestName,
                        TranslatedOptionA = x.Question.TranslatedOptionA,
                        TranslatedOptionB = x.Question.TranslatedOptionB,
                        TranslatedOptionC = x.Question.TranslatedOptionC,
                        TranslatedOptionD = x.Question.TranslatedOptionD,
                        TranslatedExplanation = x.Question.TranslatedExplanation
                    }).ToList(),
                    Subjects = group
                        .GroupBy(x => x.Question.SubjectId)
                        .Select(subjectGroup => new SubjectQuestionGroupDto
                        {
                            SubjectId = subjectGroup.Key,
                            SubjectName = subjectGroup.FirstOrDefault()?.Question.SubjectName ?? $"Subject {subjectGroup.Key}",
                            QuestionCount = subjectGroup.Count(),
                            Questions = subjectGroup.Select(x => new QuestionDto
                            {
                                Id = x.Question.Id,
                                ModuleId = x.Question.ModuleId,
                                ModuleName = x.Question.ModuleName,
                                ExamId = x.Question.ExamId,
                                ExamName = x.Question.ExamName,
                                SubjectId = x.Question.SubjectId,
                                SubjectName = x.Question.SubjectName,
                                TopicId = x.Question.TopicId,
                                TopicName = x.Question.TopicName,
                                QuestionText = string.IsNullOrWhiteSpace(x.Question.QuestionText) ? (x.Question.DisplayQuestionText ?? string.Empty) : x.Question.QuestionText,
                                OptionA = x.Question.OptionA,
                                OptionB = x.Question.OptionB,
                                OptionC = x.Question.OptionC,
                                OptionD = x.Question.OptionD,
                                CorrectAnswer = string.IsNullOrWhiteSpace(x.Question.CorrectAnswer) ? string.Empty : x.Question.CorrectAnswer,
                                Explanation = x.Question.Explanation,
                                Marks = x.Question.Marks,
                                NegativeMarks = x.Question.NegativeMarks,
                                DifficultyLevel = x.Question.DifficultyLevel,
                                QuestionType = x.Question.QuestionType,
                                QuestionImageUrl = x.Question.QuestionImageUrl,
                                OptionAImageUrl = x.Question.OptionAImageUrl,
                                OptionBImageUrl = x.Question.OptionBImageUrl,
                                OptionCImageUrl = x.Question.OptionCImageUrl,
                                OptionDImageUrl = x.Question.OptionDImageUrl,
                                ExplanationImageUrl = x.Question.ExplanationImageUrl,
                                SameExplanationForAllLanguages = x.Question.SameExplanationForAllLanguages,
                                Reference = x.Question.Reference,
                                Tags = x.Question.Tags,
                                CreatedBy = x.Question.CreatedBy,
                                ReviewedBy = x.Question.ReviewedBy,
                                IsPublished = x.Question.IsPublished,
                                PublishDate = x.Question.PublishDate,
                                UpdatedAt = x.Question.UpdatedAt,
                                MockTestId = x.Question.MockTestId,
                                MockTestName = x.Question.MockTestName,
                                TranslatedOptionA = x.Question.TranslatedOptionA,
                                TranslatedOptionB = x.Question.TranslatedOptionB,
                                TranslatedOptionC = x.Question.TranslatedOptionC,
                                TranslatedOptionD = x.Question.TranslatedOptionD,
                                TranslatedExplanation = x.Question.TranslatedExplanation
                            }).ToList()
                        }).ToList()
                }).ToList();

            return new ModuleGroupedQuestionResponseDto
            {
                ModuleGroups = moduleGroups,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
            };
        }

        public async Task<MockTestGroupedQuestionResponseDto> GetAdminQuestionsGroupedByMockTestAsync(QuestionFilterRequestDto filter)
        {
            var rows = (await _adminRepository.GetAllAdminQuestionsGroupedByMockTestAsync(filter)).ToList();

            static string GetMockTestTypeName(int mockTestTypeId)
            {
                return mockTestTypeId switch
                {
                    1 => "Mock Test",
                    2 => "Test Series",
                    3 => "Deep Practice",
                    4 => "Previous Year",
                    _ => "Unknown"
                };
            }

            var groups = rows
                .Where(r => r.MockTestId.HasValue && r.MockTestId.Value > 0)
                .GroupBy(r => new
                {
                    MockTestId = r.MockTestId!.Value,
                    r.MockTestName,
                    MockTestTypeId = r.MockTestTypeId ?? 0,
                    r.ExamId,
                    r.ExamName,
                    r.SubjectId,
                    r.SubjectName
                })
                .Select(g => new MockTestQuestionGroupDto
                {
                    MockTestId = g.Key.MockTestId,
                    MockTestName = g.Key.MockTestName ?? $"MockTest {g.Key.MockTestId}",
                    MockTestTypeId = g.Key.MockTestTypeId,
                    MockTestTypeName = GetMockTestTypeName(g.Key.MockTestTypeId),
                    ExamId = g.Key.ExamId,
                    ExamName = g.Key.ExamName,
                    SubjectId = g.Key.SubjectId,
                    SubjectName = g.Key.SubjectName,
                    QuestionCount = g.Select(x => x.Id).Distinct().Count(),
                    Questions = g
                        .GroupBy(x => x.Id)
                        .Select(x => x.First())
                        .Select(q => new QuestionDto
                        {
                            Id = q.Id,
                            ModuleId = q.ModuleId,
                            ModuleName = q.ModuleName,
                            ExamId = q.ExamId,
                            ExamName = q.ExamName,
                            SubjectId = q.SubjectId,
                            SubjectName = q.SubjectName,
                            TopicId = q.TopicId,
                            TopicName = q.TopicName,
                            QuestionText = string.IsNullOrWhiteSpace(q.QuestionText) ? (q.DisplayQuestionText ?? string.Empty) : q.QuestionText,
                            OptionA = q.OptionA,
                            OptionB = q.OptionB,
                            OptionC = q.OptionC,
                            OptionD = q.OptionD,
                            CorrectAnswer = string.IsNullOrWhiteSpace(q.CorrectAnswer) ? string.Empty : q.CorrectAnswer,
                            Explanation = q.Explanation,
                            Marks = q.Marks,
                            NegativeMarks = q.NegativeMarks,
                            DifficultyLevel = q.DifficultyLevel,
                            QuestionType = q.QuestionType,
                            QuestionImageUrl = q.QuestionImageUrl,
                            OptionAImageUrl = q.OptionAImageUrl,
                            OptionBImageUrl = q.OptionBImageUrl,
                            OptionCImageUrl = q.OptionCImageUrl,
                            OptionDImageUrl = q.OptionDImageUrl,
                            ExplanationImageUrl = q.ExplanationImageUrl,
                            SameExplanationForAllLanguages = q.SameExplanationForAllLanguages,
                            Reference = q.Reference,
                            Tags = q.Tags,
                            CreatedBy = q.CreatedBy,
                            ReviewedBy = q.ReviewedBy,
                            IsPublished = q.IsPublished,
                            PublishDate = q.PublishDate,
                            UpdatedAt = q.UpdatedAt,
                            IsActive = q.IsActive,
                            MockTestId = q.MockTestId,
                            MockTestName = q.MockTestName,
                            TranslatedOptionA = q.TranslatedOptionA,
                            TranslatedOptionB = q.TranslatedOptionB,
                            TranslatedOptionC = q.TranslatedOptionC,
                            TranslatedOptionD = q.TranslatedOptionD,
                            TranslatedExplanation = q.TranslatedExplanation
                        })
                        .ToList()
                })
                .OrderByDescending(g => g.MockTestId)
                .ToList();

            var total = groups.Count;
            var pagedGroups = groups
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            return new MockTestGroupedQuestionResponseDto
            {
                MockTestGroups = pagedGroups,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling((double)total / filter.PageSize)
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

        public async Task<BulkQuestionFileUploadResultDto> BulkUploadQuestionsFromFileAsync(BulkQuestionFileUploadRequestDto dto, int uploadedBy)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (dto.File == null || dto.File.Length == 0)
            {
                throw new ArgumentException("Bulk upload file is required.", nameof(dto.File));
            }

            var mode = (dto.Mode ?? "create").Trim().ToLowerInvariant();
            if (mode != "create" && mode != "update" && mode != "upsert")
            {
                throw new ArgumentException("Mode must be one of: create, update, upsert.");
            }

            var resolvedExamId = await ResolveExamIdAsync(dto);
            await ValidateExamSubjectTopicMappingAsync(resolvedExamId, dto.SubjectId, dto.TopicId, dto.ModuleId);

            var extension = Path.GetExtension(dto.File.FileName);
            var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{extension}");
            try
            {
                await using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(stream);
                }

                var parsedRows = await ParseBulkUploadFileAsync(tempPath);
                var result = new BulkQuestionFileUploadResultDto
                {
                    TotalRows = parsedRows.Count,
                    ModuleId = dto.ModuleId,
                    MockTestId = dto.MockTestId,
                    ExamId = resolvedExamId,
                    SubjectId = dto.SubjectId,
                    TopicId = dto.TopicId
                };

                foreach (var row in parsedRows)
                {
                    var rowResult = new BulkQuestionRowResultDto { RowNumber = row.RowNumber };
                    try
                    {
                        if (string.IsNullOrWhiteSpace(row.QuestionText))
                        {
                            throw new ArgumentException("QuestionText is required.");
                        }

                        if (string.IsNullOrWhiteSpace(row.CorrectAnswer))
                        {
                            throw new ArgumentException("CorrectAnswer is required.");
                        }

                        switch (mode)
                        {
                            case "create":
                                {
                                    var created = await CreateRowQuestionAsync(row, dto, uploadedBy, resolvedExamId);
                                    rowResult.Success = true;
                                    rowResult.QuestionId = created;
                                    rowResult.Action = "created";
                                    break;
                                }
                            case "update":
                                {
                                    var updated = await UpdateRowQuestionAsync(row, dto, resolvedExamId);
                                    rowResult.Success = true;
                                    rowResult.QuestionId = updated;
                                    rowResult.Action = "updated";
                                    break;
                                }
                            default:
                                {
                                    if (row.QuestionId.HasValue && row.QuestionId.Value > 0)
                                    {
                                        var updated = await UpdateRowQuestionAsync(row, dto, resolvedExamId);
                                        rowResult.Success = true;
                                        rowResult.QuestionId = updated;
                                        rowResult.Action = "updated";
                                    }
                                    else
                                    {
                                        var created = await CreateRowQuestionAsync(row, dto, uploadedBy, resolvedExamId);
                                        rowResult.Success = true;
                                        rowResult.QuestionId = created;
                                        rowResult.Action = "created";
                                    }
                                    break;
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        rowResult.Success = false;
                        rowResult.Error = ex.Message;
                        _logger.LogWarning(ex, "Bulk question row failed. RowNumber: {RowNumber}", row.RowNumber);
                        if (!dto.ContinueOnError)
                        {
                            result.Rows.Add(rowResult);
                            break;
                        }
                    }

                    result.Rows.Add(rowResult);
                }

                result.SuccessCount = result.Rows.Count(x => x.Success);
                result.FailedCount = result.Rows.Count(x => !x.Success);
                return result;
            }
            finally
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
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
                    await ValidateExamSubjectTopicMappingAsync(dto.ExamId, dto.SubjectId.Value, dto.TopicId, 1); // Default to Mock Test module to skip topic validation
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
                await ValidateExamSubjectTopicMappingAsync(dto.ExamId, examDetails.SubjectId, null, 1); // Default to Mock Test module to skip topic validation

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

        private async Task<int> ResolveExamIdAsync(BulkQuestionFileUploadRequestDto dto)
        {
            if (dto.MockTestId.HasValue && dto.MockTestId.Value > 0)
            {
                var mockTest = await _mockTestRepository.GetByIdAsync(dto.MockTestId.Value);
                if (mockTest == null)
                {
                    throw new ArgumentException($"Mock test with ID {dto.MockTestId.Value} not found.");
                }

                return mockTest.ExamId;
            }

            if (dto.ExamId.HasValue && dto.ExamId.Value > 0)
            {
                return dto.ExamId.Value;
            }

            throw new ArgumentException("Either ExamId or MockTestId is required.");
        }

        private async Task<int> CreateRowQuestionAsync(
            ExcelQuestionRowDto row,
            BulkQuestionFileUploadRequestDto request,
            int uploadedBy,
            int resolvedExamId)
        {
            var createDto = new CreateQuestionRequestDto
            {
                ModuleId = request.ModuleId,
                ExamId = resolvedExamId,
                SubjectId = request.SubjectId,
                TopicId = request.TopicId ?? 0,
                QuestionText = row.QuestionText.Trim(),
                OptionA = row.OptionA ?? string.Empty,
                OptionB = row.OptionB ?? string.Empty,
                OptionC = row.OptionC ?? string.Empty,
                OptionD = row.OptionD ?? string.Empty,
                CorrectAnswer = NormalizeCorrectAnswer(row.CorrectAnswer),
                Explanation = row.Explanation ?? string.Empty,
                Marks = row.Marks <= 0 ? 1 : row.Marks,
                NegativeMarks = row.NegativeMarks,
                DifficultyLevel = NormalizeDifficulty(row.DifficultyLevel),
                QuestionType = string.IsNullOrWhiteSpace(row.QuestionType) ? "MCQ" : row.QuestionType,
                QuestionImageUrl = row.QuestionImageUrl,
                OptionAImageUrl = row.OptionAImageUrl,
                OptionBImageUrl = row.OptionBImageUrl,
                OptionCImageUrl = row.OptionCImageUrl,
                OptionDImageUrl = row.OptionDImageUrl,
                ExplanationImageUrl = row.ExplanationImageUrl,
                SameExplanationForAllLanguages = row.SameExplanationForAllLanguages,
                Reference = row.Reference,
                Tags = row.Tags,
                CreatedBy = uploadedBy,
                MockTestId = request.MockTestId,
                Translations = new List<QuestionTranslationUpsertDto>
                {
                    new()
                    {
                        LanguageCode = string.IsNullOrWhiteSpace(request.LanguageCode) ? "en" : request.LanguageCode,
                        QuestionText = row.QuestionText.Trim(),
                        OptionA = row.OptionA,
                        OptionB = row.OptionB,
                        OptionC = row.OptionC,
                        OptionD = row.OptionD,
                        Explanation = row.Explanation
                    }
                }
            };

            var created = await CreateAdminQuestionAsync(createDto);
            return created.Id;
        }

        private async Task<int> UpdateRowQuestionAsync(
            ExcelQuestionRowDto row,
            BulkQuestionFileUploadRequestDto request,
            int resolvedExamId)
        {
            if (!row.QuestionId.HasValue || row.QuestionId.Value <= 0)
            {
                throw new ArgumentException("QuestionId is required for update mode.");
            }

            var questionId = row.QuestionId.Value;
            var existing = await _adminRepository.GetAdminQuestionByIdAsync(questionId);
            if (existing == null)
            {
                throw new ArgumentException($"Question with ID {questionId} not found.");
            }

            var updateDto = new UpdateQuestionAdminDto
            {
                Id = questionId,
                ModuleId = request.ModuleId,
                ExamId = resolvedExamId,
                SubjectId = request.SubjectId,
                TopicId = request.TopicId ?? 0,
                Marks = Convert.ToInt32(Math.Round(row.Marks <= 0 ? (decimal)existing.Marks : row.Marks, MidpointRounding.AwayFromZero)),
                NegativeMarks = row.NegativeMarks,
                DifficultyLevel = NormalizeDifficulty(row.DifficultyLevel),
                CorrectAnswer = NormalizeCorrectAnswer(row.CorrectAnswer),
                SameExplanationForAllLanguages = row.SameExplanationForAllLanguages,
                IsPublished = existing.IsPublished,
                IsActive = existing.IsActive,
                MockTestId = request.MockTestId,
                CreatedBy = 0,
                Translations = new List<QuestionTranslationUpsertDto>
                {
                    new()
                    {
                        LanguageCode = string.IsNullOrWhiteSpace(request.LanguageCode) ? "en" : request.LanguageCode,
                        QuestionText = row.QuestionText.Trim(),
                        OptionA = row.OptionA,
                        OptionB = row.OptionB,
                        OptionC = row.OptionC,
                        OptionD = row.OptionD,
                        Explanation = row.Explanation
                    }
                }
            };

            var updated = await UpdateAdminQuestionAsync(updateDto);
            return updated.Id;
        }

        private static string NormalizeCorrectAnswer(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer))
            {
                throw new ArgumentException("CorrectAnswer is required.");
            }

            var normalized = answer.Trim().ToUpperInvariant();
            if (normalized is "A" or "B" or "C" or "D")
            {
                return normalized;
            }

            throw new ArgumentException("CorrectAnswer must be one of A, B, C, D.");
        }

        private static string NormalizeDifficulty(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "Medium";
            }

            var normalized = value.Trim();
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(normalized.ToLowerInvariant()) switch
            {
                "Easy" => "Easy",
                "Hard" => "Hard",
                _ => "Medium"
            };
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
            // For ModuleId 1 (Mock Test), 2, 4 - Topic is optional and not validated
            if (moduleId == 3 && (!topicId.HasValue || topicId.Value <= 0))
            {
                throw new ArgumentException($"Topic is required for ModuleId 3 (Deep Practice).");
            }

            // Only validate topic mapping if topic is provided AND moduleId is 3 (Deep Practice)
            // For mock tests and previous year questions, topic validation is skipped
            if (topicId.HasValue && moduleId == 3)
            {
                var topicMapped = await _featureRepository.IsTopicMappedToExamSubjectAsync(topicId.Value, examId, subjectId);
                if (!topicMapped)
                {
                    throw new ArgumentException($"Topic {topicId.Value} is not mapped to exam {examId} and subject {subjectId}.");
                }
            }
        }

        private async Task AttachQuestionToMockTestIfRequiredAsync(
            int questionId,
            int? mockTestId,
            decimal marks,
            decimal negativeMarks)
        {
            if (!mockTestId.HasValue || mockTestId.Value <= 0)
            {
                return;
            }

            var resolvedMockTestId = mockTestId.Value;
            var mockTest = await _mockTestRepository.GetByIdAsync(resolvedMockTestId);
            if (mockTest == null)
            {
                throw new ArgumentException($"Mock test with ID {resolvedMockTestId} not found.");
            }

            var existingMappings = await _mockTestRepository.GetQuestionsAsync(resolvedMockTestId);
            var existingMapping = existingMappings.FirstOrDefault(x => x.QuestionId == questionId);

            if (existingMapping != null)
            {
                await _mockTestRepository.UpdateQuestionAsync(
                    resolvedMockTestId,
                    questionId,
                    existingMapping.QuestionNumber,
                    marks,
                    negativeMarks);
                return;
            }

            var questionNumber = existingMappings.Count == 0
                ? 1
                : existingMappings.Max(x => x.QuestionNumber) + 1;

            await _mockTestRepository.AddQuestionAsync(
                resolvedMockTestId,
                questionId,
                questionNumber,
                marks,
                negativeMarks);
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
        Task<MockTestGroupedQuestionResponseDto> GetAdminQuestionsGroupedByMockTestAsync(QuestionFilterRequestDto filter);
        Task<bool> SetPublishStatusAsync(PublishQuestionRequestDto dto);
        Task<QuestionDashboardStatsDto> GetDashboardStatsAsync();
        Task<object> BulkCreateQuestionsAsync(BulkQuestionUploadRequestDto dto);
        Task<BulkQuestionFileUploadResultDto> BulkUploadQuestionsFromFileAsync(BulkQuestionFileUploadRequestDto dto, int uploadedBy);
        
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
