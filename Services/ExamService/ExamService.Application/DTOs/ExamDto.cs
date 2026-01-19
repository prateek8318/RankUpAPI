namespace ExamService.Application.DTOs
{
    public class ExamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationInMinutes { get; set; }
        public int TotalMarks { get; set; }
        public int PassingMarks { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ImageUrl { get; set; }
        public List<int> QualificationIds { get; set; } = new();
    }

    public class CreateExamDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationInMinutes { get; set; } = 60;
        public int TotalMarks { get; set; } = 100;
        public int PassingMarks { get; set; } = 35;
        public string? ImageUrl { get; set; }
        public List<int>? QualificationIds { get; set; }
    }

    public class UpdateExamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationInMinutes { get; set; }
        public int TotalMarks { get; set; }
        public int PassingMarks { get; set; }
        public string? ImageUrl { get; set; }
        public List<int>? QualificationIds { get; set; }
    }
}
