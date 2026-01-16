using System.ComponentModel.DataAnnotations;

namespace HomeDashboardService.Domain.Entities
{
    public class PracticeMode : BaseEntity
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

    public enum PracticeModeType
    {
        MockTest = 1,
        TestSeries = 2,
        DeepPractice = 3,
        PreviousYear = 4
    }
}
