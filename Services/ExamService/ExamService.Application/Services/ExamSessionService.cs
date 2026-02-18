using Microsoft.Extensions.Logging;
using Common.Language;
using Common.Services;
using ExamService.Application.Interfaces;
using ExamService.Domain.Entities;

namespace ExamService.Application.Services
{
    public interface IExamSessionService
    {
        Task<ExamSession> StartExamAsync(int userId, int examId, string language);
        Task<ExamSession?> GetExamSessionAsync(int sessionId);
        Task<ExamSession?> GetActiveExamSessionAsync(int userId);
        Task<bool> CanChangeLanguageAsync(int sessionId);
        Task<ExamSession> PauseExamAsync(int sessionId);
        Task<ExamSession> ResumeExamAsync(int sessionId);
        Task<ExamSession> CompleteExamAsync(int sessionId);
    }

    public class ExamSessionService : IExamSessionService
    {
        private readonly IExamSessionRepository _examSessionRepository;
        private readonly ILanguageService _languageService;
        private readonly ILogger<ExamSessionService> _logger;

        public ExamSessionService(
            IExamSessionRepository examSessionRepository,
            ILanguageService languageService,
            ILogger<ExamSessionService> logger)
        {
            _examSessionRepository = examSessionRepository;
            _languageService = languageService;
            _logger = logger;
        }

        public async Task<ExamSession> StartExamAsync(int userId, int examId, string language)
        {
            // Validate language
            LanguageValidator.ValidateLanguage(language, nameof(language));

            // Check if user already has an active exam session
            var existingSession = await GetActiveExamSessionAsync(userId);
            if (existingSession != null)
            {
                throw new InvalidOperationException("User already has an active exam session");
            }

            var examSession = new ExamSession
            {
                UserId = userId,
                ExamId = examId,
                ExamLanguage = language,
                StartTime = DateTime.UtcNow,
                DurationInMinutes = 60 // This should come from exam configuration
            };

            var createdSession = await _examSessionRepository.AddAsync(examSession);
            _logger.LogInformation("Started exam session {SessionId} for user {UserId} in language {Language}", 
                createdSession.Id, userId, language);

            return createdSession;
        }

        public async Task<ExamSession?> GetExamSessionAsync(int sessionId)
        {
            return await _examSessionRepository.GetByIdAsync(sessionId);
        }

        public async Task<ExamSession?> GetActiveExamSessionAsync(int userId)
        {
            return await _examSessionRepository.GetActiveSessionByUserIdAsync(userId);
        }

        public async Task<bool> CanChangeLanguageAsync(int sessionId)
        {
            var session = await GetExamSessionAsync(sessionId);
            if (session == null)
                return false;

            // Language can only be changed before exam starts or if exam is paused
            return !session.IsCompleted && session.StartTime > DateTime.UtcNow.AddMinutes(-1);
        }

        public async Task<ExamSession> PauseExamAsync(int sessionId)
        {
            var session = await GetExamSessionAsync(sessionId);
            if (session == null)
                throw new KeyNotFoundException($"Exam session {sessionId} not found");

            if (session.IsCompleted)
                throw new InvalidOperationException("Cannot pause completed exam");

            session.IsPaused = true;
            session.LastPauseTime = DateTime.UtcNow;

            return await _examSessionRepository.UpdateAsync(session);
        }

        public async Task<ExamSession> ResumeExamAsync(int sessionId)
        {
            var session = await GetExamSessionAsync(sessionId);
            if (session == null)
                throw new KeyNotFoundException($"Exam session {sessionId} not found");

            if (!session.IsPaused)
                throw new InvalidOperationException("Exam is not paused");

            if (session.LastPauseTime.HasValue)
            {
                var pauseDuration = DateTime.UtcNow - session.LastPauseTime.Value;
                session.TotalPauseDuration += (int)pauseDuration.TotalSeconds;
            }

            session.IsPaused = false;
            session.LastPauseTime = null;

            return await _examSessionRepository.UpdateAsync(session);
        }

        public async Task<ExamSession> CompleteExamAsync(int sessionId)
        {
            var session = await GetExamSessionAsync(sessionId);
            if (session == null)
                throw new KeyNotFoundException($"Exam session {sessionId} not found");

            session.IsCompleted = true;
            session.EndTime = DateTime.UtcNow;

            return await _examSessionRepository.UpdateAsync(session);
        }
    }
}
