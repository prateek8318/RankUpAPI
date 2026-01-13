using System.ComponentModel.DataAnnotations;

namespace RankUpAPI.Models
{
    public class Skill : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string? Description { get; set; }
        
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
