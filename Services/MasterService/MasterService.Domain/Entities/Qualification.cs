using System.ComponentModel.DataAnnotations;

namespace MasterService.Domain.Entities
{
    public class Qualification : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(10)]
        public string? CountryCode { get; set; }

        public Country? Country { get; set; }
        public ICollection<QualificationLanguage> QualificationLanguages { get; set; } = new List<QualificationLanguage>();
        public ICollection<Stream> Streams { get; set; } = new List<Stream>();
    }
}
