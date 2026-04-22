using System.ComponentModel.DataAnnotations;

namespace QuestionService.Application.DTOs
{
    // Mock Test Types Enum
    public enum MockTestType
    {
        MockTest = 1,        // Subject wise questions
        TestSeries = 2,      // Full length papers
        DeepPractice = 3,    // Topic wise MCQs
        PreviousYear = 4     // Previous years solved papers
    }

    // Mock Test Entity DTOs
    public class MockTestDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public MockTestType MockTestType { get; set; }
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public string ExamType { get; set; } = string.Empty;
        public int? SubjectId { get; set; } // Null for full-length papers
        public string? SubjectName { get; set; }
        public int? TopicId { get; set; } // Only for Deep Practice type
        public string? TopicName { get; set; }
        public int DurationInMinutes { get; set; }
        public int TotalQuestions { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal PassingMarks { get; set; }
        public bool HasNegativeMarking { get; set; }
        public decimal? NegativeMarkingValue { get; set; }
        public int? SubscriptionPlanId { get; set; }
        public string AccessType { get; set; } = "Free"; // Free, Paid
        public int AttemptsAllowed { get; set; } = 1;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        
        // Additional properties for different types
        public int? Year { get; set; } // For Previous Year papers
        public string? Difficulty { get; set; } // For Deep Practice
        public string? PaperCode { get; set; } // For Test Series
        
        // Navigation properties
        public List<MockTestQuestionDto> Questions { get; set; } = new();
        public SubscriptionPlanDto? SubscriptionPlan { get; set; }
    }

    public class MockTestQuestionDto
    {
        public int Id { get; set; }
        public int MockTestId { get; set; }
        public int QuestionId { get; set; }
        public int QuestionNumber { get; set; }
        public decimal Marks { get; set; }
        public decimal NegativeMarks { get; set; }
        
        // Navigation properties
        public QuestionDto? Question { get; set; }
    }

    public class CreateMockTestDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [Required]
        public MockTestType MockTestType { get; set; }
        
        [Required]
        public int ExamId { get; set; }
        
        public int? SubjectId { get; set; } // Required for MockTest, optional for others
        public int? TopicId { get; set; } // Required for DeepPractice
        
        [Required]
        public int DurationInMinutes { get; set; } = 30;
        
        [Required]
        public int TotalQuestions { get; set; } = 20;
        
        [Required]
        public decimal TotalMarks { get; set; } = 100;
        
        [Required]
        public decimal PassingMarks { get; set; } = 35;
        
        public int? SubscriptionPlanId { get; set; }
        public string AccessType { get; set; } = "Free";
        public int AttemptsAllowed { get; set; } = 1;
        
        // Type-specific properties
        public int? Year { get; set; } // For PreviousYear papers
        public string? Difficulty { get; set; } // For DeepPractice (Easy, Medium, Hard)
        public string? PaperCode { get; set; } // For TestSeries
        
        [Required]
        public int CreatedBy { get; set; }
        
        // Optional: Pre-select questions for this mock test
        public List<int>? QuestionIds { get; set; }
    }

    public class UpdateMockTestDto
    {
        [Required]
        public int Id { get; set; }
        
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? DurationInMinutes { get; set; }
        public int? TotalQuestions { get; set; }
        public decimal? TotalMarks { get; set; }
        public decimal? PassingMarks { get; set; }
        public int? SubscriptionPlanId { get; set; }
        public string? AccessType { get; set; }
        public int? AttemptsAllowed { get; set; }
        public bool? IsActive { get; set; }
    }

    public class MockTestListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public MockTestType MockTestType { get; set; }
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public string ExamType { get; set; } = string.Empty;
        public int? SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public int? TopicId { get; set; }
        public string? TopicName { get; set; }
        public int DurationInMinutes { get; set; }
        public int TotalQuestions { get; set; }
        public decimal TotalMarks { get; set; }
        public bool HasNegativeMarking { get; set; }
        public string AccessType { get; set; } = "Free";
        public int? SubscriptionPlanId { get; set; }
        public string? SubscriptionPlanName { get; set; }
        public bool IsUnlocked { get; set; } // For user perspective
        public int AttemptsUsed { get; set; }
        public int AttemptsAllowed { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Type-specific display properties
        public int? Year { get; set; } // For PreviousYear papers
        public string? Difficulty { get; set; } // For DeepPractice
        public string? PaperCode { get; set; } // For TestSeries
        public string MockTestTypeDisplay { get; set; } = string.Empty; // Friendly name
    }

    public class MockTestDetailDto : MockTestListDto
    {
        public string SubjectName { get; set; } = string.Empty;
        public decimal PassingMarks { get; set; }
        public decimal? NegativeMarkingValue { get; set; }
        public bool IsActive { get; set; }
        public List<QuestionDto> Questions { get; set; } = new();
        public List<MockTestAttemptDto> Attempts { get; set; } = new();
    }

    public class MockTestAttemptDto
    {
        public int Id { get; set; }
        public int MockTestId { get; set; }
        public int UserId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public TimeSpan? Duration { get; set; }
        public int TotalQuestions { get; set; }
        public int AnsweredQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public decimal ObtainedMarks { get; set; }
        public decimal Percentage { get; set; }
        public string Status { get; set; } = "NotStarted"; // NotStarted, InProgress, Completed
        public string Grade { get; set; } = string.Empty;
    }

    public class StartMockTestDto
    {
        [Required]
        public int MockTestId { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        public string LanguageCode { get; set; } = "en";
    }

    public class MockTestSessionDto
    {
        public int SessionId { get; set; }
        public int MockTestId { get; set; }
        public string MockTestName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int DurationInMinutes { get; set; }
        public int TotalQuestions { get; set; }
        public int AnsweredQuestions { get; set; }
        public int MarkedForReview { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal ObtainedMarks { get; set; }
        public bool HasNegativeMarking { get; set; }
        public decimal NegativeMarksPerQuestion { get; set; }
        public string Status { get; set; } = "InProgress";
        public List<QuizQuestionDto> Questions { get; set; } = new();
    }

    // Subscription Plan DTO for integration
    public class SubscriptionPlanDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationInDays { get; set; }
        public List<string> Features { get; set; } = new();
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // User Subscription DTO
    public class UserSubscriptionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SubscriptionPlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;
    }

    // Response DTOs
    public class MockTestListResponseDto
    {
        public IReadOnlyList<MockTestListDto> MockTests { get; set; } = new List<MockTestListDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public UserSubscriptionDto? UserSubscription { get; set; }
    }

    public class MockTestAccessResponseDto
    {
        public bool CanAccess { get; set; }
        public string Reason { get; set; } = string.Empty;
        public UserSubscriptionDto? UserSubscription { get; set; }
        public MockTestDto? MockTest { get; set; }
    }
}
