namespace TestService.Application.DTOs
{
    public class PracticeModeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
        public string? ImageUrl { get; set; }
        public string? LinkUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ExamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
        public string? ImageUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<SubjectDto> Subjects { get; set; } = new();
    }

    public class SubjectDto
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    // Home Dashboard DTOs
    public class SelectedExamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
    }

    public class ResumeTestDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string PracticeModeName { get; set; } = string.Empty;
        public int ProgressPercentage { get; set; }
        public DateTime StartedAt { get; set; }
        public int TimeRemainingSeconds { get; set; }
        public string? SeriesName { get; set; }
        public string? SubjectName { get; set; }
    }

    public class DailyTargetDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int TargetMinutes { get; set; }
        public int CompletedMinutes { get; set; }
        public int ProgressPercentage { get; set; }
        public bool IsCompleted { get; set; }
    }
}
