using System.ComponentModel.DataAnnotations;

namespace MasterService.Application.DTOs
{
    public class CountryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? SubdivisionLabelEn { get; set; }
        public string? SubdivisionLabelHi { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateCountryDto
    {
        [Required]
        [MaxLength(100)]
        public string NameEn { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string NameHi { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(10)]
        public string Code { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? SubdivisionLabelEn { get; set; }
        
        [MaxLength(50)]
        public string? SubdivisionLabelHi { get; set; }
    }

    public class UpdateCountryDto
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string NameEn { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string NameHi { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(10)]
        public string Code { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? SubdivisionLabelEn { get; set; }
        
        [MaxLength(50)]
        public string? SubdivisionLabelHi { get; set; }
        
        public bool IsActive { get; set; }
    }
}
