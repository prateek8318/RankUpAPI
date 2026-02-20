namespace MasterService.Application.DTOs
{
    public class QualificationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CountryCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<QualificationLanguageDto> Names { get; set; } = new();
    }

    public class QualificationLanguageDto
    {
        public int LanguageId { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
        public string LanguageName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class QualificationLanguageCreateDto
    {
        public int LanguageId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class CreateQualificationDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CountryCode { get; set; }
        public List<QualificationLanguageCreateDto>? Names { get; set; }
    }

    public class UpdateQualificationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CountryCode { get; set; }
        public List<QualificationLanguageCreateDto>? Names { get; set; }
    }

    public class StreamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int QualificationId { get; set; }
        public string? QualificationName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<StreamLanguageDto> Names { get; set; } = new();
    }

    public class StreamLanguageDto
    {
        public int LanguageId { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
        public string LanguageName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class StreamLanguageCreateDto
    {
        public int LanguageId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class CreateStreamDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int QualificationId { get; set; }
        public List<StreamLanguageCreateDto>? Names { get; set; }
    }

    public class UpdateStreamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int QualificationId { get; set; }
        public List<StreamLanguageCreateDto>? Names { get; set; }
    }
}
