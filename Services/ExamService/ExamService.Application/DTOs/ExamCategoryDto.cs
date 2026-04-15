namespace ExamService.Application.DTOs
{
    public class ExamCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class ExamTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ExamCategoryId { get; set; }
        public bool IsActive { get; set; }
    }
}
