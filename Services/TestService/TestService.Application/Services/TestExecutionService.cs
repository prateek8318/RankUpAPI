using AutoMapper;
using System.Text.Json;
using TestService.Application.DTOs;
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
        private readonly IMapper _mapper;

        public TestExecutionService(
            ITestRepository testRepository,
            IUserTestAttemptRepository userTestAttemptRepository,
            ITestQuestionRepository testQuestionRepository,
            IMapper mapper)
        {
            _testRepository = testRepository;
            _userTestAttemptRepository = userTestAttemptRepository;
            _testQuestionRepository = testQuestionRepository;
            _mapper = mapper;
        }

        public async Task<UserTestAttemptDto> StartTestAsync(int testId, int userId)
        {
            // Check if test exists and is active
            var test = await _testRepository.GetByIdAsync(testId);
            if (test == null || !test.IsActive)
                throw new KeyNotFoundException($"Test with ID {testId} not found or inactive");

            // Check if user has an ongoing attempt
            var existingAttempt = (await _userTestAttemptRepository.GetOngoingByUserIdAsync(userId))
                .FirstOrDefault(a => a.TestId == testId && a.Status == TestAttemptStatus.InProgress);

            if (existingAttempt != null)
                throw new InvalidOperationException("User already has an ongoing attempt for this test");

            // Create new attempt
            var attempt = new UserTestAttempt
            {
                TestId = testId,
                UserId = userId,
                StartedAt = DateTime.UtcNow,
                CurrentQuestionIndex = 0,
                Score = 0,
                TotalMarks = test.TotalMarks,
                Accuracy = 0,
                Status = TestAttemptStatus.InProgress,
                AnswersJson = JsonSerializer.Serialize(new Dictionary<int, string>()),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userTestAttemptRepository.AddAsync(attempt);
            await _userTestAttemptRepository.SaveChangesAsync();

            var attemptDto = _mapper.Map<UserTestAttemptDto>(attempt);
            attemptDto.Test = _mapper.Map<TestDto>(test);

            return attemptDto;
        }

        public async Task SaveAnswerAsync(int attemptId, int questionId, string? answer, int userId)
        {
            var attempt = await _userTestAttemptRepository.GetByIdWithTestAsync(attemptId);
            if (attempt == null || !attempt.IsActive)
                throw new KeyNotFoundException($"Test attempt with ID {attemptId} not found or inactive");

            if (attempt.UserId != userId)
                throw new UnauthorizedAccessException("User is not authorized to access this attempt");

            if (attempt.Status != TestAttemptStatus.InProgress)
                throw new InvalidOperationException("Cannot save answer for a test that is not in progress");

            // Check if test is expired
            if (attempt.StartedAt.HasValue && DateTime.UtcNow > attempt.StartedAt.Value.AddMinutes(attempt.Test.DurationInMinutes))
            {
                attempt.Status = TestAttemptStatus.Expired;
                attempt.UpdatedAt = DateTime.UtcNow;
                await _userTestAttemptRepository.UpdateAsync(attempt);
                await _userTestAttemptRepository.SaveChangesAsync();
                throw new InvalidOperationException("Test time has expired");
            }

            // Parse existing answers
            var answers = new Dictionary<int, string>();
            if (!string.IsNullOrEmpty(attempt.AnswersJson))
            {
                try
                {
                    answers = JsonSerializer.Deserialize<Dictionary<int, string>>(attempt.AnswersJson) ?? new Dictionary<int, string>();
                }
                catch (JsonException)
                {
                    answers = new Dictionary<int, string>();
                }
            }

            // Update answer
            answers[questionId] = answer ?? string.Empty;
            attempt.AnswersJson = JsonSerializer.Serialize(answers);
            attempt.UpdatedAt = DateTime.UtcNow;

            await _userTestAttemptRepository.UpdateAsync(attempt);
            await _userTestAttemptRepository.SaveChangesAsync();
        }

        public async Task<TestResultDto> SubmitTestAsync(int attemptId, int userId)
        {
            var attempt = await _userTestAttemptRepository.GetByIdWithTestAsync(attemptId);
            if (attempt == null || !attempt.IsActive)
                throw new KeyNotFoundException($"Test attempt with ID {attemptId} not found or inactive");

            if (attempt.UserId != userId)
                throw new UnauthorizedAccessException("User is not authorized to access this attempt");

            if (attempt.Status != TestAttemptStatus.InProgress)
                throw new InvalidOperationException("Cannot submit a test that is not in progress");

            // Calculate score
            var score = await CalculateScoreAsync(attempt);
            
            // Update attempt
            attempt.Score = score.TotalScore;
            attempt.Accuracy = score.Accuracy;
            attempt.Status = TestAttemptStatus.Completed;
            attempt.CompletedAt = DateTime.UtcNow;
            attempt.UpdatedAt = DateTime.UtcNow;

            await _userTestAttemptRepository.UpdateAsync(attempt);
            await _userTestAttemptRepository.SaveChangesAsync();

            return new TestResultDto
            {
                AttemptId = attempt.Id,
                TestId = attempt.TestId,
                TestTitle = attempt.Test.Title,
                TotalQuestions = attempt.Test.TotalQuestions,
                TotalMarks = attempt.Test.TotalMarks,
                Score = score.TotalScore,
                Accuracy = score.Accuracy,
                Passed = score.TotalScore >= attempt.Test.PassingMarks,
                StartedAt = attempt.StartedAt,
                CompletedAt = attempt.CompletedAt,
                DurationMinutes = (int)((attempt.CompletedAt - attempt.StartedAt)?.TotalMinutes ?? 0),
                QuestionResults = score.QuestionResults
            };
        }

        public async Task<UserTestAttemptDto> GetAttemptAsync(int attemptId, int userId)
        {
            var attempt = await _userTestAttemptRepository.GetByIdWithTestAsync(attemptId);
            if (attempt == null || !attempt.IsActive)
                throw new KeyNotFoundException($"Test attempt with ID {attemptId} not found or inactive");

            if (attempt.UserId != userId)
                throw new UnauthorizedAccessException("User is not authorized to access this attempt");

            var attemptDto = _mapper.Map<UserTestAttemptDto>(attempt);
            attemptDto.Test = _mapper.Map<TestDto>(attempt.Test);

            return attemptDto;
        }

        public async Task<TestQuestionListDto> GetTestQuestionsAsync(int attemptId, int userId)
        {
            var attempt = await _userTestAttemptRepository.GetByIdWithTestAsync(attemptId);
            if (attempt == null || !attempt.IsActive)
                throw new KeyNotFoundException($"Test attempt with ID {attemptId} not found or inactive");

            if (attempt.UserId != userId)
                throw new UnauthorizedAccessException("User is not authorized to access this attempt");

            // Get test questions
            var testQuestions = await _testQuestionRepository.GetByTestIdAsync(attempt.TestId);
            
            // Parse user answers
            var answers = new Dictionary<int, string>();
            if (!string.IsNullOrEmpty(attempt.AnswersJson))
            {
                try
                {
                    answers = JsonSerializer.Deserialize<Dictionary<int, string>>(attempt.AnswersJson) ?? new Dictionary<int, string>();
                }
                catch (JsonException)
                {
                    answers = new Dictionary<int, string>();
                }
            }

            var questionDtos = testQuestions.Select(tq => new TestQuestionWithAnswerDto
            {
                Id = tq.Question.Id,
                QuestionText = tq.Question.QuestionText,
                ImageUrl = tq.Question.ImageUrl,
                VideoUrl = tq.Question.VideoUrl,
                Explanation = tq.Question.Explanation,
                Difficulty = (QuestionDifficulty)tq.Question.Difficulty,
                Marks = tq.Marks,
                DisplayOrder = tq.DisplayOrder,
                UserAnswer = answers.GetValueOrDefault(tq.QuestionId, string.Empty)
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

        private async Task<TestScoreResult> CalculateScoreAsync(UserTestAttempt attempt)
        {
            var testQuestions = await _testQuestionRepository.GetByTestIdAsync(attempt.TestId);
            
            // Parse answers
            var answers = new Dictionary<int, string>();
            if (!string.IsNullOrEmpty(attempt.AnswersJson))
            {
                try
                {
                    answers = JsonSerializer.Deserialize<Dictionary<int, string>>(attempt.AnswersJson) ?? new Dictionary<int, string>();
                }
                catch (JsonException)
                {
                    answers = new Dictionary<int, string>();
                }
            }

            var totalScore = 0;
            var totalPossibleMarks = 0;
            var correctAnswers = 0;
            var questionResults = new List<QuestionResultDto>();

            foreach (var testQuestion in testQuestions)
            {
                totalPossibleMarks += testQuestion.Marks;
                
                var userAnswer = answers.GetValueOrDefault(testQuestion.QuestionId, string.Empty);
                var isCorrect = CheckAnswer(testQuestion.Question, userAnswer);
                
                if (isCorrect)
                {
                    totalScore += testQuestion.Marks;
                    correctAnswers++;
                }

                questionResults.Add(new QuestionResultDto
                {
                    QuestionId = testQuestion.QuestionId,
                    QuestionText = testQuestion.Question.QuestionText,
                    UserAnswer = userAnswer,
                    CorrectAnswer = GetCorrectAnswer(testQuestion.Question), // This would need to be implemented based on question type
                    IsCorrect = isCorrect,
                    Marks = testQuestion.Marks,
                    Explanation = testQuestion.Question.Explanation
                });
            }

            var accuracy = totalPossibleMarks > 0 ? (decimal)(correctAnswers * 100.0 / (double)testQuestions.Count()) : 0;

            return new TestScoreResult
            {
                TotalScore = totalScore,
                Accuracy = accuracy,
                QuestionResults = questionResults
            };
        }

        private bool CheckAnswer(Question question, string userAnswer)
        {
            // This is a simplified implementation
            // In a real system, you'd have different question types and answer validation logic
            if (string.IsNullOrEmpty(userAnswer))
                return false;

            // For multiple choice questions, you'd compare with the correct option
            // For now, this is a placeholder implementation
            return userAnswer.Trim().Length > 0;
        }

        private string GetCorrectAnswer(Question question)
        {
            // This would need to be implemented based on your question structure
            // For now, return a placeholder
            return "Correct answer would be here";
        }
    }

    internal class TestScoreResult
    {
        public int TotalScore { get; set; }
        public decimal Accuracy { get; set; }
        public List<QuestionResultDto> QuestionResults { get; set; } = new();
    }
}
