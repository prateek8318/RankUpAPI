using System.ComponentModel.DataAnnotations;

namespace ExamService.Domain.Entities
{
    public class ExamCategory : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
