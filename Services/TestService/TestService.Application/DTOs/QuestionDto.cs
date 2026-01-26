using TestService.Domain.Enums;

namespace TestService.Application.DTOs
{
    public class QuestionDto
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }
        public string? Explanation { get; set; }
        public QuestionDifficulty Difficulty { get; set; }
        public int Marks { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateQuestionDto
    {
        public string QuestionText { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }
        public string? Explanation { get; set; }
        public QuestionDifficulty Difficulty { get; set; } = QuestionDifficulty.Medium;
        public int Marks { get; set; } = 1;
        public int DisplayOrder { get; set; } = 0;
    }

    public class UpdateQuestionDto
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }
        public string? Explanation { get; set; }
        public QuestionDifficulty Difficulty { get; set; }
        public int Marks { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class TestQuestionDto
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public int QuestionId { get; set; }
        public int DisplayOrder { get; set; }
        public int Marks { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public QuestionDto? Question { get; set; }
    }

    public class CreateTestQuestionDto
    {
        public int TestId { get; set; }
        public int QuestionId { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public int Marks { get; set; } = 1;
    }

    public class UpdateTestQuestionDto
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public int QuestionId { get; set; }
        public int DisplayOrder { get; set; }
        public int Marks { get; set; }
    }

    public class UserTestAttemptDto
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public int UserId { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int CurrentQuestionIndex { get; set; }
        public int Score { get; set; }
        public int TotalMarks { get; set; }
        public decimal Accuracy { get; set; }
        public TestAttemptStatus Status { get; set; }
        public string? AnswersJson { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public TestDto? Test { get; set; }
    }

    public class CreateUserTestAttemptDto
    {
        public int TestId { get; set; }
        public int UserId { get; set; }
        public DateTime? StartedAt { get; set; }
        public int CurrentQuestionIndex { get; set; } = 0;
        public TestAttemptStatus Status { get; set; } = TestAttemptStatus.NotStarted;
        public string? AnswersJson { get; set; }
    }

    public class UpdateUserTestAttemptDto
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public int UserId { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int CurrentQuestionIndex { get; set; }
        public int Score { get; set; }
        public int TotalMarks { get; set; }
        public decimal Accuracy { get; set; }
        public TestAttemptStatus Status { get; set; }
        public string? AnswersJson { get; set; }
    }
}
