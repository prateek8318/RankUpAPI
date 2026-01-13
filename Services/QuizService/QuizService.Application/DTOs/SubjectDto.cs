namespace QuizService.Application.DTOs
{
    public class SubjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ExamId { get; set; }
        public bool IsActive { get; set; }
        public List<int> ChapterIds { get; set; } = new();
    }

    public class CreateSubjectDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ExamId { get; set; }
    }

    public class UpdateSubjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
