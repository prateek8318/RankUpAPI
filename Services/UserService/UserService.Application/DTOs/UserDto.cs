using Microsoft.AspNetCore.Http;
namespace UserService.Application.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string? CountryCode { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Qualification { get; set; }
        public string? LanguagePreference { get; set; }
        public string? ProfilePhoto { get; set; }
        public string? ProfilePhotoUrl { get; set; } // Full URL for profile photo
        public string? PreferredExam { get; set; }
        public int? StateId { get; set; }
        public int? LanguageId { get; set; }
        public int? QualificationId { get; set; }
        public int? ExamId { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsPhoneVerified { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ProfileUpdateRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Qualification { get; set; }
        public string? LanguagePreference { get; set; }
        public string? PreferredExam { get; set; }
        public int? StateId { get; set; }
        public int? LanguageId { get; set; }
        public int? QualificationId { get; set; }
        public int? ExamId { get; set; }
    }
    
    public class PatchProfileRequest
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public DateTime? Dob { get; set; }
        public int? StateId { get; set; }
        public int? LanguageId { get; set; }
        public int? QualificationId { get; set; }
        public int? ExamId { get; set; }
    }

    public class PatchProfileFormData
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public DateTime? Dob { get; set; }
        public int? StateId { get; set; }
        public int? LanguageId { get; set; }
        public int? QualificationId { get; set; }
        public int? ExamId { get; set; }
        public IFormFile? ProfilePhoto { get; set; }
    }

    public class OtpRequest
    {
        public string MobileNumber { get; set; } = string.Empty;
        public string CountryCode { get; set; } = "+91"; // Default to India
    }

    public class OtpVerificationRequest
    {
        public string MobileNumber { get; set; } = string.Empty;
        public string CountryCode { get; set; } = "+91"; // Default to India
        public string Otp { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public int? UserId { get; set; }
    }
}
