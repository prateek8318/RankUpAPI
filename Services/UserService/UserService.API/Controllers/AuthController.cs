using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;

namespace UserService.API.Controllers
{
    /// <summary>
    /// Authentication Controller - Handles OTP-based login, user registration, and JWT token management
    /// </summary>
    /// <remarks>
    /// **Authentication Flow:**
    /// 1. Send OTP to user's mobile number
    /// 2. Verify OTP to get JWT token
    /// 3. Use JWT token for authenticated requests
    /// 
    /// **Supported Countries:** India (+91), USA (+1), UK (+44), and others
    /// **OTP Format:** 6-digit numeric code
    /// **Token Validity:** 60 minutes
    /// </remarks>
    [ApiController]
    [Route("api/users/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IOtpService _otpService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IConfiguration configuration,
            IUserService userService,
            IOtpService otpService,
            ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _userService = userService;
            _otpService = otpService;
            _logger = logger;
        }

        /// <summary>
        /// Send OTP to user's mobile number for authentication
        /// </summary>
        /// <param name="request">Mobile number and country code for OTP delivery</param>
        /// <returns>OTP delivery confirmation</returns>
        /// <remarks>
        /// **Request Details:**
        /// - Mobile Number: Must be exactly 10 digits (e.g., "9876543210")
        /// - Country Code: Must start with '+' followed by 1-3 digits (e.g., "+91", "+1", "+44")
        /// - Default Country Code: +91 (India) if not specified
        /// 
        /// **OTP Settings:**
        /// - Development: Uses default OTP "123456"
        /// - Production: Generates random 6-digit OTP
        /// - OTP Validity: 5 minutes
        /// 
        /// **Example Request:**
        /// {
        ///   "mobileNumber": "9876543210",
        ///   "countryCode": "+91"
        /// }
        /// 
        /// **Example Response:**
        /// {
        ///   "success": true,
        ///   "message": "OTP sent to +919876543210",
        ///   "data": null
        /// }
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [AllowAnonymous]
        [HttpPost("send-otp")]
        public IActionResult SendOtp([FromBody] OtpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.MobileNumber) || !IsValidMobileNumber(request.MobileNumber))
            {
                return BadRequest(ApiResponse.CreateBadRequest(
                    "Please provide a valid 10-digit mobile number",
                    ErrorCodes.INVALID_MOBILE_NUMBER));
            }

            // Validate country code
            if (string.IsNullOrWhiteSpace(request.CountryCode) || !IsValidCountryCode(request.CountryCode))
            {
                return BadRequest(ApiResponse.CreateBadRequest(
                    "Please provide a valid country code (e.g., +91, +1, +44)",
                    ErrorCodes.INVALID_COUNTRY_CODE));
            }

            // Use default OTP from configuration
            var useRandomOtp = _configuration.GetValue<bool>("OtpSettings:UseRandomOtp", false);
            var defaultOtp = _configuration.GetValue<string>("OtpSettings:DefaultOtp", "123456");
            var otp = useRandomOtp ? _otpService.GenerateOtp() : defaultOtp;
            
            // Store OTP with full phone number (country code + mobile number)
            var fullPhoneNumber = $"{request.CountryCode}{request.MobileNumber}";
            _otpService.StoreOtp(fullPhoneNumber, otp);
            _logger.LogInformation($"OTP for {fullPhoneNumber}: {otp}");

            return Ok(new AuthResponse
            {
                Success = true,
                Message = $"OTP sent to {fullPhoneNumber}"
            });
        }

        /// <summary>
        /// Verify OTP and authenticate user to receive JWT token
        /// </summary>
        /// <param name="request">OTP verification details with optional device information</param>
        /// <returns>JWT token and user profile information</returns>
        /// <remarks>
        /// **Request Details:**
        /// - Mobile Number: Same 10-digit number used for sending OTP
        /// - Country Code: Same country code used for sending OTP
        /// - OTP: 6-digit numeric code received via SMS
        /// - FCM Token: Optional - for push notifications (Android/iOS)
        /// - Device ID: Optional - unique device identifier
        /// - Device Type: Optional - "android", "ios", or "web"
        /// 
        /// **Response Includes:**
        /// - JWT Token: For authenticated API calls
        /// - User ID: Unique user identifier
        /// - User Name: Current user name (or default if new user)
        /// - Is New User: true if first-time login
        /// - Is Profile Complete: true if user has set a custom name
        /// - Phone Verified: true if phone number is verified
        /// 
        /// **Example Request:**
        /// {
        ///   "mobileNumber": "9876543210",
        ///   "countryCode": "+91",
        ///   "otp": "123456",
        ///   "fcmToken": "firebase_fcm_token",
        ///   "deviceId": "unique_device_id",
        ///   "deviceType": "android"
        /// }
        /// 
        /// **Example Response:**
        /// {
        ///   "success": true,
        ///   "message": "Login successful! Welcome back.",
        ///   "data": {
        ///     "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        ///     "userId": 123,
        ///     "userName": "John Doe",
        ///     "isNewUser": false,
        ///     "isProfileComplete": true,
        ///     "phoneNumber": "+919876543210",
        ///     "isPhoneVerified": true
        ///   }
        /// }
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse))]
        [AllowAnonymous]
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerificationRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.MobileNumber) || string.IsNullOrWhiteSpace(request?.Otp))
                {
                    return BadRequest(ApiResponse.CreateBadRequest(
                        "Mobile number and OTP are required for verification",
                        ErrorCodes.MISSING_REQUIRED_FIELDS));
                }

                // Validate country code
                if (string.IsNullOrWhiteSpace(request.CountryCode) || !IsValidCountryCode(request.CountryCode))
                {
                    return BadRequest(ApiResponse.CreateBadRequest(
                        "Please provide a valid country code (e.g., +91, +1, +44)",
                        ErrorCodes.INVALID_COUNTRY_CODE));
                }

                // Use full phone number for OTP validation
                var fullPhoneNumber = $"{request.CountryCode}{request.MobileNumber}";

                if (!_otpService.ValidateOtp(fullPhoneNumber, request.Otp))
                {
                    return BadRequest(ApiResponse.CreateBadRequest(
                        "The OTP you entered is invalid or has expired. Please try again.",
                        ErrorCodes.INVALID_OTP));
                }

                var userDto = await _userService.GetOrCreateUserAsync(request.MobileNumber, request.CountryCode);
                if (userDto == null)
                {
                    return StatusCode(500, ApiResponse.CreateInternalServerError(
                        "Unable to process your login request. Please try again later.",
                        ErrorCodes.DATABASE_ERROR));
                }

                await _userService.UpdateUserLoginInfoAsync(userDto.Id);
                _otpService.RemoveOtp(fullPhoneNumber);

                var token = GenerateJwtToken(userDto.Id.ToString(), userDto.PhoneNumber);

                // Create response object with device information if provided
                var responseData = new Dictionary<string, object>
                {
                    ["token"] = token,
                    ["userId"] = userDto.Id,
                    ["userName"] = userDto.Name,
                    ["isNewUser"] = userDto.IsNewUser,
                    ["isProfileComplete"] = !string.IsNullOrWhiteSpace(userDto.Name) && 
                                           userDto.Name != $"User{userDto.PhoneNumber}",
                    ["phoneNumber"] = fullPhoneNumber,
                    ["isPhoneVerified"] = userDto.IsPhoneVerified
                };

                // Add device information to response if provided in request
                if (!string.IsNullOrWhiteSpace(request.FcmToken))
                    responseData["fcmToken"] = request.FcmToken;
                if (!string.IsNullOrWhiteSpace(request.DeviceId))
                    responseData["deviceId"] = request.DeviceId;
                if (!string.IsNullOrWhiteSpace(request.DeviceType))
                    responseData["deviceType"] = request.DeviceType;

                return Ok(ApiResponse.CreateSuccess(
                    "Login successful! Welcome back.",
                    responseData));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in VerifyOtp: {ex.Message}");
                return StatusCode(500, ApiResponse.CreateInternalServerError(
                    "An unexpected error occurred during login. Please try again.",
                    ErrorCodes.INTERNAL_SERVER_ERROR));
            }
        }

        
        [HttpPut("profile/{userId}")]
        [Authorize]
        public async Task<ActionResult<UserDto>> UpdateProfile(int userId, [FromBody] ProfileUpdateRequest request)
        {
            try
            {
                var user = await _userService.UpdateUserProfileAsync(userId, request);
                return Ok(ApiResponse.CreateSuccess(
                    "User profile updated successfully",
                    user));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse.CreateNotFound(
                    $"User profile with ID {userId} was not found for update",
                    ErrorCodes.USER_NOT_FOUND));
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("IX_Users_Email") == true 
                                               || ex.InnerException?.Message.Contains("Email") == true)
            {
                _logger.LogWarning(ex, "Duplicate email detected while updating user profile for ID {UserId}", userId);
                return BadRequest(ApiResponse.CreateBadRequest(
                    "This email address is already registered with another account. Please use a different email.",
                    ErrorCodes.DUPLICATE_EMAIL));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user profile for ID {userId}: {ex.Message}");
                return StatusCode(500, ApiResponse.CreateInternalServerError(
                    "Unable to update user profile. Please try again later.",
                    ErrorCodes.DATABASE_ERROR));
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            try
            {
                // In a stateless JWT implementation, we don't need to do anything server-side
                // The client should simply discard the token
                // For enhanced security, we could implement token blacklisting here if needed
                
                return Ok(ApiResponse.CreateSuccess(
                    "Logout successful. Please discard the token from client side."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in Logout: {ex.Message}");
                return StatusCode(500, ApiResponse.CreateInternalServerError(
                    "An error occurred during logout. Please try again.",
                    ErrorCodes.INTERNAL_SERVER_ERROR));
            }
        }

        private static bool IsValidMobileNumber(string mobileNumber)
        {
            return !string.IsNullOrWhiteSpace(mobileNumber) &&
                   Regex.IsMatch(mobileNumber, "^[0-9]{10}$");
        }

        private static bool IsValidCountryCode(string countryCode)
        {
            return !string.IsNullOrWhiteSpace(countryCode) &&
                   Regex.IsMatch(countryCode, @"^\+[0-9]{1,3}$");
        }

        private string GenerateJwtToken(string userId, string mobileNumber)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);
            var tokenExpiryInMinutes = int.Parse(jwtSettings["ExpireInMinutes"] ?? "60");

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(JwtRegisteredClaimNames.Sub, userId),
                    new Claim(ClaimTypes.MobilePhone, mobileNumber),
                    new Claim(ClaimTypes.Role, "User"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(tokenExpiryInMinutes),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
