using System.ComponentModel.DataAnnotations;

namespace MasterService.Domain.Entities
{
    public class Language : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(10)]
        public string? Code { get; set; }
    }
}
