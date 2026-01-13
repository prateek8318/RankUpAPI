using System.ComponentModel.DataAnnotations;

namespace QuizService.Domain.Entities
{
    public class Chapter : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public int SubjectId { get; set; }
        public virtual Subject Subject { get; set; } = null!;
    }
}
