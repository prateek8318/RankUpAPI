using System.ComponentModel.DataAnnotations;

namespace MasterService.Domain.Entities
{
    public class State : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(10)]
        public string? Code { get; set; }
        
        [MaxLength(10)]
        public string? CountryCode { get; set; }
    }
}
