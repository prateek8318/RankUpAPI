using System.ComponentModel.DataAnnotations;

namespace MasterService.Application.DTOs
{
    public class PhoneDto
    {
        public string CountryCode { get; set; } = string.Empty;
        public int Length { get; set; }
    }

    public class CountryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Iso2 { get; set; } = string.Empty;
        public PhoneDto Phone { get; set; } = new PhoneDto();
        public string CurrencyCode { get; set; } = string.Empty;
        public string? Image { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateCountryDto
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
        
        [Required]
        [MaxLength(3)]
        public string CurrencyCode { get; set; } = string.Empty;
        
        [MaxLength(255)]
        public string? Image { get; set; }
    }

    public class UpdateCountryDto
    {
        public int Id { get; set; }
        
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
        
        [Required]
        [MaxLength(3)]
        public string CurrencyCode { get; set; } = string.Empty;
        
        [MaxLength(255)]
        public string? Image { get; set; }
        
        public bool IsActive { get; set; }
    }
}
