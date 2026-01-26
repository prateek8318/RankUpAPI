using System.ComponentModel.DataAnnotations;
using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Application.DTOs
{
    public class AdminDashboardMetricsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveSubscriptions { get; set; }
        public int MostAttemptedQuizId { get; set; }
        public string MostAttemptedQuizTitle { get; set; } = string.Empty;
        public decimal DailyRevenue { get; set; }
        public int DailyActiveUsers { get; set; }
        public MetricTrendsDto Trends { get; set; } = new();
    }

    public class MetricTrendsDto
    {
        public decimal UsersChangePercentage { get; set; }
        public decimal SubscriptionsChangePercentage { get; set; }
        public decimal RevenueChangePercentage { get; set; }
        public decimal DailyActiveUsersChangePercentage { get; set; }
        public bool IsUsersUp { get; set; }
        public bool IsSubscriptionsUp { get; set; }
        public bool IsRevenueUp { get; set; }
        public bool IsDAUUp { get; set; }
    }

    // Exam DTOs
    public class ExamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateExamDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        public int DisplayOrder { get; set; } = 0;
    }

    public class UpdateExamDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        public int DisplayOrder { get; set; } = 0;
    }

    // Subject DTOs
    public class SubjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateSubjectDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int ExamId { get; set; }

        public int DisplayOrder { get; set; } = 0;
    }

    public class UpdateSubjectDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public int DisplayOrder { get; set; } = 0;
    }

    // Chapter DTOs
    public class ChapterDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateChapterDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int SubjectId { get; set; }

        public int DisplayOrder { get; set; } = 0;
    }

    public class UpdateChapterDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public int DisplayOrder { get; set; } = 0;
    }

    // Quiz DTOs
    public class QuizDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ChapterId { get; set; }
        public string ChapterName { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public int TotalQuestions { get; set; }
        public int PassingMarks { get; set; }
        public QuizType Type { get; set; }
        public QuizDifficulty Difficulty { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateQuizDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int ChapterId { get; set; }

        public int DurationMinutes { get; set; }

        public int TotalQuestions { get; set; }

        public int PassingMarks { get; set; }

        public QuizType Type { get; set; } = QuizType.Practice;

        public QuizDifficulty Difficulty { get; set; } = QuizDifficulty.Medium;

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }

    public class UpdateQuizDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public int DurationMinutes { get; set; }

        public int TotalQuestions { get; set; }

        public int PassingMarks { get; set; }

        public QuizType Type { get; set; }

        public QuizDifficulty Difficulty { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }

    // Question DTOs
    public class QuestionDto
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }
        public string? Explanation { get; set; }
        public QuestionDifficulty Difficulty { get; set; }
        public int Marks { get; set; }
        public int DisplayOrder { get; set; }
        public List<QuestionOptionDto> Options { get; set; } = new();
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateQuestionDto
    {
        [Required]
        public int QuizId { get; set; }

        [Required]
        [MaxLength(2000)]
        public string QuestionText { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        public string? VideoUrl { get; set; }

        [MaxLength(2000)]
        public string? Explanation { get; set; }

        public QuestionDifficulty Difficulty { get; set; } = QuestionDifficulty.Medium;

        public int Marks { get; set; } = 1;

        public int DisplayOrder { get; set; } = 0;

        [Required]
        [MinLength(2)]
        public List<CreateQuestionOptionDto> Options { get; set; } = new();
    }

    public class UpdateQuestionDto
    {
        [Required]
        [MaxLength(2000)]
        public string QuestionText { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        public string? VideoUrl { get; set; }

        [MaxLength(2000)]
        public string? Explanation { get; set; }

        public QuestionDifficulty Difficulty { get; set; }

        public int Marks { get; set; }

        public int DisplayOrder { get; set; }

        [Required]
        [MinLength(2)]
        public List<CreateQuestionOptionDto> Options { get; set; } = new();
    }

    public class QuestionOptionDto
    {
        public int Id { get; set; }
        public string OptionText { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsCorrect { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class CreateQuestionOptionDto
    {
        [Required]
        [MaxLength(1000)]
        public string OptionText { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public bool IsCorrect { get; set; } = false;

        public int DisplayOrder { get; set; } = 0;
    }

    // Bulk Upload DTOs
    public class BulkUploadResultDto
    {
        public int BulkUploadLogId { get; set; }
        public int TotalRows { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public BulkUploadStatus Status { get; set; }
        public string? ErrorReportPath { get; set; }
        public string? ErrorSummary { get; set; }
        public List<BulkUploadErrorDto> Errors { get; set; } = new();
    }

    public class BulkUploadErrorDto
    {
        public int RowNumber { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public string? RowData { get; set; }
    }

    public class BulkUploadLogDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public int TotalRows { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public BulkUploadStatus Status { get; set; }
        public string? ErrorReportPath { get; set; }
        public string? ErrorSummary { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public int ProcessedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Admin DTOs for Home Page Management
    public class CreateHomeBannerDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        public string? LinkUrl { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsPromoted { get; set; } = false;
    }

    public class UpdateHomeBannerDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        public string? LinkUrl { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsPromoted { get; set; } = false;

        public bool IsActive { get; set; } = true;
    }

    public class CreateOfferBannerDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        public string? LinkUrl { get; set; }

        [MaxLength(100)]
        public string? DiscountCode { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }

    public class UpdateOfferBannerDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        public string? LinkUrl { get; set; }

        [MaxLength(100)]
        public string? DiscountCode { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class CreatePracticeModeDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? IconUrl { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        public string? LinkUrl { get; set; }

        public PracticeModeType Type { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsFeatured { get; set; } = false;
    }

    public class UpdatePracticeModeDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? IconUrl { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        public string? LinkUrl { get; set; }

        public PracticeModeType Type { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsFeatured { get; set; } = false;

        public bool IsActive { get; set; } = true;
    }

    public class CreateRapidFireTestDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [Required]
        public int QuizId { get; set; }

        public int DurationSeconds { get; set; } = 60;

        public int TotalQuestions { get; set; } = 10;

        public int DisplayOrder { get; set; } = 0;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsFeatured { get; set; } = false;
    }

    public class UpdateRapidFireTestDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [Required]
        public int QuizId { get; set; }

        public int DurationSeconds { get; set; } = 60;

        public int TotalQuestions { get; set; } = 10;

        public int DisplayOrder { get; set; } = 0;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsFeatured { get; set; } = false;

        public bool IsActive { get; set; } = true;
    }

    public class CreateFreeTestDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        public string? ThumbnailUrl { get; set; }

        [Required]
        public int QuizId { get; set; }

        [Required]
        public int ExamId { get; set; }

        public int DurationMinutes { get; set; } = 60;

        public int TotalQuestions { get; set; } = 0;

        public int TotalMarks { get; set; } = 100;

        public int DisplayOrder { get; set; } = 0;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsFeatured { get; set; } = false;

        [MaxLength(500)]
        public string? LinkUrl { get; set; }
    }

    public class UpdateFreeTestDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        public string? ThumbnailUrl { get; set; }

        [Required]
        public int QuizId { get; set; }

        [Required]
        public int ExamId { get; set; }

        public int DurationMinutes { get; set; } = 60;

        public int TotalQuestions { get; set; } = 0;

        public int TotalMarks { get; set; } = 100;

        public int DisplayOrder { get; set; } = 0;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsFeatured { get; set; } = false;

        [MaxLength(500)]
        public string? LinkUrl { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class CreateMotivationMessageDto
    {
        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Author { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public MessageType Type { get; set; } = MessageType.Daily;

        public int DisplayOrder { get; set; } = 0;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsGreeting { get; set; } = false;
    }

    public class UpdateMotivationMessageDto
    {
        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Author { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public MessageType Type { get; set; } = MessageType.Daily;

        public int DisplayOrder { get; set; } = 0;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsGreeting { get; set; } = false;

        public bool IsActive { get; set; } = true;
    }

    public class CreateSubscriptionBannerDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        public string? LinkUrl { get; set; }

        [MaxLength(100)]
        public string? CtaText { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool ShowToSubscribedUsers { get; set; } = false;

        public bool ShowToNonSubscribedUsers { get; set; } = true;
    }

    public class UpdateSubscriptionBannerDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        public string? LinkUrl { get; set; }

        [MaxLength(100)]
        public string? CtaText { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool ShowToSubscribedUsers { get; set; } = false;

        public bool ShowToNonSubscribedUsers { get; set; } = true;

        public bool IsActive { get; set; } = true;
    }

    public class TrendingTestDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int QuizId { get; set; }
        public string QuizTitle { get; set; } = string.Empty;
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalMarks { get; set; }
        public bool IsFeatured { get; set; }
        public int AttemptCount { get; set; }
        public int TotalAttempts { get; set; }
        public decimal AverageScore { get; set; }
        public string Type { get; set; } = string.Empty;
    }

    public class UpdateTrendingTestDto
    {
        [MaxLength(200)]
        public string? Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public int? QuizId { get; set; }

        public int? ExamId { get; set; }

        public int? DurationMinutes { get; set; }

        public int? TotalQuestions { get; set; }

        public int? TotalMarks { get; set; }

        public bool? IsFeatured { get; set; }

        public int? AttemptCount { get; set; }

        public decimal? AverageScore { get; set; }
    }

    // Rapid Fire Test with Image Upload DTOs
    public class CreateRapidFireTestWithImagesDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int QuizId { get; set; }

        public int DurationSeconds { get; set; } = 60;

        public int TotalQuestions { get; set; } = 10;

        public int DisplayOrder { get; set; } = 0;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsFeatured { get; set; } = false;
    }

    public class UpdateRapidFireTestWithImagesDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int QuizId { get; set; }

        public int DurationSeconds { get; set; } = 60;

        public int TotalQuestions { get; set; } = 10;

        public int DisplayOrder { get; set; } = 0;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsFeatured { get; set; } = false;

        public bool IsActive { get; set; } = true;
    }

    // Rapid Fire Test Images Response DTO
    public class RapidFireTestImagesDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? LogoUrl { get; set; }
        public string? BackgroundImageUrl { get; set; }
        public int QuizId { get; set; }
        public int DurationSeconds { get; set; }
        public int TotalQuestions { get; set; }
        public bool IsFeatured { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
