using System.ComponentModel.DataAnnotations;

namespace RankUpAPI.Models
{
    public enum CMSType
    {
        AboutUs = 1,
        PrivacyPolicy = 2,
        TermsAndConditions = 3,
        FAQ = 4,
        Help = 5,
        Other = 6
    }

    public class CMSContent : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty; // HTML content
        
        public CMSType Type { get; set; }
        
        [MaxLength(50)]
        public string? Slug { get; set; }
        
        [MaxLength(500)]
        public string? MetaDescription { get; set; }
        
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; } = 0;
    }
}
