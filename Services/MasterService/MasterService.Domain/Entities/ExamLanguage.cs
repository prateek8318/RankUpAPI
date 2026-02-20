using System.ComponentModel.DataAnnotations;

namespace MasterService.Domain.Entities
{
    public class ExamLanguage : BaseEntity
    {
        [Required]
        public int ExamId { get; set; }

        [Required]
        public int LanguageId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public Exam Exam { get; set; } = null!;
        public Language Language { get; set; } = null!;
    }
}

