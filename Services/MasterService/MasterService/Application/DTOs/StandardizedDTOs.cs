using System.ComponentModel.DataAnnotations;

namespace MasterService.Application.DTOs
{
    // Base DTO for common properties
    public abstract class BaseDto
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    // Base create DTO with validation
    public abstract class BaseCreateDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;
    }

    // Base update DTO with validation
    public abstract class BaseUpdateDto : BaseCreateDto
    {
        public int Id { get; set; }
    }

    // Standardized response wrapper
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Language { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Error { get; set; }
        public int? TotalCount { get; set; }
    }

    // =============================================
    // CATEGORY DTOS
    // =============================================

    public class CategoryDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string? NameHi { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class CreateCategoryDto : BaseCreateDto
    {
        public string? NameHi { get; set; }
        [Required(ErrorMessage = "Key is required")]
        [StringLength(50, ErrorMessage = "Key cannot exceed 50 characters")]
        public string Key { get; set; } = string.Empty;
        [Required(ErrorMessage = "Type is required")]
        [StringLength(50, ErrorMessage = "Type cannot exceed 50 characters")]
        public string Type { get; set; } = string.Empty;
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
        public int DisplayOrder { get; set; } = 0;
    }

    public class UpdateCategoryDto : BaseUpdateDto
    {
        public string? NameHi { get; set; }
        [Required(ErrorMessage = "Key is required")]
        [StringLength(50, ErrorMessage = "Key cannot exceed 50 characters")]
        public string Key { get; set; } = string.Empty;
        [Required(ErrorMessage = "Type is required")]
        [StringLength(50, ErrorMessage = "Type cannot exceed 50 characters")]
        public string Type { get; set; } = string.Empty;
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
        public int DisplayOrder { get; set; } = 0;
    }

    // =============================================
    // STATE DTOS
    // =============================================

    public class StateDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public List<StateLanguageDto>? Names { get; set; }
    }

    public class StateLanguageDto
    {
        public int LanguageId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LanguageCode { get; set; } = string.Empty;
    }

    public class CreateStateDto : BaseCreateDto
    {
        [Required(ErrorMessage = "Code is required")]
        [StringLength(10, ErrorMessage = "Code cannot exceed 10 characters")]
        public string Code { get; set; } = string.Empty;
        [Required(ErrorMessage = "Country code is required")]
        [StringLength(10, ErrorMessage = "Country code cannot exceed 10 characters")]
        public string CountryCode { get; set; } = string.Empty;
        public List<StateLanguageCreateDto>? Names { get; set; }
    }

    public class UpdateStateDto : BaseUpdateDto
    {
        [Required(ErrorMessage = "Code is required")]
        [StringLength(10, ErrorMessage = "Code cannot exceed 10 characters")]
        public string Code { get; set; } = string.Empty;
        [Required(ErrorMessage = "Country code is required")]
        [StringLength(10, ErrorMessage = "Country code cannot exceed 10 characters")]
        public string CountryCode { get; set; } = string.Empty;
        public List<StateLanguageCreateDto>? Names { get; set; }
    }

    public class StateLanguageCreateDto
    {
        [Required(ErrorMessage = "Language ID is required")]
        public int LanguageId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;
    }

    // =============================================
    // QUALIFICATION DTOS
    // =============================================

    public class QualificationDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CountryCode { get; set; }
        public List<QualificationLanguageDto>? Names { get; set; }
    }

    public class QualificationLanguageDto
    {
        public int LanguageId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
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

    // =============================================
    // STREAM DTOS
    // =============================================

    public class StreamDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int QualificationId { get; set; }
        public string QualificationName { get; set; } = string.Empty;
        public List<StreamLanguageDto>? Names { get; set; }
    }

    public class StreamLanguageDto
    {
        public int LanguageId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
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

    // =============================================
    // EXAM DTOS
    // =============================================

    public class ExamDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CountryCode { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsInternational { get; set; }
        public List<int> QualificationIds { get; set; } = new();
        public List<int?> StreamIds { get; set; } = new();
        public List<ExamLanguageDto>? Names { get; set; }
    }

    public class ExamLanguageDto
    {
        public int LanguageId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
        public string LanguageName { get; set; } = string.Empty;
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
        [Required(ErrorMessage = "At least one qualification is required")]
        public List<int> QualificationIds { get; set; } = new();
        public List<int?> StreamIds { get; set; } = new();
        public List<ExamLanguageCreateDto>? Names { get; set; }
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
        public bool IsInternational { get; set; } = false;
        [Required(ErrorMessage = "At least one qualification is required")]
        public List<int> QualificationIds { get; set; } = new();
        public List<int?> StreamIds { get; set; } = new();
        public List<ExamLanguageCreateDto>? Names { get; set; }
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

    // =============================================
    // FILTER DTOS
    // =============================================

    public class BaseFilterDto
    {
        public string? Language { get; set; }
        public string? CountryCode { get; set; }
        public bool? IsActive { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class ExamFilterDto : BaseFilterDto
    {
        public int? QualificationId { get; set; }
        public int? StreamId { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public bool? IsInternational { get; set; }
    }

    public class StreamFilterDto : BaseFilterDto
    {
        public int? QualificationId { get; set; }
    }
}
