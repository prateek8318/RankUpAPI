using MasterService.Domain.Entities;

namespace MasterService.Application.DTOs
{
    /// <summary>
    /// User-facing CMS content with localized Title and Content for requested language.
    /// </summary>
    public class CmsContentDto
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;

        /// <summary>Localized title for requested language (fallback to en)</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Localized content for requested language (fallback to en)</summary>
        public string Content { get; set; } = string.Empty;

        public CmsContentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        /// <summary>All translations - for admin panel only</summary>
        public IReadOnlyList<CmsContentTranslationDto>? Translations { get; set; }
    }

    /// <summary>Single language translation - use for Create/Update and admin response.</summary>
    public class CmsContentTranslationDto
    {
        public string LanguageCode { get; set; } = string.Empty; // "en", "hi", "ta", "gu"
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    public class CreateCmsContentDto
    {
        public string Key { get; set; } = string.Empty;

        /// <summary>At least one translation required. "en" is mandatory, others optional.</summary>
        public List<CmsContentTranslationDto> Translations { get; set; } = new();
    }

    public class UpdateCmsContentDto
    {
        public string Key { get; set; } = string.Empty;
        public List<CmsContentTranslationDto> Translations { get; set; } = new();
        public CmsContentStatus Status { get; set; } = CmsContentStatus.Active;
    }

    /// <summary>Schema for status update - use enum directly. PATCH api/cms/{id}/status</summary>
    public class UpdateCmsStatusDto
    {
        public CmsContentStatus Status { get; set; }
    }
}
