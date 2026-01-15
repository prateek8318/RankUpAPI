using AutoMapper;
using HomeDashboardService.Application.DTOs;
using HomeDashboardService.Application.Interfaces;
using HomeDashboardService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace HomeDashboardService.Application.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IChapterRepository _chapterRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<QuizService> _logger;

        public QuizService(
            IQuizRepository quizRepository,
            IChapterRepository chapterRepository,
            IMapper mapper,
            ILogger<QuizService> logger)
        {
            _quizRepository = quizRepository;
            _chapterRepository = chapterRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<QuizDto> CreateQuizAsync(CreateQuizDto createQuizDto)
        {
            try
            {
                var chapter = await _chapterRepository.GetByIdAsync(createQuizDto.ChapterId);
                if (chapter == null)
                    throw new KeyNotFoundException($"Chapter with ID {createQuizDto.ChapterId} not found");

                var quiz = _mapper.Map<Domain.Entities.Quiz>(createQuizDto);
                var created = await _quizRepository.AddAsync(quiz);
                await _quizRepository.SaveChangesAsync();
                return _mapper.Map<QuizDto>(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating quiz");
                throw;
            }
        }

        public async Task<QuizDto?> UpdateQuizAsync(int id, UpdateQuizDto updateQuizDto)
        {
            try
            {
                var quiz = await _quizRepository.GetByIdWithQuestionsAsync(id);
                if (quiz == null)
                    return null;

                _mapper.Map(updateQuizDto, quiz);
                quiz.UpdatedAt = DateTime.UtcNow;
                await _quizRepository.UpdateAsync(quiz);
                await _quizRepository.SaveChangesAsync();
                return _mapper.Map<QuizDto>(quiz);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quiz: {QuizId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteQuizAsync(int id)
        {
            try
            {
                return await _quizRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting quiz: {QuizId}", id);
                throw;
            }
        }

        public async Task<bool> EnableDisableQuizAsync(int id, bool isActive)
        {
            try
            {
                var quiz = await _quizRepository.GetByIdAsync(id);
                if (quiz == null)
                    return false;

                quiz.IsActive = isActive;
                quiz.UpdatedAt = DateTime.UtcNow;
                await _quizRepository.UpdateAsync(quiz);
                await _quizRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling/disabling quiz: {QuizId}", id);
                throw;
            }
        }

        public async Task<QuizDto?> GetQuizByIdAsync(int id)
        {
            try
            {
                var quiz = await _quizRepository.GetByIdWithQuestionsAsync(id);
                return quiz != null ? _mapper.Map<QuizDto>(quiz) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quiz: {QuizId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<QuizDto>> GetQuizzesByChapterIdAsync(int chapterId)
        {
            try
            {
                var quizzes = await _quizRepository.GetByChapterIdAsync(chapterId);
                return _mapper.Map<IEnumerable<QuizDto>>(quizzes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quizzes for chapter: {ChapterId}", chapterId);
                throw;
            }
        }
    }
}
