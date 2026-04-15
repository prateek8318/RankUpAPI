using System.ComponentModel.DataAnnotations;

namespace ExamService.Domain.Entities
{
    public class ExamType : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public int ExamCategoryId { get; set; }
        public ExamCategory? ExamCategory { get; set; }
    }
}
