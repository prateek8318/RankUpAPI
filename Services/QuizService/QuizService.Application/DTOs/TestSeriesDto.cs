namespace QuizService.Application.DTOs
{
    public class TestSeriesDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ExamId { get; set; }
        public int DurationInMinutes { get; set; }
        public int TotalMarks { get; set; }
        public int TotalQuestions { get; set; }
        public int PassingMarks { get; set; }
        public string? InstructionsEnglish { get; set; }
        public string? InstructionsHindi { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsLocked { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateTestSeriesDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ExamId { get; set; }
        public int DurationInMinutes { get; set; } = 60;
        public int TotalMarks { get; set; } = 100;
        public int PassingMarks { get; set; } = 35;
        public string? InstructionsEnglish { get; set; }
        public string? InstructionsHindi { get; set; }
        public int DisplayOrder { get; set; } = 0;
    }

    public class UpdateTestSeriesDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationInMinutes { get; set; }
        public int TotalMarks { get; set; }
        public int PassingMarks { get; set; }
        public string? InstructionsEnglish { get; set; }
        public string? InstructionsHindi { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsLocked { get; set; }
    }
}
