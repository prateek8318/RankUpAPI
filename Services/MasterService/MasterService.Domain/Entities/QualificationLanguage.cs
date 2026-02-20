using System.ComponentModel.DataAnnotations;

namespace MasterService.Domain.Entities
{
    public class QualificationLanguage : BaseEntity
    {
        [Required]
        public int QualificationId { get; set; }

        [Required]
        public int LanguageId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public Qualification Qualification { get; set; } = null!;
        public Language Language { get; set; } = null!;
    }
}
