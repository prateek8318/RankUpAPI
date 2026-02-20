using System.ComponentModel.DataAnnotations;

namespace MasterService.Domain.Entities
{
    public class StreamLanguage : BaseEntity
    {
        [Required]
        public int StreamId { get; set; }

        [Required]
        public int LanguageId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public Stream Stream { get; set; } = null!;
        public Language Language { get; set; } = null!;
    }
}
