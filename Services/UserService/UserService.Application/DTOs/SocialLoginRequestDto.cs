using System.ComponentModel.DataAnnotations;

namespace UserService.Application.DTOs
{
    public class SocialLoginRequestDto
    {
        public string? Provider { get; set; } // Google, Facebook, LinkedIn, etc.
        
        public string? GoogleId { get; set; } // Google UID
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public string? AvatarUrl { get; set; }
        
        public string? DeviceId { get; set; }
        
        public string? DeviceType { get; set; }
        
        public string? DeviceName { get; set; }
        
        public string? FcmToken { get; set; }
        
        public string? AccessToken { get; set; }
        
        public string? RefreshToken { get; set; }
        
        public DateTime? ExpiresAt { get; set; }
    }
}
