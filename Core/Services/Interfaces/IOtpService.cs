namespace RankUpAPI.Core.Services.Interfaces
{
    /// <summary>
    /// Service for OTP generation and validation
    /// </summary>
    public interface IOtpService
    {
        /// <summary>
        /// Generates an OTP based on configuration
        /// </summary>
        string GenerateOtp();
        
        /// <summary>
        /// Stores OTP for a mobile number
        /// </summary>
        void StoreOtp(string mobileNumber, string otp);
        
        /// <summary>
        /// Validates OTP for a mobile number
        /// </summary>
        bool ValidateOtp(string mobileNumber, string otp);
        
        /// <summary>
        /// Removes OTP after successful validation
        /// </summary>
        void RemoveOtp(string mobileNumber);
        
        /// <summary>
        /// Gets stored OTP for a mobile number
        /// </summary>
        string? GetStoredOtp(string mobileNumber);
    }
}
