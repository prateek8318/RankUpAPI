using System.ComponentModel.DataAnnotations;

namespace MasterService.Domain.Entities
{
    public class Stream : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public int QualificationId { get; set; }

        public Qualification Qualification { get; set; } = null!;
        public ICollection<StreamLanguage> StreamLanguages { get; set; } = new List<StreamLanguage>();
    }
}
