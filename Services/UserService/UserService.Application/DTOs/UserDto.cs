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
        public bool IsNewUser { get; set; } // Added to identify new vs existing user
        public bool InterestedInIntlExam { get; set; }
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
    
    /// <summary>
    /// Partial profile update request - Updates only specified fields
    /// </summary>
    /// <remarks>
    /// **Usage:** Used for partial profile updates (PATCH operations)
    /// **Behavior:** Only non-null fields will be updated
    /// **Flexibility:** Send only the fields you want to update
    /// 
    /// **Field Descriptions:**
    /// - FullName: User's full name (2-100 characters, required for profile completion)
    /// - Email: Valid email address (must be unique across all users)
    /// - Gender: "Male", "Female", or "Other"
    /// - Dob: Date of birth in YYYY-MM-DD format (e.g., "1990-01-15")
    /// - StateId: Valid state ID from language data endpoint (e.g., 1, 2, 3)
    /// - LanguageId: Valid language ID from language data endpoint (e.g., 1, 2, 3)
    /// - QualificationId: Valid qualification ID from language data endpoint (e.g., 1, 2, 3)
    /// - ExamId: Valid exam ID from language data endpoint (e.g., 1, 2, 3)
    /// - InterestedInIntlExam: Boolean flag for international exam preferences (true/false)
    /// 
    /// **Examples:**
    /// ```json
    /// // Update just name
    /// {
    ///   "fullName": "John Doe"
    /// }
    /// 
    /// // Update email and gender
    /// {
    ///   "email": "john.doe@example.com",
    ///   "gender": "Male"
    /// }
    /// 
    /// // Update multiple fields
    /// {
    ///   "fullName": "John Doe",
    ///   "email": "john@example.com",
    ///   "gender": "Male",
    ///   "dob": "1990-01-15",
    ///   "stateId": 1,
    ///   "languageId": 1,
    ///   "interestedInIntlExam": true
    /// }
    /// ```
    /// </remarks>
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
        public bool? InterestedInIntlExam { get; set; }
    }

    /// <summary>
    /// Partial profile update request with photo upload support
    /// </summary>
    /// <remarks>
    /// **Usage:** Used for partial profile updates with file upload (PATCH operations)
    /// **Content-Type:** multipart/form-data
    /// **File Support:** JPG, JPEG, PNG, GIF formats (max 10MB)
    /// 
    /// **Form Fields:**
    /// - All text fields from PatchProfileRequest (all optional)
    /// - ProfilePhoto: IFormFile for image upload (optional)
    /// 
    /// **Field Descriptions:**
    /// - FullName: User's full name (2-100 characters)
    /// - Email: Valid email address (must be unique)
    /// - Gender: "Male", "Female", or "Other"
    /// - Dob: Date of birth in YYYY-MM-DD format (e.g., "1990-01-15")
    /// - StateId: Valid state ID from language data endpoint (e.g., 1, 2, 3)
    /// - LanguageId: Valid language ID from language data endpoint (e.g., 1, 2, 3)
    /// - QualificationId: Valid qualification ID from language data endpoint (e.g., 1, 2, 3)
    /// - ExamId: Valid exam ID from language data endpoint (e.g., 1, 2, 3)
    /// - InterestedInIntlExam: Boolean flag for international exam preferences (true/false)
    /// - ProfilePhoto: Image file (JPG, JPEG, PNG, GIF, max 10MB)
    /// 
    /// **Request Format (multipart/form-data):**
    /// ```
    /// FullName: "John Doe"
    /// Email: "john@example.com"
    /// Gender: "Male"
    /// Dob: "1990-01-15"
    /// StateId: "1"
    /// LanguageId: "1"
    /// InterestedInIntlExam: "true"
    /// ProfilePhoto: [binary file data]
    /// ```
    /// 
    /// **File Processing:**
    /// - Automatic image validation and resizing
    /// - Generates unique filename to prevent conflicts
    /// - Returns full URL in response for frontend display
    /// </remarks>
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
        public bool? InterestedInIntlExam { get; set; }
        public IFormFile? ProfilePhoto { get; set; }
    }

    /// <summary>
    /// OTP request for mobile authentication
    /// </summary>
    /// <remarks>
    /// **Usage:** Send OTP to user's mobile number for login/registration
    /// **Validation Rules:**
    /// - Mobile Number: Exactly 10 digits (0-9), no spaces or special characters
    /// - Country Code: Must start with '+' followed by 1-3 digits (e.g., +91, +1, +44)
    /// - Default Country Code: +91 (India) if not specified
    /// 
    /// **Examples:**
    /// ```json
    /// {
    ///   "mobileNumber": "9876543210",
    ///   "countryCode": "+91"
    /// }
    /// ```
    /// 
    /// **Field Descriptions:**
    /// - mobileNumber: 10-digit mobile number without country code
    /// - countryCode: Country code with + prefix (default: +91)
    /// </remarks>
    public class OtpRequest
    {
        public string MobileNumber { get; set; } = string.Empty;
        public string CountryCode { get; set; } = "+91"; // Default to India
    }

    /// <summary>
    /// OTP verification request with device information
    /// </summary>
    /// <remarks>
    /// **Usage:** Verify OTP and authenticate user to receive JWT token
    /// **Required Fields:**
    /// - Mobile Number: Same 10-digit number used for sending OTP
    /// - Country Code: Same country code used for sending OTP
    /// - OTP: 6-digit numeric code received via SMS
    /// 
    /// **Optional Fields:**
    /// - FCM Token: For push notifications (Android/iOS)
    /// - Device ID: Unique device identifier for tracking
    /// - Device Type: "android", "ios", or "web"
    /// 
    /// **Examples:**
    /// ```json
    /// {
    ///   "mobileNumber": "9876543210",
    ///   "countryCode": "+91",
    ///   "otp": "123456",
    ///   "fcmToken": "firebase_fcm_token_here",
    ///   "deviceId": "unique_device_id",
    ///   "deviceType": "android"
    /// }
    /// ```
    /// 
    /// **Field Descriptions:**
    /// - mobileNumber: 10-digit mobile number (e.g., "9876543210")
    /// - countryCode: Country code with + prefix (e.g., "+91", "+1", "+44")
    /// - otp: 6-digit OTP code received via SMS (e.g., "123456")
    /// - fcmToken: Firebase Cloud Messaging token for push notifications
    /// - deviceId: Unique device identifier for tracking
    /// - deviceType: Device platform ("android", "ios", "web")
    /// </remarks>
    public class OtpVerificationRequest
    {
        public string MobileNumber { get; set; } = string.Empty;
        public string CountryCode { get; set; } = "+91"; // Default to India
        public string Otp { get; set; } = string.Empty;
        public string? FcmToken { get; set; } // Optional FCM token for push notifications
        public string? DeviceId { get; set; } // Optional device identifier
        public string? DeviceType { get; set; } // Optional device type (android, ios, web)
    }

    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public int? UserId { get; set; }
    }
}
