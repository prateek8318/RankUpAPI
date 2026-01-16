namespace MasterService.Application.DTOs
{
    public class LanguageDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateLanguageDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
    }

    public class UpdateLanguageDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
    }
}
