namespace MasterService.Application.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        // Localized name based on requested language (Hi select karne par Hindi, warna English)
        public string Name { get; set; } = string.Empty;

        // Stored English name from DB
        public string NameEn { get; set; } = string.Empty;

        // Stored Hindi name from DB (optional)
        public string? NameHi { get; set; }

        public string Key { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // category, qualification, exam_category, stream
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateCategoryDto
    {
        // English name (required)
        public string NameEn { get; set; } = string.Empty;

        // Hindi name (optional)
        public string? NameHi { get; set; }

        public string Key { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

    public class UpdateCategoryDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string? NameHi { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
