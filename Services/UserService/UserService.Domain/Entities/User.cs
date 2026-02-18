using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Language;

namespace UserService.Domain.Entities
{
    public class User : BaseEntity
    {
        [MaxLength(100)]
        public string? Name { get; set; }
        
        [EmailAddress, MaxLength(100)]
        public string? Email { get; set; }
        
        [MaxLength(255)]
        public string? PasswordHash { get; set; }
        
        [Required, MaxLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [MaxLength(10)]
        public string? CountryCode { get; set; } = "+91";
        
        [MaxLength(20)]
        public string? Gender { get; set; }
        
        [Column(TypeName = "date")]
        public DateTime? DateOfBirth { get; set; }
        
        [MaxLength(100)]
        public string? Qualification { get; set; }
        
        [MaxLength(5)]
        [LanguageValidation]
        public string? PreferredLanguage { get; set; }
        
        [MaxLength(255)]
        public string? ProfilePhoto { get; set; }
        
        [MaxLength(100)]
        public string? PreferredExam { get; set; }
        
        public int? StateId { get; set; }
        
        public int? LanguageId { get; set; }
        
        public int? QualificationId { get; set; }
        
        public int? ExamId { get; set; }
        
        public int? CategoryId { get; set; }
        
        public int? StreamId { get; set; }
        
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        
        public DateTime? LastLoginAt { get; set; }
        
        public bool IsPhoneVerified { get; set; } = false;
        
        public bool InterestedInIntlExam { get; set; } = false;
    }
}
