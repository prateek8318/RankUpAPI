using System.ComponentModel.DataAnnotations;

namespace MasterService.Domain.Entities
{
    /// <summary>
    /// Key-based CMS content (e.g. terms_and_conditions, privacy_policy).
    /// Translations stored in CmsContentTranslation - fully dynamic for any language.
    /// </summary>
    public class CmsContent : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Key { get; set; } = string.Empty;

        public ICollection<CmsContentTranslation> Translations { get; set; } = new List<CmsContentTranslation>();
    }
}
