namespace MasterService.Application.DTOs
{
    public class ExamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CountryCode { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsInternational { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ExamLanguageDto> Names { get; set; } = new();
        public List<int> QualificationIds { get; set; } = new();
        public List<int?> StreamIds { get; set; } = new();
    }

    public class ExamLanguageDto
    {
        public int LanguageId { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
        public string LanguageName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class ExamLanguageCreateDto
    {
        public int LanguageId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class CreateExamDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CountryCode { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsInternational { get; set; } = false;

        public List<ExamLanguageCreateDto>? Names { get; set; }

        /// <summary>
        /// Linked qualifications for this exam (multi-select).
        /// </summary>
        public List<int>? QualificationIds { get; set; }

        /// <summary>
        /// Optional streams, index-aligned with QualificationIds.
        /// </summary>
        public List<int?>? StreamIds { get; set; }
    }

    public class UpdateExamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CountryCode { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsInternational { get; set; }

        public List<ExamLanguageCreateDto>? Names { get; set; }
        public List<int>? QualificationIds { get; set; }
        public List<int?>? StreamIds { get; set; }
    }

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
