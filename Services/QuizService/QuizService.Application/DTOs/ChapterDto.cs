namespace QuizService.Application.DTOs
{
    public class ChapterDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SubjectId { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateChapterDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SubjectId { get; set; }
    }

    public class UpdateChapterDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
