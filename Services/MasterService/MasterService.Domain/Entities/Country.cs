using System.ComponentModel.DataAnnotations;

namespace MasterService.Domain.Entities
{
    public class Country : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(10)]
        public string Code { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? SubdivisionLabelEn { get; set; }
        
        [MaxLength(50)]
        public string? SubdivisionLabelHi { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
