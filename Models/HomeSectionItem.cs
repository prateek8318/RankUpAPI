using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RankUpAPI.Models
{
    /// <summary>
    /// Represents a single card/tile on the home screen under a specific section.
    /// Admin will POST these and user app will GET them.
    /// </summary>
    public class HomeSectionItem : BaseEntity
    {
        [Required]
        public HomeSectionType SectionType { get; set; }

        /// <summary>
        /// Optional exam filter – which exam this card belongs to.
        /// </summary>
        public int? ExamId { get; set; }

        [ForeignKey(nameof(ExamId))]
        public Exam? Exam { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Subtitle { get; set; }

        /// <summary>
        /// Progress text like "45%" or "10/20" if needed.
        /// </summary>
        [MaxLength(50)]
        public string? ProgressText { get; set; }

        /// <summary>
        /// Small tag like "New", "Trending", etc.
        /// </summary>
        [MaxLength(50)]
        public string? Tag { get; set; }

        /// <summary>
        /// Image URL that mobile app will use (you will upload somewhere and save URL here).
        /// </summary>
        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Optional deep-link / navigation key for the app.
        /// </summary>
        [MaxLength(200)]
        public string? ActionKey { get; set; }

        /// <summary>
        /// Sort order inside the same section.
        /// </summary>
        public int DisplayOrder { get; set; } = 0;

        /// <summary>
        /// Whether this item should be visible to users.
        /// </summary>
        public bool IsVisible { get; set; } = true;
    }
}

