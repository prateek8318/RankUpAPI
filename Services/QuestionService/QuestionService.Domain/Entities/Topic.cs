using System.ComponentModel.DataAnnotations;

namespace QuestionService.Domain.Entities
{
    public class Topic : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int SubjectId { get; set; } // FK to Master Service Subjects table

        [MaxLength(500)]
        public string? Description { get; set; }

        public int? ParentTopicId { get; set; } // For hierarchical topics

        [Required]
        public int SortOrder { get; set; } = 0;

        // Navigation properties
        public virtual Topic? ParentTopic { get; set; }
        public virtual ICollection<Topic> ChildTopics { get; set; } = new List<Topic>();
        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
