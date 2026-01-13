using System.ComponentModel.DataAnnotations;

namespace QuizService.Domain.Entities
{
    public class Subject : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public int ExamId { get; set; } // Reference to ExamService

        public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
    }
}
