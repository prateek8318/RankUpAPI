using System.ComponentModel.DataAnnotations;

namespace RankUpAPI.Models
{
    public class ProfileUpdateRequest
    {
        [MaxLength(100)]
        public string? Name { get; set; }
        
        [EmailAddress, MaxLength(100)]
        public string? Email { get; set; }
        
        [MaxLength(20)]
        public string? Gender { get; set; }
        
        public DateTime? DateOfBirth { get; set; }
        
        [MaxLength(100)]
        public string? Qualification { get; set; }
        
        [MaxLength(50)]
        public string? LanguagePreference { get; set; }
        
        [MaxLength(100)]
        public string? PreferredExam { get; set; }
    }
}
