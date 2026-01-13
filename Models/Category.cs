using System.ComponentModel.DataAnnotations;

namespace RankUpAPI.Models
{
    public class Category : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string? Description { get; set; }
        
        [MaxLength(255)]
        public string? ImageUrl { get; set; }
        
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
