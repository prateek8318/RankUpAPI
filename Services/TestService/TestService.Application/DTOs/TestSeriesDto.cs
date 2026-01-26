namespace TestService.Application.DTOs
{
    public class TestSeriesDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public int DurationInMinutes { get; set; }
        public int TotalMarks { get; set; }
        public int TotalQuestions { get; set; }
        public int PassingMarks { get; set; }
        public string? InstructionsEnglish { get; set; }
        public string? InstructionsHindi { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsLocked { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int TestCount { get; set; } = 0;
    }

    public class CreateTestSeriesDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ExamId { get; set; }
        public int DurationInMinutes { get; set; } = 60;
        public int TotalMarks { get; set; } = 100;
        public int TotalQuestions { get; set; } = 0;
        public int PassingMarks { get; set; } = 35;
        public string? InstructionsEnglish { get; set; }
        public string? InstructionsHindi { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public bool IsLocked { get; set; } = false;
    }

    public class UpdateTestSeriesDto
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
    }

    // Bulk upload DTOs
    public class TestBulkUploadDto
    {
        public int SeriesId { get; set; }
        public string TestTitle { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationInMinutes { get; set; } = 60;
        public int TotalQuestions { get; set; } = 0;
        public int TotalMarks { get; set; } = 100;
        public int PassingMarks { get; set; } = 35;
        public string? InstructionsEnglish { get; set; }
        public string? InstructionsHindi { get; set; }
        public int DisplayOrder { get; set; } = 0;
    }

    public class TestBulkUploadResultDto
    {
        public int SeriesId { get; set; }
        public int TotalRows { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<TestBulkUploadErrorDto> Errors { get; set; } = new();
        public string Status { get; set; } = string.Empty;
    }

    public class TestBulkUploadErrorDto
    {
        public int RowNumber { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public string? RowData { get; set; }
    }
}
