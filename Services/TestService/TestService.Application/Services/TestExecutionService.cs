using AutoMapper;
using System.Text.Json;
using TestService.Application.DTOs;
using TestService.Application.Interfaces;
using TestService.Domain.Entities;
using TestService.Domain.Enums;
using TestService.Domain.Interfaces;

namespace TestService.Application.Services
{
    public class TestExecutionService
    {
        private readonly ITestRepository _testRepository;
        private readonly IUserTestAttemptRepository _userTestAttemptRepository;
        private readonly ITestQuestionRepository _testQuestionRepository;
        private readonly IAttemptAnswerRepository _attemptAnswerRepository;
        private readonly ISubscriptionValidationClient _subscriptionValidationClient;
        private readonly IQuestionEvaluationClient _questionEvaluationClient;
        private readonly IMapper _mapper;

        public TestExecutionService(
            ITestRepository testRepository,
            IUserTestAttemptRepository userTestAttemptRepository,
            ITestQuestionRepository testQuestionRepository,
            IAttemptAnswerRepository attemptAnswerRepository,
            ISubscriptionValidationClient subscriptionValidationClient,
            IQuestionEvaluationClient questionEvaluationClient,
            IMapper mapper)
        {
            _testRepository = testRepository;
            _userTestAttemptRepository = userTestAttemptRepository;
            _testQuestionRepository = testQuestionRepository;
            _attemptAnswerRepository = attemptAnswerRepository;
            _subscriptionValidationClient = subscriptionValidationClient;
            _questionEvaluationClient = questionEvaluationClient;
            _mapper = mapper;
        }

        public async Task<UserTestAttemptDto> StartTestAsync(int testId, int userId)
        {
            var test = await _testRepository.GetByIdAsync(testId);
            if (test == null || !test.IsActive)
            {
                throw new KeyNotFoundException($"Test with ID {testId} not found or inactive");
            }

            if (test.IsLocked)
            {
                var validation = await _subscriptionValidationClient.ValidateAsync(userId);
                if (validation == null || !validation.IsValid || !validation.HasActiveSubscription)
                {
                    throw new InvalidOperationException(validation?.Message ?? "Active subscription is required to start this test");
                }
            }

            var existingAttempt = (await _userTestAttemptRepository.GetOngoingByUserIdAsync(userId))
                .FirstOrDefault(a => a.TestId == testId && a.Status == TestAttemptStatus.InProgress);

            if (existingAttempt != null)
            {
                throw new InvalidOperationException("User already has an ongoing attempt for this test");
            }

            var attempt = new UserTestAttempt
            {
                TestId = testId,
                UserId = userId,
                StartedAt = DateTime.UtcNow,
                CurrentQuestionIndex = 0,
                Score = 0,
                TotalMarks = (int)test.TotalMarks,
                Accuracy = 0,
                Status = TestAttemptStatus.InProgress,
                AnswersJson = JsonSerializer.Serialize(new Dictionary<int, string>()),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userTestAttemptRepository.AddAsync(attempt);

            var attemptDto = _mapper.Map<UserTestAttemptDto>(attempt);
            attemptDto.Test = _mapper.Map<TestDto>(test);
            return attemptDto;
        }

        public async Task<SaveAnswerActionResultDto> SaveAnswerActionAsync(int attemptId, SaveAnswerActionRequestDto request, int userId)
        {
            var attempt = await GetAuthorizedAttemptAsync(attemptId, userId);
            await EnsureAttemptNotExpiredAsync(attempt);

            var action = ResolveAction(request);
            var normalizedAnswer = action == SaveAnswerAction.Clear ? null : NormalizeAnswer(request.Answer);
            var isAnswered = !string.IsNullOrWhiteSpace(normalizedAnswer);
            var isMarkedForReview = action == SaveAnswerAction.MarkAndNext || (request.MarkForReview && action != SaveAnswerAction.Clear);

            attempt.AnswersJson = UpdateAnswersJson(attempt.AnswersJson, request.QuestionId, normalizedAnswer);
            attempt.CurrentQuestionIndex = await ResolveCurrentQuestionIndexAsync(attempt, request.QuestionId, action);
            attempt.UpdatedAt = DateTime.UtcNow;

            await _attemptAnswerRepository.SaveAsync(attemptId, request.QuestionId, normalizedAnswer, isMarkedForReview, isAnswered);
            await _userTestAttemptRepository.UpdateAsync(attempt);

            return new SaveAnswerActionResultDto
            {
                AttemptId = attemptId,
                QuestionId = request.QuestionId,
                Action = action.ToString(),
                CurrentQuestionIndex = attempt.CurrentQuestionIndex,
                IsMarkedForReview = isMarkedForReview,
                IsAnswered = isAnswered,
                SavedAnswer = normalizedAnswer,
                SavedAtUtc = attempt.UpdatedAt ?? DateTime.UtcNow
            };
        }

        public async Task SaveAnswerAsync(int attemptId, int questionId, string? answer, int userId)
        {
            await SaveAnswerActionAsync(attemptId, new SaveAnswerActionRequestDto
            {
                QuestionId = questionId,
                Answer = answer,
                Action = SaveAnswerAction.SaveAndNext.ToString()
            }, userId);
        }

        public async Task<TestResultDto> SubmitTestAsync(int attemptId, int userId)
        {
            var attempt = await GetAuthorizedAttemptAsync(attemptId, userId);
            if (attempt.Status != TestAttemptStatus.InProgress)
            {
                throw new InvalidOperationException("Cannot submit a test that is not in progress");
            }

            var score = await CalculateScoreAsync(attempt);

            attempt.Score = score.TotalScore;
            attempt.Accuracy = score.Accuracy;
            attempt.Status = TestAttemptStatus.Completed;
            attempt.CompletedAt = DateTime.UtcNow;
            attempt.UpdatedAt = DateTime.UtcNow;

            await _userTestAttemptRepository.UpdateAsync(attempt);

            return new TestResultDto
            {
                AttemptId = attempt.Id,
                TestId = attempt.TestId,
                TestTitle = attempt.Test.Title,
                TotalQuestions = attempt.Test.TotalQuestions,
                TotalMarks = (int)attempt.Test.TotalMarks,
                Score = score.TotalScore,
                Accuracy = score.Accuracy,
                Passed = score.TotalScore >= attempt.Test.PassingMarks,
                StartedAt = attempt.StartedAt,
                CompletedAt = attempt.CompletedAt,
                DurationMinutes = (int)((attempt.CompletedAt - attempt.StartedAt)?.TotalMinutes ?? 0),
                TimeTakenSeconds = (int)((attempt.CompletedAt - attempt.StartedAt)?.TotalSeconds ?? 0),
                CorrectCount = score.CorrectCount,
                IncorrectCount = score.IncorrectCount,
                SkippedCount = score.SkippedCount,
                QuestionResults = score.QuestionResults
            };
        }

        public async Task<UserTestAttemptDto> GetAttemptAsync(int attemptId, int userId)
        {
            var attempt = await GetAuthorizedAttemptAsync(attemptId, userId);
            var attemptDto = _mapper.Map<UserTestAttemptDto>(attempt);
            attemptDto.Test = _mapper.Map<TestDto>(attempt.Test);
            return attemptDto;
        }

        public async Task<TestQuestionListDto> GetTestQuestionsAsync(int attemptId, int userId)
        {
            var attempt = await GetAuthorizedAttemptAsync(attemptId, userId);
            var testQuestions = await _testQuestionRepository.GetByTestIdAsync(attempt.TestId);
            var attemptAnswers = await _attemptAnswerRepository.GetByAttemptIdAsync(attemptId);
            var answersByQuestion = attemptAnswers.ToDictionary(x => x.QuestionId, x => x);

            var questionDtos = testQuestions.Select(tq => new TestQuestionWithAnswerDto
            {
                Id = tq.Question.Id,
                QuestionText = tq.Question.QuestionText,
                ImageUrl = tq.Question.ImageUrl,
                VideoUrl = tq.Question.VideoUrl,
                Explanation = tq.Question.Explanation,
                Difficulty = (QuestionDifficulty)tq.Question.Difficulty,
                Marks = (int)tq.Marks,
                DisplayOrder = tq.DisplayOrder,
                UserAnswer = answersByQuestion.TryGetValue(tq.QuestionId, out var answer) ? (answer.Answer ?? string.Empty) : string.Empty,
                IsMarkedForReview = answersByQuestion.TryGetValue(tq.QuestionId, out var review) && review.IsMarkedForReview,
                IsAnswered = answersByQuestion.TryGetValue(tq.QuestionId, out var answered) && answered.IsAnswered
            }).OrderBy(q => q.DisplayOrder).ToList();

            return new TestQuestionListDto
            {
                AttemptId = attemptId,
                TestId = attempt.TestId,
                TestTitle = attempt.Test.Title,
                DurationMinutes = attempt.Test.DurationInMinutes,
                TotalQuestions = questionDtos.Count,
                StartedAt = attempt.StartedAt,
                CurrentQuestionIndex = attempt.CurrentQuestionIndex,
                Questions = questionDtos
            };
        }

        private async Task<UserTestAttempt> GetAuthorizedAttemptAsync(int attemptId, int userId)
        {
            var attempt = await _userTestAttemptRepository.GetByIdWithTestAsync(attemptId);
            if (attempt == null || !attempt.IsActive)
            {
                throw new KeyNotFoundException($"Test attempt with ID {attemptId} not found or inactive");
            }

            if (attempt.UserId != userId)
            {
                throw new UnauthorizedAccessException("User is not authorized to access this attempt");
            }

            return attempt;
        }

        private async Task EnsureAttemptNotExpiredAsync(UserTestAttempt attempt)
        {
            if (attempt.Status != TestAttemptStatus.InProgress)
            {
                throw new InvalidOperationException("Cannot save answer for a test that is not in progress");
            }

            if (attempt.StartedAt.HasValue && DateTime.UtcNow > attempt.StartedAt.Value.AddMinutes(attempt.Test.DurationInMinutes))
            {
                attempt.Status = TestAttemptStatus.Expired;
                attempt.UpdatedAt = DateTime.UtcNow;
                await _userTestAttemptRepository.UpdateAsync(attempt);
                throw new InvalidOperationException("Test time has expired");
            }
        }

        private async Task<TestScoreResult> CalculateScoreAsync(UserTestAttempt attempt)
        {
            var testQuestions = (await _testQuestionRepository.GetByTestIdAsync(attempt.TestId)).ToList();
            var attemptAnswers = await _attemptAnswerRepository.GetByAttemptIdAsync(attempt.Id);
            var answersByQuestion = attemptAnswers.ToDictionary(x => x.QuestionId, x => NormalizeAnswer(x.Answer) ?? string.Empty);
            var questionMetadata = await _questionEvaluationClient.GetMetadataByIdsAsync(testQuestions.Select(x => x.QuestionId));

            var totalScore = 0;
            var correctAnswers = 0;
            var incorrectAnswers = 0;
            var skippedAnswers = 0;
            var questionResults = new List<QuestionResultDto>();

            foreach (var testQuestion in testQuestions)
            {
                var userAnswer = answersByQuestion.GetValueOrDefault(testQuestion.QuestionId, string.Empty);
                var isSkipped = string.IsNullOrWhiteSpace(userAnswer);
                questionMetadata.TryGetValue(testQuestion.QuestionId, out var metadata);
                var correctAnswer = NormalizeAnswer(metadata?.CorrectAnswer) ?? string.Empty;
                var isCorrect = !isSkipped && CheckAnswer(correctAnswer, userAnswer);

                if (isCorrect)
                {
                    totalScore += (int)testQuestion.Marks;
                    correctAnswers++;
                }
                else if (isSkipped)
                {
                    skippedAnswers++;
                }
                else
                {
                    incorrectAnswers++;
                }

                questionResults.Add(new QuestionResultDto
                {
                    QuestionId = testQuestion.QuestionId,
                    QuestionText = testQuestion.Question.QuestionText,
                    UserAnswer = userAnswer,
                    CorrectAnswer = correctAnswer,
                    IsCorrect = isCorrect,
                    Marks = (int)testQuestion.Marks,
                    Explanation = metadata?.ExplanationEnglish ?? testQuestion.Question.Explanation
                });
            }

            var totalQuestions = testQuestions.Count;
            var accuracy = totalQuestions > 0 ? (decimal)(correctAnswers * 100.0 / totalQuestions) : 0;

            return new TestScoreResult
            {
                TotalScore = totalScore,
                Accuracy = accuracy,
                CorrectCount = correctAnswers,
                IncorrectCount = incorrectAnswers,
                SkippedCount = skippedAnswers,
                QuestionResults = questionResults
            };
        }

        private async Task<int> ResolveCurrentQuestionIndexAsync(UserTestAttempt attempt, int questionId, SaveAnswerAction action)
        {
            if (action == SaveAnswerAction.Clear)
            {
                return attempt.CurrentQuestionIndex;
            }

            var orderedQuestions = (await _testQuestionRepository.GetByTestIdAsync(attempt.TestId))
                .OrderBy(q => q.DisplayOrder)
                .ToList();

            var currentIndex = orderedQuestions.FindIndex(q => q.QuestionId == questionId);
            if (currentIndex < 0)
            {
                return attempt.CurrentQuestionIndex;
            }

            return Math.Min(currentIndex + 1, Math.Max(orderedQuestions.Count - 1, 0));
        }

        private static SaveAnswerAction ResolveAction(SaveAnswerActionRequestDto request)
        {
            if (request.ClearResponse)
            {
                return SaveAnswerAction.Clear;
            }

            if (Enum.TryParse<SaveAnswerAction>(request.Action, true, out var parsed))
            {
                return parsed;
            }

            if (request.MarkForReview)
            {
                return SaveAnswerAction.MarkAndNext;
            }

            return SaveAnswerAction.SaveAndNext;
        }

        private static string? NormalizeAnswer(string? answer)
        {
            return string.IsNullOrWhiteSpace(answer) ? null : answer.Trim();
        }

        private static string UpdateAnswersJson(string? answersJson, int questionId, string? answer)
        {
            var answers = new Dictionary<int, string>();
            if (!string.IsNullOrWhiteSpace(answersJson))
            {
                try
                {
                    answers = JsonSerializer.Deserialize<Dictionary<int, string>>(answersJson) ?? new Dictionary<int, string>();
                }
                catch (JsonException)
                {
                    answers = new Dictionary<int, string>();
                }
            }

            if (string.IsNullOrWhiteSpace(answer))
            {
                answers.Remove(questionId);
            }
            else
            {
                answers[questionId] = answer;
            }

            return JsonSerializer.Serialize(answers);
        }

        private static bool CheckAnswer(string correctAnswer, string userAnswer)
        {
            if (string.IsNullOrWhiteSpace(correctAnswer) || string.IsNullOrWhiteSpace(userAnswer))
            {
                return false;
            }

            return string.Equals(correctAnswer.Trim(), userAnswer.Trim(), StringComparison.OrdinalIgnoreCase);
        }
    }

    internal class TestScoreResult
    {
        public int TotalScore { get; set; }
        public decimal Accuracy { get; set; }
        public int CorrectCount { get; set; }
        public int IncorrectCount { get; set; }
        public int SkippedCount { get; set; }
        public List<QuestionResultDto> QuestionResults { get; set; } = new();
    }

    internal enum SaveAnswerAction
    {
        SaveAndNext,
        MarkAndNext,
        Clear
    }
}
