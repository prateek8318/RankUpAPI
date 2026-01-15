using System.ComponentModel.DataAnnotations;

namespace HomeDashboardService.Domain.Entities
{
    public class Chapter : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int SubjectId { get; set; }

        public int DisplayOrder { get; set; } = 0;

        // Navigation properties
        public virtual Subject Subject { get; set; } = null!;
        public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    }
}
