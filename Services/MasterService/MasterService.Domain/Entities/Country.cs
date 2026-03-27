using System.ComponentModel.DataAnnotations;

namespace MasterService.Domain.Entities
{
    public class Country : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(2)]
        public string Iso2 { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(5)]
        public string CountryCode { get; set; } = string.Empty;
        
        public int PhoneLength { get; set; } = 10;
        
        [MaxLength(3)]
        public string CurrencyCode { get; set; } = string.Empty;
        
        [MaxLength(255)]
        public string? Image { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
