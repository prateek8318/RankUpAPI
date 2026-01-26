namespace TestService.Application.DTOs
{
    public class TestDto
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public int PracticeModeId { get; set; }
        public string PracticeModeName { get; set; } = string.Empty;
        public int? SeriesId { get; set; }
        public string? SeriesName { get; set; }
        public int? SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public int? Year { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationInMinutes { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalMarks { get; set; }
        public int PassingMarks { get; set; }
        public string? InstructionsEnglish { get; set; }
        public string? InstructionsHindi { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsLocked { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateTestDto
    {
        public int ExamId { get; set; }
        public int PracticeModeId { get; set; }
        public int? SeriesId { get; set; }
        public int? SubjectId { get; set; }
        public int? Year { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationInMinutes { get; set; } = 60;
        public int TotalQuestions { get; set; } = 0;
        public int TotalMarks { get; set; } = 100;
        public int PassingMarks { get; set; } = 35;
        public string? InstructionsEnglish { get; set; }
        public string? InstructionsHindi { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public bool IsLocked { get; set; } = false;
    }

    public class UpdateTestDto
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int PracticeModeId { get; set; }
        public int? SeriesId { get; set; }
        public int? SubjectId { get; set; }
        public int? Year { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationInMinutes { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalMarks { get; set; }
        public int PassingMarks { get; set; }
        public string? InstructionsEnglish { get; set; }
        public string? InstructionsHindi { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsLocked { get; set; }
    }

    public class TestFilterDto
    {
        public int ExamId { get; set; }
        public int PracticeModeId { get; set; }
        public int? SeriesId { get; set; }
        public int? SubjectId { get; set; }
        public int? Year { get; set; }
    }
}
