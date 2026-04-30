using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
using QuestionService.Application.Serialization;
using QuestionService.Domain.Entities;

namespace QuestionService.Application.DTOs
{
    public class QuestionDto
    {
        public int Id { get; set; }
        public int? ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public int ExamId { get; set; }
        public string? ExamName { get; set; }
        public int SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public int? TopicId { get; set; }
        public string? TopicName { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        public string CorrectAnswer { get; set; } = string.Empty;
        public string? Explanation { get; set; }
        public decimal Marks { get; set; }
        public decimal NegativeMarks { get; set; }
        public string DifficultyLevel { get; set; } = "Medium";
        public string QuestionType { get; set; } = "MCQ";
        public string? QuestionImageUrl { get; set; }
        public string? OptionAImageUrl { get; set; }
        public string? OptionBImageUrl { get; set; }
        public string? OptionCImageUrl { get; set; }
        public string? OptionDImageUrl { get; set; }
        public string? ExplanationImageUrl { get; set; }
        public bool SameExplanationForAllLanguages { get; set; }
        public string? Reference { get; set; }
        public string? Tags { get; set; }
        public int CreatedBy { get; set; }
        public int? ReviewedBy { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? PublishDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public int? MockTestId { get; set; }
        public string? MockTestName { get; set; }
        
        // Translation fields for multi-language support
        public string? TranslatedOptionA { get; set; }
        public string? TranslatedOptionB { get; set; }
        public string? TranslatedOptionC { get; set; }
        public string? TranslatedOptionD { get; set; }
        public string? TranslatedExplanation { get; set; }
        
        // Navigation properties
        public TopicDto? Topic { get; set; }
        public List<QuestionTranslationDto> Translations { get; set; } = new();
    }

    public class QuestionTranslationDto
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
        public string QuestionText { get; set; } = string.Empty;
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        public string? Explanation { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class TopicDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public string? Description { get; set; }
        public int? ParentTopicId { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public TopicDto? ParentTopic { get; set; }
        public List<TopicDto> ChildTopics { get; set; } = new();
    }

    public class CreateQuestionDto
    {
        [Required]
        public int? ModuleId { get; set; }
        
        [Required]
        public int ExamId { get; set; }
        
        [Required]
        public int SubjectId { get; set; }
        
        public int? TopicId { get; set; }
        
        [Required]
        public string QuestionText { get; set; } = string.Empty;
        
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        
        [Required]
        [StringLength(1)]
        public string CorrectAnswer { get; set; } = string.Empty;
        
        public string? Explanation { get; set; }
        
        [Required]
        [Range(0.01, 100.00)]
        public decimal Marks { get; set; } = 1.00m;
        
        [Range(0.00, 100.00)]
        public decimal NegativeMarks { get; set; } = 0.00m;
        
        [Required]
        public string DifficultyLevel { get; set; } = "Medium";
        
        [Required]
        public string QuestionType { get; set; } = "MCQ";
        
        public string? QuestionImageUrl { get; set; }
        public string? OptionAImageUrl { get; set; }
        public string? OptionBImageUrl { get; set; }
        public string? OptionCImageUrl { get; set; }
        public string? OptionDImageUrl { get; set; }
        public string? ExplanationImageUrl { get; set; }
        
        public bool SameExplanationForAllLanguages { get; set; } = false;
        
        public string? Reference { get; set; }
        public string? Tags { get; set; }
        
        [Required]
        public int CreatedBy { get; set; }
        
        // Translation support
        public List<QuestionTranslationCreateDto>? Translations { get; set; }
    }

    public class QuestionTranslationCreateDto
    {
        [Required]
        public string LanguageCode { get; set; } = string.Empty;
        
        [Required]
        public string QuestionText { get; set; } = string.Empty;
        
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        
        public string? Explanation { get; set; }
    }

    public class QuestionTranslationUpsertDto
    {
        [Required]
        public string LanguageCode { get; set; } = string.Empty;
        
        [Required]
        public string QuestionText { get; set; } = string.Empty;
        
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        
        public string? Explanation { get; set; }
        public string? QuestionImageUrl { get; set; }
        public string? OptionAImageUrl { get; set; }
        public string? OptionBImageUrl { get; set; }
        public string? OptionCImageUrl { get; set; }
        public string? OptionDImageUrl { get; set; }
    }

    public class UpdateQuestionDto
    {
        [Required]
        public int Id { get; set; }
        
        public int? ModuleId { get; set; }
        public int? TopicId { get; set; }
        public string? QuestionText { get; set; }
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        public string? CorrectAnswer { get; set; }
        public string? Explanation { get; set; }
        public decimal? Marks { get; set; }
        public decimal? NegativeMarks { get; set; }
        public string? DifficultyLevel { get; set; }
        public string? QuestionType { get; set; }
        public string? QuestionImageUrl { get; set; }
        public string? OptionAImageUrl { get; set; }
        public string? OptionBImageUrl { get; set; }
        public string? OptionCImageUrl { get; set; }
        public string? OptionDImageUrl { get; set; }
        public string? ExplanationImageUrl { get; set; }
        public bool? SameExplanationForAllLanguages { get; set; }
        public string? Reference { get; set; }
        public string? Tags { get; set; }
        
        // Translation updates
        public List<QuestionTranslationUpdateDto>? Translations { get; set; }
    }

    public class QuestionTranslationUpdateDto
    {
        [Required]
        public string LanguageCode { get; set; } = string.Empty;
        
        public string? QuestionText { get; set; }
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        public string? Explanation { get; set; }
    }

    public class QuestionListRequestDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int? ExamId { get; set; }
        public int? SubjectId { get; set; }
        public int? TopicId { get; set; }
        public string? DifficultyLevel { get; set; }
        public bool? IsPublished { get; set; }
        public string LanguageCode { get; set; } = "en";
    }

    public class QuestionListResponseDto
    {
        public IReadOnlyList<QuestionDto> Items { get; set; } = new List<QuestionDto>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    // Cursor Pagination DTOs
    public class QuestionCursorRequestDto
    {
        public int? ExamId { get; set; }
        public int? SubjectId { get; set; }
        public int? TopicId { get; set; }
        public string? DifficultyLevel { get; set; }
        public bool? IsPublished { get; set; }
        public string LanguageCode { get; set; } = "en";
        public int PageSize { get; set; } = 20;
        public string? Cursor { get; set; }
        public string? Direction { get; set; } = "next"; // "next" or "prev"
    }

    public class QuestionCursorResponseDto
    {
        public IReadOnlyList<QuestionDto> Data { get; set; } = new List<QuestionDto>();
        public string? NextCursor { get; set; }
        public string? PreviousCursor { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
    }

    public class QuestionStatisticsDto
    {
        public int TotalQuestions { get; set; }
        public int AddedToday { get; set; }
        public int NegativeMarksCount { get; set; }
        public int UnpublishedCount { get; set; }
        public int EasyCount { get; set; }
        public int MediumCount { get; set; }
        public int HardCount { get; set; }
        public List<SubjectQuestionCountDto> QuestionsBySubject { get; set; } = new();
        public List<ExamQuestionCountDto> QuestionsByExam { get; set; } = new();
    }

    public class SubjectQuestionCountDto
    {
        public string SubjectName { get; set; } = string.Empty;
        public int QuestionCount { get; set; }
    }

    public class ExamQuestionCountDto
    {
        public string ExamName { get; set; } = string.Empty;
        public int QuestionCount { get; set; }
    }

    public class CreateTopicDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public int SubjectId { get; set; }
        
        public string? Description { get; set; }
        
        public int? ParentTopicId { get; set; }
        
        [Range(0, int.MaxValue)]
        public int SortOrder { get; set; } = 0;
    }

    public class UpdateTopicDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? ParentTopicId { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsActive { get; set; }
    }

    // Batch Upload DTOs
    public class QuestionBatchDto
    {
        public int Id { get; set; }
        public string BatchName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public int ProcessedQuestions { get; set; }
        public int FailedQuestions { get; set; }
        public BatchStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
        public int UploadedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        
        // Navigation properties
        public List<QuestionErrorDto> Errors { get; set; } = new();
    }

    public class QuestionErrorDto
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public int RowNumber { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string? RawData { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateQuestionBatchDto
    {
        [Required]
        public string BatchName { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [Required]
        public string FileName { get; set; } = string.Empty;
        
        [Required]
        public string FilePath { get; set; } = string.Empty;
        
        [Required]
        public int UploadedBy { get; set; }
    }

    public class QuestionBatchListRequestDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public BatchStatus? Status { get; set; }
    }

    public class QuestionBatchListResponseDto
    {
        public IReadOnlyList<QuestionBatchDto> Items { get; set; } = new List<QuestionBatchDto>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class PublishQuestionDto
    {
        [Required]
        public int QuestionId { get; set; }
        
        [Required]
        public int ReviewedBy { get; set; }
    }

    public class ToggleQuestionPublishStatusDto
    {
        [Required]
        public int QuestionId { get; set; }
        
        [Required]
        public bool IsPublished { get; set; }
        
        [Required]
        public int ReviewedBy { get; set; }
    }

    // Admin-specific DTOs
    public class CreateQuestionRequestDto
    {
        public int ModuleId { get; set; }
        public int ExamId { get; set; }
        public int SubjectId { get; set; }
        public int TopicId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string OptionA { get; set; } = string.Empty;
        public string OptionB { get; set; } = string.Empty;
        public string OptionC { get; set; } = string.Empty;
        public string OptionD { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public decimal Marks { get; set; } = 1;
        public decimal NegativeMarks { get; set; } = 0;
        public string DifficultyLevel { get; set; } = "Medium";
        public string QuestionType { get; set; } = "MCQ";
        public string? QuestionImageUrl { get; set; }
        public string? OptionAImageUrl { get; set; }
        public string? OptionBImageUrl { get; set; }
        public string? OptionCImageUrl { get; set; }
        public string? OptionDImageUrl { get; set; }
        public string? ExplanationImageUrl { get; set; }
        public bool SameExplanationForAllLanguages { get; set; }
        public string? Reference { get; set; }
        public string? Tags { get; set; }
        public int CreatedBy { get; set; }
        public int? MockTestId { get; set; }
        public List<QuestionTranslationUpsertDto> Translations { get; set; } = new();
    }

    public class CreateQuestionFormDataDto
    {
        public int ModuleId { get; set; }
        public int MockTestId { get; set; }
        public int SubjectId { get; set; }
        public int? TopicId { get; set; }
        // NOTE: These are nullable to avoid ASP.NET Core implicit "required" validation
        // for non-nullable reference types when using [ApiController] + [FromForm].
        // We validate/normalize them in the service layer.
        public string? QuestionText { get; set; }
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        public string? CorrectAnswer { get; set; }
        public string? Explanation { get; set; }
        public decimal Marks { get; set; } = 1;
        public decimal NegativeMarks { get; set; } = 0;
        public string DifficultyLevel { get; set; } = "Medium";
        public string QuestionType { get; set; } = "MCQ";
        public bool SameExplanationForAllLanguages { get; set; }
        public string? Reference { get; set; }
        public string? Tags { get; set; }
        public int CreatedBy { get; set; }
        public string TranslationsJson { get; set; } = "[]";
        
        // Image files
        public IFormFile? QuestionImage { get; set; }
        public IFormFile? OptionAImage { get; set; }
        public IFormFile? OptionBImage { get; set; }
        public IFormFile? OptionCImage { get; set; }
        public IFormFile? OptionDImage { get; set; }
        public IFormFile? ExplanationImage { get; set; }
    }

    public class CreateQuestionAdminDto
    {
        public int ModuleId { get; set; }
        public int ExamId { get; set; }
        public int SubjectId { get; set; }
        public int TopicId { get; set; }
        public int Marks { get; set; } = 1;
        public decimal NegativeMarks { get; set; } = 0;
        public string DifficultyLevel { get; set; } = "Medium";
        public string CorrectAnswer { get; set; } = string.Empty;
        public bool SameExplanationForAllLanguages { get; set; }
        public bool IsPublished { get; set; }
        public int CreatedBy { get; set; }
        public List<QuestionTranslationUpsertDto> Translations { get; set; } = new();
    }

    public class UpdateQuestionAdminDto : CreateQuestionAdminDto
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public int? MockTestId { get; set; }
    }

    public class QuestionAdminListItemDto
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public int ExamId { get; set; }
        public string? ExamName { get; set; }
        public int SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public int TopicId { get; set; }
        public string? TopicName { get; set; }
        public string DifficultyLevel { get; set; } = "Medium";
        public int Marks { get; set; }
        public decimal NegativeMarks { get; set; }
        public bool IsPublished { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? DisplayQuestionText { get; set; }
        public string? LanguageCode { get; set; }
        
        // Additional fields for complete question data
        public string QuestionText { get; set; } = string.Empty;
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        public string CorrectAnswer { get; set; } = string.Empty;
        public string? Explanation { get; set; }
        public string QuestionType { get; set; } = "MCQ";
        public string? QuestionImageUrl { get; set; }
        public string? OptionAImageUrl { get; set; }
        public string? OptionBImageUrl { get; set; }
        public string? OptionCImageUrl { get; set; }
        public string? OptionDImageUrl { get; set; }
        public string? ExplanationImageUrl { get; set; }
        public bool SameExplanationForAllLanguages { get; set; }
        public string? Reference { get; set; }
        public string? Tags { get; set; }
        public int CreatedBy { get; set; }
        public int? ReviewedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? PublishDate { get; set; }
        public int? MockTestId { get; set; }
        public string? MockTestName { get; set; }
        public int? MockTestTypeId { get; set; }
        
        // Translation fields for multi-language support
        public string? TranslatedOptionA { get; set; }
        public string? TranslatedOptionB { get; set; }
        public string? TranslatedOptionC { get; set; }
        public string? TranslatedOptionD { get; set; }
        public string? TranslatedExplanation { get; set; }
    }

    public class QuestionAdminDetailDto
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public int ExamId { get; set; }
        public int SubjectId { get; set; }
        public int TopicId { get; set; }
        public int Marks { get; set; }
        public decimal NegativeMarks { get; set; }
        public string DifficultyLevel { get; set; } = "Medium";
        public string CorrectAnswer { get; set; } = string.Empty;
        public bool SameExplanationForAllLanguages { get; set; }
        public bool IsPublished { get; set; }
        public bool IsActive { get; set; }
        public List<QuestionTranslationDto> Translations { get; set; } = new();
    }

    public class QuestionDashboardStatsDto
    {
        public int TotalQuestions { get; set; }
        public int AddedToday { get; set; }
        public int NegativeMarksCount { get; set; }
        public int UnpublishedCount { get; set; }
    }

    public class QuestionFilterRequestDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int? ModuleId { get; set; }
        public int? SubjectId { get; set; }
        public int? ExamId { get; set; }
        public int? TopicId { get; set; }
        public int? MockTestId { get; set; }
        public string? GroupBy { get; set; } // "module"(default), "mocktest"
        public string? DifficultyLevel { get; set; }
        public string? LanguageCode { get; set; }
        public bool? IsPublished { get; set; }
        public bool IncludeInactive { get; set; } = true;
    }

    public class QuestionPagedResponseDto
    {
        public IReadOnlyList<QuestionDto> Items { get; set; } = new List<QuestionDto>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class PublishQuestionRequestDto
    {
        [Required]
        public int QuestionId { get; set; }
        
        [Required]
        public bool IsPublished { get; set; }
        
        [Required]
        public int ReviewedBy { get; set; }
    }

    public class BulkQuestionUploadRequestDto
    {
        [Required]
        public string FileName { get; set; } = string.Empty;
        
        [Required]
        public int ExamId { get; set; }
        
        [Required]
        public int SubjectId { get; set; }
        
        public int? TopicId { get; set; }
        
        [Required]
        public int UploadedBy { get; set; }
        
        public string? LanguageCode { get; set; } = "en";
        
        public bool SkipDuplicates { get; set; } = false;
        
        public bool ValidateOnly { get; set; } = false;
    }

    // Image Upload DTOs
    public class QuestionImageUploadDto
    {
        [Required]
        public IFormFile Image { get; set; } = null!;
        
        [Required]
        public string ImageType { get; set; } = string.Empty; // Question, OptionA, OptionB, OptionC, OptionD, Explanation
        
        public int? QuestionId { get; set; }
        
        public string? LanguageCode { get; set; } = "en";
    }

    public class ImageUploadResponseDto
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string ImageType { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }

    // Create/Update Question with Images DTOs
    public class CreateQuestionWithImagesDto
    {
        [Required]
        public int? ModuleId { get; set; }
        
        [Required]
        public int ExamId { get; set; }
        
        [Required]
        public int SubjectId { get; set; }
        
        public int? TopicId { get; set; }
        
        [Required]
        public string QuestionText { get; set; } = string.Empty;
        
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        
        [Required]
        public string CorrectAnswer { get; set; } = string.Empty;
        
        public string? Explanation { get; set; }
        
        [Required]
        public decimal Marks { get; set; }
        
        public decimal NegativeMarks { get; set; }
        
        public string DifficultyLevel { get; set; } = "Medium";
        
        [Required]
        public string QuestionType { get; set; } = "MCQ";
        
        // Image files
        public IFormFile? QuestionImage { get; set; }
        public IFormFile? OptionAImage { get; set; }
        public IFormFile? OptionBImage { get; set; }
        public IFormFile? OptionCImage { get; set; }
        public IFormFile? OptionDImage { get; set; }
        public IFormFile? ExplanationImage { get; set; }
        
        // Existing image URLs (if any)
        public string? QuestionImageUrl { get; set; }
        public string? OptionAImageUrl { get; set; }
        public string? OptionBImageUrl { get; set; }
        public string? OptionCImageUrl { get; set; }
        public string? OptionDImageUrl { get; set; }
        public string? ExplanationImageUrl { get; set; }
        
        public bool SameExplanationForAllLanguages { get; set; } = false;
        public string? Reference { get; set; }
        public string? Tags { get; set; }
        public int CreatedBy { get; set; }
    }

    public class UpdateQuestionWithImagesDto
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public int? ModuleId { get; set; }
        
        public int? ExamId { get; set; }
        public int? MockTestId { get; set; }
        
        [Required]
        public int SubjectId { get; set; }
        
        public int? TopicId { get; set; }
        
        [Required]
        public string QuestionText { get; set; } = string.Empty;
        
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        
        [Required]
        public string CorrectAnswer { get; set; } = string.Empty;
        
        public string? Explanation { get; set; }
        
        [Required]
        public decimal Marks { get; set; }
        
        public decimal? NegativeMarks { get; set; }
        
        public string? DifficultyLevel { get; set; }
        public string? QuestionType { get; set; }
        
        // Image files
        public IFormFile? QuestionImage { get; set; }
        public IFormFile? OptionAImage { get; set; }
        public IFormFile? OptionBImage { get; set; }
        public IFormFile? OptionCImage { get; set; }
        public IFormFile? OptionDImage { get; set; }
        public IFormFile? ExplanationImage { get; set; }
        
        // Existing image URLs (if any)
        public string? QuestionImageUrl { get; set; }
        public string? OptionAImageUrl { get; set; }
        public string? OptionBImageUrl { get; set; }
        public string? OptionCImageUrl { get; set; }
        public string? OptionDImageUrl { get; set; }
        public string? ExplanationImageUrl { get; set; }
        
        public bool? SameExplanationForAllLanguages { get; set; }
        public string? Reference { get; set; }
        public string? Tags { get; set; }
        public string? TranslationsJson { get; set; }
    }

    // Quiz and Subject Management DTOs
    public class SubjectListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int QuestionCount { get; set; }
        public bool IsActive { get; set; }
    }

    public class ExamListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int QuestionCount { get; set; }
        public int Duration { get; set; } // in minutes
        public bool IsActive { get; set; }
        [JsonConverter(typeof(BoolIntJsonConverter))]
        public bool IsLocked { get; set; } // For mobile app unlock functionality
    }

    public class QuizQuestionDto
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        public string? QuestionImageUrl { get; set; }
        public string? OptionAImageUrl { get; set; }
        public string? OptionBImageUrl { get; set; }
        public string? OptionCImageUrl { get; set; }
        public string? OptionDImageUrl { get; set; }
        public string? ExplanationImageUrl { get; set; }
        public decimal Marks { get; set; }
        public decimal NegativeMarks { get; set; }
        public string DifficultyLevel { get; set; } = "Medium";
        public int QuestionNumber { get; set; }
        public bool IsMarkedForReview { get; set; }
        public bool IsAnswered { get; set; }
        public string? SelectedAnswer { get; set; }
        public int TimeLimitInSeconds { get; set; }
        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableUntil { get; set; }
        public bool CanAnswer { get; set; } = true;
        public bool IsReported { get; set; }
        public bool IsBookmarked { get; set; }
        public List<QuestionTranslationDto> Translations { get; set; } = new();
    }

    public class QuizSessionDto
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string LanguageCode { get; set; } = "en";
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int Duration { get; set; } // in minutes
        public int TotalQuestions { get; set; }
        public int AnsweredQuestions { get; set; }
        public int MarkedForReview { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal ObtainedMarks { get; set; }
        public string Status { get; set; } = "NotStarted"; // NotStarted, InProgress, Completed, Submitted
        public List<QuizQuestionDto> Questions { get; set; } = new();
    }

    public class QuizStartRequestDto
    {
        [Required]
        public int ExamId { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        public string LanguageCode { get; set; } = "en";
        
        public int? SubjectId { get; set; }
        
        public int? TopicId { get; set; }
        
        public int NumberOfQuestions { get; set; } = 100;
        
        public string? DifficultyLevel { get; set; }
    }

    public class QuizAnswerRequestDto
    {
        [Required]
        public int QuizSessionId { get; set; }
        
        [Required]
        public int QuestionId { get; set; }
        
        [Required]
        public string Answer { get; set; } = string.Empty;
        
        public bool MarkForReview { get; set; } = false;
        
        public int TimeSpent { get; set; } = 0; // in seconds
    }

    public class QuizSubmitRequestDto
    {
        [Required]
        public int QuizSessionId { get; set; }
        
        public List<QuizAnswerRequestDto> Answers { get; set; } = new();
    }

    public class QuizResultDto
    {
        public int QuizSessionId { get; set; }
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public int SkippedQuestions { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal ObtainedMarks { get; set; }
        public decimal Percentage { get; set; }
        public string Grade { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<QuestionResultDto> QuestionResults { get; set; } = new();
    }

    public class QuestionResultDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        public string CorrectAnswer { get; set; } = string.Empty;
        public string? UserAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public decimal Marks { get; set; }
        public decimal NegativeMarks { get; set; }
        public decimal ObtainedMarks { get; set; }
        public bool WasMarkedForReview { get; set; }
        public int TimeSpent { get; set; } // in seconds
    }

    // Bulk Upload Processing DTOs
    public class BulkUploadProcessDto
    {
        public int BatchId { get; set; }
        public string BatchName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public int ProcessedQuestions { get; set; }
        public int FailedQuestions { get; set; }
        public int SuccessQuestions { get; set; }
        public List<string> ErrorMessages { get; set; } = new();
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    public class ExcelQuestionRowDto
    {
        public int RowNumber { get; set; }
        public int? QuestionId { get; set; }
        public string? ExternalQuestionCode { get; set; }
        public string? Module { get; set; }
        public int? MockTestId { get; set; }
        public string Exam { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string? Topic { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        public string CorrectAnswer { get; set; } = string.Empty;
        public string? Explanation { get; set; }
        public decimal Marks { get; set; } = 1;
        public decimal NegativeMarks { get; set; } = 0;
        public string DifficultyLevel { get; set; } = "Medium";
        public string QuestionType { get; set; } = "MCQ";
        public string? QuestionImageUrl { get; set; }
        public string? OptionAImageUrl { get; set; }
        public string? OptionBImageUrl { get; set; }
        public string? OptionCImageUrl { get; set; }
        public string? OptionDImageUrl { get; set; }
        public string? ExplanationImageUrl { get; set; }
        public string? Reference { get; set; }
        public string? Tags { get; set; }
        public bool SameExplanationForAllLanguages { get; set; } = false;
        public List<QuestionTranslationCreateDto>? Translations { get; set; }
    }

    public class BulkQuestionFileUploadRequestDto
    {
        [Required]
        public IFormFile File { get; set; } = null!;

        [Required]
        public int ModuleId { get; set; }

        public int? MockTestId { get; set; }
        public int? ExamId { get; set; }

        [Required]
        public int SubjectId { get; set; }

        public int? TopicId { get; set; }
        public string? Mode { get; set; } = "create";
        public bool ContinueOnError { get; set; } = true;
        public string? LanguageCode { get; set; } = "en";
    }

    public class BulkQuestionRowResultDto
    {
        public int RowNumber { get; set; }
        public bool Success { get; set; }
        public int? QuestionId { get; set; }
        public string? Action { get; set; }
        public string? Error { get; set; }
    }

    public class BulkQuestionFileUploadResultDto
    {
        public int TotalRows { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public int? MockTestId { get; set; }
        public int ModuleId { get; set; }
        public int ExamId { get; set; }
        public int SubjectId { get; set; }
        public int? TopicId { get; set; }
        public List<BulkQuestionRowResultDto> Rows { get; set; } = new();
    }

    // Simple Question Creation DTO with Exam Integration
    public class SimpleQuestionCreateDto
    {
        [Required]
        public int ExamId { get; set; }
        
        [Required]
        public string QuestionText { get; set; } = string.Empty;
        
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        
        [Required]
        [StringLength(1)]
        public string CorrectAnswer { get; set; } = string.Empty;
        
        public string? Explanation { get; set; }
        
        [Required]
        public int CreatedBy { get; set; }
    }

    public class ExamTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ExamType { get; set; } = string.Empty;
    }

    public class ExamNameDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ExamType { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public bool HasNegativeMarking { get; set; }
        public decimal? NegativeMarkingValue { get; set; }
        public decimal MarksPerQuestion { get; set; }
    }
}
