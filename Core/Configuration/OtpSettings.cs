namespace RankUpAPI.Core.Configuration
{
    /// <summary>
    /// OTP Configuration Settings
    /// This can be changed in appsettings.json without code changes
    /// </summary>
    public class OtpSettings
    {
        public const string SectionName = "OtpSettings";
        
        /// <summary>
        /// Default OTP value for development/testing
        /// Can be changed in appsettings.json
        /// </summary>
        public string DefaultOtp { get; set; } = "1234";
        
        /// <summary>
        /// OTP expiration time in minutes
        /// </summary>
        public int ExpirationMinutes { get; set; } = 5;
        
        /// <summary>
        /// Enable random OTP generation (if false, uses DefaultOtp)
        /// </summary>
        public bool UseRandomOtp { get; set; } = false;
        
        /// <summary>
        /// OTP length when generating random OTP
        /// </summary>
        public int OtpLength { get; set; } = 4;
    }
}
