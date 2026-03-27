using System.ComponentModel.DataAnnotations;

namespace MasterService.Application.DTOs
{
    // State DTOs with inheritance from base classes
    public class StateDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? CountryCode { get; set; }
        public List<StateLanguageDto> Names { get; set; } = new();
    }

    public class StateLanguageDto
    {
        public int LanguageId { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
        public string LanguageName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class CreateStateDto : BaseCreateDto
    {
        [Required(ErrorMessage = "Code is required")]
        [StringLength(10, ErrorMessage = "Code cannot exceed 10 characters")]
        public string? Code { get; set; }

        [Required(ErrorMessage = "Country code is required")]
        [StringLength(10, ErrorMessage = "Country code cannot exceed 10 characters")]
        public string? CountryCode { get; set; }

        [Required(ErrorMessage = "At least one language name must be provided")]
        [MinLength(1, ErrorMessage = "At least one language name must be provided")]
        public List<StateLanguageCreateDto> Names { get; set; } = new();
    }

    public class StateLanguageCreateDto
    {
        [Required(ErrorMessage = "Language ID is required")]
        public int LanguageId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateStateDto : BaseUpdateDto
    {
        [Required(ErrorMessage = "Code is required")]
        [StringLength(10, ErrorMessage = "Code cannot exceed 10 characters")]
        public string? Code { get; set; }

        [Required(ErrorMessage = "Country code is required")]
        [StringLength(10, ErrorMessage = "Country code cannot exceed 10 characters")]
        public string? CountryCode { get; set; }

        [Required(ErrorMessage = "At least one language name must be provided")]
        [MinLength(1, ErrorMessage = "At least one language name must be provided")]
        public List<StateLanguageCreateDto> Names { get; set; } = new();
    }
}
