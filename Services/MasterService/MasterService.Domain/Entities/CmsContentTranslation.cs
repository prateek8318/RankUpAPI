using System.ComponentModel.DataAnnotations;

namespace MasterService.Domain.Entities
{
    /// <summary>
    /// Per-language translation for CMS content. Supports any language from LanguageConstants - fully dynamic.
    /// </summary>
    public class CmsContentTranslation
    {
        public int Id { get; set; }
        public int CmsContentId { get; set; }

        [Required]
        [MaxLength(10)]
        public string LanguageCode { get; set; } = string.Empty; // "en", "hi", "ta", "gu", etc.

        [Required]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public CmsContent CmsContent { get; set; } = null!;
    }
}
