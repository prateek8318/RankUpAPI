using System.ComponentModel.DataAnnotations;

namespace TestService.Domain.Entities
{
    public class TestQuestion : BaseEntity
    {
        [Required]
        public int TestId { get; set; }

        [Required]
        public int QuestionId { get; set; }

        public int DisplayOrder { get; set; } = 0;
        public int Marks { get; set; } = 1;

        // Navigation properties
        public virtual Test Test { get; set; } = null!;
        public virtual Question Question { get; set; } = null!;
    }
}
