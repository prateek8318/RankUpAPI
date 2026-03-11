using System.ComponentModel.DataAnnotations;

namespace MasterService.Application.DTOs
{
    // Exam DTOs with inheritance from base classes
    public class ExamDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CountryCode { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsInternational { get; set; }
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
        [Required(ErrorMessage = "Language ID is required")]
        public int LanguageId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
    }

    public class CreateExamDto : BaseCreateDto
    {
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [StringLength(10, ErrorMessage = "Country code cannot exceed 10 characters")]
        public string? CountryCode { get; set; }

        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }

        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        public string? ImageUrl { get; set; }

        public bool IsInternational { get; set; } = false;

        public List<ExamLanguageCreateDto>? Names { get; set; }

        /// <summary>
        /// Linked qualifications for this exam (multi-select).
        /// </summary>
        [Required(ErrorMessage = "At least one qualification is required")]
        public List<int>? QualificationIds { get; set; }

        /// <summary>
        /// Optional streams, index-aligned with QualificationIds.
        /// </summary>
        public List<int?>? StreamIds { get; set; }
    }

    public class UpdateExamDto : BaseUpdateDto
    {
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [StringLength(10, ErrorMessage = "Country code cannot exceed 10 characters")]
        public string? CountryCode { get; set; }

        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }

        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        public string? ImageUrl { get; set; }

        public bool IsInternational { get; set; }

        public List<ExamLanguageCreateDto>? Names { get; set; }

        [Required(ErrorMessage = "At least one qualification is required")]
        public List<int>? QualificationIds { get; set; }

        public List<int?>? StreamIds { get; set; }
    }

    // Qualification DTOs with inheritance from base classes
    public class QualificationDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string? NameHi { get; set; }
        public string? Description { get; set; }
        public string? CountryCode { get; set; }
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
        [Required(ErrorMessage = "Language ID is required")]
        public int LanguageId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
    }

    public class CreateQualificationDto : BaseCreateDto
    {
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [StringLength(10, ErrorMessage = "Country code cannot exceed 10 characters")]
        public string? CountryCode { get; set; }

        public List<QualificationLanguageCreateDto>? Names { get; set; }
    }

    public class UpdateQualificationDto : BaseUpdateDto
    {
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [StringLength(10, ErrorMessage = "Country code cannot exceed 10 characters")]
        public string? CountryCode { get; set; }

        public List<QualificationLanguageCreateDto>? Names { get; set; }
    }

    // Stream DTOs with inheritance from base classes
    public class StreamDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int QualificationId { get; set; }
        public string? QualificationName { get; set; }
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
        [Required(ErrorMessage = "Language ID is required")]
        public int LanguageId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
    }

    public class CreateStreamDto : BaseCreateDto
    {
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Qualification ID is required")]
        public int QualificationId { get; set; }

        public List<StreamLanguageCreateDto>? Names { get; set; }
    }

    public class UpdateStreamDto : BaseUpdateDto
    {
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Qualification ID is required")]
        public int QualificationId { get; set; }

        public List<StreamLanguageCreateDto>? Names { get; set; }
    }
}
