using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MasterService.Application.DTOs
{
    public class CreateCountryWithImageDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Iso2 is required")]
        [MaxLength(2, ErrorMessage = "Iso2 cannot exceed 2 characters")]
        public string Iso2 { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "CountryCode is required")]
        [MaxLength(5, ErrorMessage = "CountryCode cannot exceed 5 characters")]
        public string CountryCode { get; set; } = string.Empty;
        
        public int PhoneLength { get; set; } = 10;
        
        [Required(ErrorMessage = "CurrencyCode is required")]
        [MaxLength(3, ErrorMessage = "CurrencyCode cannot exceed 3 characters")]
        public string CurrencyCode { get; set; } = string.Empty;
        
        public IFormFile? ImageFile { get; set; }
    }

    public class UpdateCountryWithImageDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Iso2 is required")]
        [MaxLength(2, ErrorMessage = "Iso2 cannot exceed 2 characters")]
        public string Iso2 { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "CountryCode is required")]
        [MaxLength(5, ErrorMessage = "CountryCode cannot exceed 5 characters")]
        public string CountryCode { get; set; } = string.Empty;
        
        public int PhoneLength { get; set; } = 10;
        
        [Required(ErrorMessage = "CurrencyCode is required")]
        [MaxLength(3, ErrorMessage = "CurrencyCode cannot exceed 3 characters")]
        public string CurrencyCode { get; set; } = string.Empty;
        
        public IFormFile? ImageFile { get; set; }
        
        public bool IsActive { get; set; }
    }
}
