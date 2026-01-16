using System.ComponentModel.DataAnnotations;

namespace MasterService.Domain.Entities
{
    public class StateLanguage : BaseEntity
    {
        [Required]
        public int StateId { get; set; }
        
        [Required]
        public int LanguageId { get; set; }
        
        public State State { get; set; } = null!;
        public Language Language { get; set; } = null!;
    }
}
