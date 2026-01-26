using System.ComponentModel.DataAnnotations;

namespace QualificationService.Domain.Entities
{
    public class Qualification : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public int? StreamId { get; set; }
        
        // Navigation property
        public virtual Stream? Stream { get; set; }
    }
}
