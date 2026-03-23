using System.ComponentModel.DataAnnotations;

namespace MasterService.Domain.Entities
{
    public class Category : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string NameEn { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? NameHi { get; set; }

        [Required]
        [MaxLength(50)]
        public string Key { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty;

        // For backward compatibility with existing code
        public string Name => NameEn;

        [MaxLength(500)]
        public string? Description { get; set; }

        public int DisplayOrder { get; set; } = 0;

        // Status field instead of IsActive
        public string Status { get; set; } = "active"; // active, inactive, draft, archived
        
        // Keep IsActive for backward compatibility
        public bool IsActive 
        { 
            get => Status == "active";
            set => Status = value ? "active" : "inactive";
        }
    }
}

