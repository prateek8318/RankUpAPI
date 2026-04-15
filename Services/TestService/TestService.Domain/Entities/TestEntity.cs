using System.ComponentModel.DataAnnotations;
using TestService.Domain.Enums;

namespace TestService.Domain.Entities
{
    public enum TestType
    {
        Practice = 1,
        Mock = 2,
        PreviousYear = 3,
        ChapterWise = 4
    }

    public enum DifficultyLevel
    {
        Easy = 1,
        Medium = 2,
        Hard = 3,
        Mixed = 4
    }

    
    public class Test : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        // Alias for Name to maintain compatibility
        public string Title { get { return Name; } set { Name = value; } }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int ExamId { get; set; } // FK to Master Service Exams table

        [Required]
        public int SubjectId { get; set; } // FK to Master Service Subjects table

        [Required]
        public TestType TestType { get; set; } = TestType.Practice;

        [Required]
        public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Mixed;

        [Required]
        public int TotalQuestions { get; set; } = 0;

        [Required]
        public int DurationMinutes { get; set; } = 60;

        // Alias for DurationMinutes to maintain compatibility
        public int DurationInMinutes { get { return DurationMinutes; } set { DurationMinutes = value; } }

        [Required]
        public decimal TotalMarks { get; set; } = 0;

        [Required]
        public decimal PassingMarks { get; set; } = 0;

        [Required]
        public bool NegativeMarking { get; set; } = true;

        [Required]
        public decimal NegativeMarksPerQuestion { get; set; } = 0.25m;

        [MaxLength(2000)]
        public string? Instructions { get; set; }

        [MaxLength(2000)]
        public string? InstructionsEnglish { get; set; }

        [MaxLength(2000)]
        public string? InstructionsHindi { get; set; }

        public int DisplayOrder { get; set; } = 0;

        [Required]
        public bool IsLocked { get; set; } = false;

        [Required]
        public bool IsFree { get; set; } = false;

        [Required]
        public decimal Price { get; set; } = 0;

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "INR";

        [Required]
        public bool IsPublished { get; set; } = false;

        public DateTime? PublishDate { get; set; }

        [Required]
        public int CreatedBy { get; set; } // Admin user ID

        public int? PracticeModeId { get; set; } // FK to PracticeMode table
        public int? SeriesId { get; set; } // FK to TestSeries table

        // Navigation properties
        public virtual ICollection<TestQuestion> TestQuestions { get; set; } = new List<TestQuestion>();
        public virtual ICollection<TestAttempt> TestAttempts { get; set; } = new List<TestAttempt>();
        public virtual ICollection<TestReview> Reviews { get; set; } = new List<TestReview>();
        public virtual ICollection<TestAnalytics> Analytics { get; set; } = new List<TestAnalytics>();
        
        // Additional navigation properties
        public virtual Exam? Exam { get; set; }
        public virtual PracticeMode? PracticeMode { get; set; }
        public virtual TestSeries? Series { get; set; }
        public virtual Subject? Subject { get; set; }
    }

    public class TestQuestion : BaseEntity
    {
        [Required]
        public int TestId { get; set; }

        [Required]
        public int QuestionId { get; set; } // FK to QuestionService Questions table

        [Required]
        public int QuestionNumber { get; set; }

        public int DisplayOrder { get; set; } = 0;

        [Required]
        public decimal Marks { get; set; } = 1.00m;

        [Required]
        public decimal NegativeMarks { get; set; } = 0.00m;

        [Required]
        public bool IsOptional { get; set; } = false;

        // Navigation properties
        public virtual Test Test { get; set; } = null!;
        public virtual Question Question { get; set; } = null!;
    }

    public class TestAttempt : BaseEntity
    {
        [Required]
        public int UserId { get; set; } // FK to User Service Users table

        [Required]
        public int TestId { get; set; }

        [Required]
        public int AttemptNumber { get; set; } = 1;

        [Required]
        public TestAttemptStatus Status { get; set; } = TestAttemptStatus.InProgress;

        [Required]
        public DateTime StartTime { get; set; } = DateTime.UtcNow;

        public DateTime? EndTime { get; set; }

        public int? DurationTaken { get; set; } // In seconds

        [Required]
        public int TotalQuestions { get; set; } = 0;

        [Required]
        public int QuestionsAttempted { get; set; } = 0;

        [Required]
        public int CorrectAnswers { get; set; } = 0;

        [Required]
        public int WrongAnswers { get; set; } = 0;

        [Required]
        public int Unanswered { get; set; } = 0;

        [Required]
        public decimal TotalMarks { get; set; } = 0;

        [Required]
        public decimal ObtainedMarks { get; set; } = 0;

        [Required]
        public decimal Percentage { get; set; } = 0;

        public int? Rank { get; set; }

        public int? TotalParticipants { get; set; }

        public bool? IsPassed { get; set; }

        [MaxLength(45)]
        public string? IpAddress { get; set; }

        [MaxLength(500)]
        public string? DeviceInfo { get; set; } // Browser/device info

        // Navigation properties
        public virtual Test Test { get; set; } = null!;
        public virtual ICollection<AttemptAnswer> AttemptAnswers { get; set; } = new List<AttemptAnswer>();
        public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
    }

    public class AttemptAnswer : BaseEntity
    {
        [Required]
        public int TestAttemptId { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [MaxLength(10)]
        public string? SelectedAnswer { get; set; } // A, B, C, D, True, False

        // Alias for SelectedAnswer to maintain compatibility
        public string? Answer { get { return SelectedAnswer; } set { SelectedAnswer = value; } }

        public bool? IsCorrect { get; set; }

        // Alias for !IsSkipped to maintain compatibility
        public bool IsAnswered { get { return !IsSkipped; } }

        [Required]
        public decimal MarksObtained { get; set; } = 0;

        [Required]
        public decimal NegativeMarks { get; set; } = 0;

        public int? TimeTaken { get; set; } // In seconds to answer this question

        [Required]
        public bool IsSkipped { get; set; } = false;

        [Required]
        public bool IsMarkedForReview { get; set; } = false;

        public DateTime? AnsweredAt { get; set; }

        // Navigation properties
        public virtual TestAttempt TestAttempt { get; set; } = null!;
    }

    public class TestResult : BaseEntity
    {
        [Required]
        public int TestAttemptId { get; set; }

        [MaxLength(100)]
        public string? Section { get; set; } // For section-wise analysis

        [Required]
        public int TotalQuestions { get; set; } = 0;

        [Required]
        public int CorrectAnswers { get; set; } = 0;

        [Required]
        public int WrongAnswers { get; set; } = 0;

        [Required]
        public int Unanswered { get; set; } = 0;

        [Required]
        public decimal TotalMarks { get; set; } = 0;

        [Required]
        public decimal ObtainedMarks { get; set; } = 0;

        [Required]
        public decimal Percentage { get; set; } = 0;

        [Required]
        public decimal Accuracy { get; set; } = 0;

        [Required]
        public int TimeTaken { get; set; } = 0; // In seconds

        [Required]
        public decimal AverageTimePerQuestion { get; set; } = 0;

        [MaxLength(20)]
        public string? DifficultyLevel { get; set; }

        public int? TopicId { get; set; } // For topic-wise analysis

        // Navigation properties
        public virtual TestAttempt TestAttempt { get; set; } = null!;
    }

    public class TestAnalytics : BaseEntity
    {
        [Required]
        public int TestId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int TotalAttempts { get; set; } = 0;

        [Required]
        public int CompletedAttempts { get; set; } = 0;

        [Required]
        public decimal AverageScore { get; set; } = 0;

        [Required]
        public int AverageTime { get; set; } = 0; // In seconds

        [Required]
        public decimal PassPercentage { get; set; } = 0;

        [Required]
        public int UniqueUsers { get; set; } = 0;

        public string? DifficultyWiseStats { get; set; } // JSON data for difficulty-wise stats

        public string? TopicWiseStats { get; set; } // JSON data for topic-wise stats

        // Navigation properties
        public virtual Test Test { get; set; } = null!;
    }

    public class TestReview : BaseEntity
    {
        [Required]
        public int TestId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Review { get; set; }

        public string? Pros { get; set; } // JSON array of pros

        public string? Cons { get; set; } // JSON array of cons

        [Required]
        public bool IsVerified { get; set; } = false;

        [Required]
        public int HelpfulCount { get; set; } = 0;

        // Navigation properties
        public virtual Test Test { get; set; } = null!;
    }

    // Additional entity classes for navigation properties
    public class Exam
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
