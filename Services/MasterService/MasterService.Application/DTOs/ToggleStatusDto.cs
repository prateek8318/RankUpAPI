using System.ComponentModel.DataAnnotations;

namespace MasterService.Application.DTOs
{
    public class ToggleStatusDto
    {
        [Required]
        public bool IsActive { get; set; }
    }
}
