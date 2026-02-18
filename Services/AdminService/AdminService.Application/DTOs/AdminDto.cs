namespace AdminService.Application.DTOs
{
    public class AdminDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Role { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new();
        public bool IsTwoFactorEnabled { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class AdminLoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AdminAuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public AdminDto? Admin { get; set; }
    }

    public class AdminLoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool RequiresTwoFactor { get; set; }
        public string? MobileNumber { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public AdminDto? Admin { get; set; }
    }

    public class AdminOtpVerificationRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }

    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<int> PermissionIds { get; set; } = new();
    }

    public class PermissionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Resource { get; set; }
        public string? Action { get; set; }
        public string? Description { get; set; }
    }
}
