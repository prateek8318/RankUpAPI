using System.ComponentModel.DataAnnotations;

namespace RankUpAPI.Models
{
    public class Language : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(10)]
        public string? Code { get; set; } // en, hi, etc.
        
        public bool IsActive { get; set; } = true;
    }
}
