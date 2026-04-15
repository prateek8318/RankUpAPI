using AutoMapper;
using Microsoft.Extensions.Logging;
using QuestionService.Application.DTOs;
using QuestionService.Application.Interfaces;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces;

namespace QuestionService.Application.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly Domain.Interfaces.IQuestionRepository _questionRepository;
        private readonly Domain.Interfaces.ITopicRepository _topicRepository;
        private readonly Domain.Interfaces.IQuestionTranslationRepository _translationRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<QuestionService> _logger;

        public QuestionService(
            Domain.Interfaces.IQuestionRepository questionRepository,
            Domain.Interfaces.ITopicRepository topicRepository,
            Domain.Interfaces.IQuestionTranslationRepository translationRepository,
            IMapper mapper,
            ILogger<QuestionService> logger)
        {
            _questionRepository = questionRepository;
            _topicRepository = topicRepository;
            _translationRepository = translationRepository;
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
            var createdTopic = await _topicRepository.CreateTopicAsync(dto.Name, dto.SubjectId, dto.Description, dto.ParentTopicId, dto.SortOrder);
            return _mapper.Map<TopicDto>(createdTopic);
        }

        public async Task<IEnumerable<TopicDto>> GetTopicsAsync(int subjectId)
        {
            var topics = await _topicRepository.GetTopicsBySubjectAsync(subjectId);
            return _mapper.Map<IEnumerable<TopicDto>>(topics);
        }

        public async Task<QuestionAdminDetailDto> CreateAdminQuestionAsync(CreateQuestionAdminDto dto)
        {
            var createdQuestion = await _questionRepository.CreateQuestionAsync(
                dto.Translations?.FirstOrDefault()?.QuestionText ?? string.Empty,
                dto.Translations?.FirstOrDefault()?.OptionA,
                dto.Translations?.FirstOrDefault()?.OptionB,
                dto.Translations?.FirstOrDefault()?.OptionC,
                dto.Translations?.FirstOrDefault()?.OptionD,
                dto.CorrectAnswer,
                dto.Marks,
                dto.ExamId,
                dto.SubjectId,
                dto.TopicId,
                dto.DifficultyLevel,
                dto.CreatedBy);
            return _mapper.Map<QuestionAdminDetailDto>(createdQuestion);
        }

        public async Task<QuestionAdminDetailDto> UpdateAdminQuestionAsync(UpdateQuestionAdminDto dto)
        {
            var existingQuestion = await _questionRepository.GetByIdAsync(dto.Id);
            if (existingQuestion == null)
                throw new Exception($"Question with ID {dto.Id} not found");

            var updated = await _questionRepository.UpdateQuestionAsync(dto.Id, null, null, null, null, null, null, dto.Marks, dto.DifficultyLevel);
            if (!updated)
                throw new Exception($"Failed to update question with ID {dto.Id}");

            var updatedQuestion = await _questionRepository.GetByIdAsync(dto.Id);
            return _mapper.Map<QuestionAdminDetailDto>(updatedQuestion);
        }

        public async Task<QuestionAdminDetailDto?> GetAdminQuestionByIdAsync(int id)
        {
            var question = await _questionRepository.GetByIdAsync(id);
            return question == null ? null : _mapper.Map<QuestionAdminDetailDto>(question);
        }

        public async Task<QuestionPagedResponseDto> GetAdminQuestionsPagedAsync(QuestionFilterRequestDto filter)
        {
            var (questions, totalCount) = await _questionRepository.GetQuestionsPagedAsync(
                filter.PageNumber, filter.PageSize, filter.ExamId, filter.SubjectId, filter.TopicId, 
                filter.DifficultyLevel, filter.IsPublished, filter.LanguageCode);
            return new QuestionPagedResponseDto
            {
                Items = _mapper.Map<IReadOnlyList<QuestionDto>>(questions),
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
            };
        }

        public async Task<bool> SetPublishStatusAsync(PublishQuestionRequestDto dto)
        {
            var result = await _questionRepository.TogglePublishStatusAsync(dto.QuestionId, dto.IsPublished, dto.ReviewedBy);
            return result;
        }

        public async Task<QuestionDashboardStatsDto> GetDashboardStatsAsync()
        {
            var (totalQuestions, addedToday, negativeMarksCount, unpublishedCount) = await _questionRepository.GetStatisticsAsync();
            return new QuestionDashboardStatsDto
            {
                TotalQuestions = totalQuestions,
                AddedToday = addedToday,
                NegativeMarksCount = negativeMarksCount,
                UnpublishedCount = unpublishedCount
            };
        }

        public async Task<object> BulkCreateQuestionsAsync(BulkQuestionUploadRequestDto dto)
        {
            // Implementation for bulk upload would go here
            // For now, return a placeholder response
            return new { success = true, message = "Bulk upload feature not yet implemented" };
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
    }
}
