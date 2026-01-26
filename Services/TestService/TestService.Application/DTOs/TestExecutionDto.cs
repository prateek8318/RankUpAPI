using TestService.Domain.Enums;

namespace TestService.Application.DTOs
{
    public class TestResultDto
    {
        public int AttemptId { get; set; }
        public int TestId { get; set; }
        public string TestTitle { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public int TotalMarks { get; set; }
        public int Score { get; set; }
        public decimal Accuracy { get; set; }
        public bool Passed { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int DurationMinutes { get; set; }
        public List<QuestionResultDto> QuestionResults { get; set; } = new();
    }

    public class QuestionResultDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string UserAnswer { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int Marks { get; set; }
        public string? Explanation { get; set; }
    }

    public class TestQuestionListDto
    {
        public int AttemptId { get; set; }
        public int TestId { get; set; }
        public string TestTitle { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime? StartedAt { get; set; }
        public int CurrentQuestionIndex { get; set; }
        public List<TestQuestionWithAnswerDto> Questions { get; set; } = new();
    }

    public class TestQuestionWithAnswerDto
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }
        public string? Explanation { get; set; }
        public QuestionDifficulty Difficulty { get; set; }
        public int Marks { get; set; }
        public int DisplayOrder { get; set; }
        public string UserAnswer { get; set; } = string.Empty;
    }
}
