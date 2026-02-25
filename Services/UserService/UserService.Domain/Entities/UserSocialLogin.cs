using System;

namespace UserService.Domain.Entities
{
    public class UserSocialLogin
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Provider { get; set; } // Google, Facebook, LinkedIn, etc.
        public string? GoogleId { get; set; } // Google UID
        public string Email { get; set; }
        public string Name { get; set; }
        public string? AvatarUrl { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public virtual User User { get; set; }
    }
}
