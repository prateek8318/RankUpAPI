using System.ComponentModel.DataAnnotations;

namespace TestService.Domain.Entities
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

        public int DisplayOrder { get; set; } = 0;
        public bool IsFeatured { get; set; } = false;
    }

    // Static practice mode definitions as per requirements
    public static class PracticeModeIds
    {
        public const int MockTest = 3;
        public const int TestSeries = 4;
        public const int DeepPractice = 5;
        public const int PreviousYear = 6;
    }
}
