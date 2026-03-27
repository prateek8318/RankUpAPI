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
    [ApiController]
    [Route("api/users/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IOtpService _otpService;
        private readonly IDeviceInfoService _deviceInfoService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IConfiguration configuration,
            IUserService userService,
            IOtpService otpService,
            IDeviceInfoService deviceInfoService,
            ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _userService = userService;
            _otpService = otpService;
            _deviceInfoService = deviceInfoService;
            _logger = logger;
        }

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

            if (string.IsNullOrWhiteSpace(request.CountryCode) || !IsValidCountryCode(request.CountryCode))
            {
                return BadRequest(ApiResponse.CreateBadRequest(
                    "Please provide a valid country code (e.g., +91, +1, +44)",
                    ErrorCodes.INVALID_COUNTRY_CODE));
            }

            var useRandomOtp = _configuration.GetValue<bool>("OtpSettings:UseRandomOtp", false);
            var defaultOtp = _configuration.GetValue<string>("OtpSettings:DefaultOtp", "123456");
            var otp = useRandomOtp ? _otpService.GenerateOtp() : defaultOtp;
            
            var fullPhoneNumber = $"{request.CountryCode}{request.MobileNumber}";
            _otpService.StoreOtp(fullPhoneNumber, otp);
            _logger.LogInformation($"OTP for {fullPhoneNumber}: {otp}");

            return Ok(new AuthResponse
            {
                Success = true,
                Message = $"OTP sent to {fullPhoneNumber}"
            });
        }

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

                if (string.IsNullOrWhiteSpace(request.CountryCode) || !IsValidCountryCode(request.CountryCode))
                {
                    return BadRequest(ApiResponse.CreateBadRequest(
                        "Please provide a valid country code (e.g., +91, +1, +44)",
                        ErrorCodes.INVALID_COUNTRY_CODE));
                }

                var fullPhoneNumber = $"{request.CountryCode}{request.MobileNumber}";

                if (!_otpService.ValidateOtp(fullPhoneNumber, request.Otp))
                {
                    return BadRequest(ApiResponse.CreateBadRequest(
                        "The OTP you entered is invalid or has expired. Please try again.",
                        ErrorCodes.INVALID_OTP));
                }

                var userDto = await _userService.GetOrCreateUserAsync(request.MobileNumber, request.CountryCode, markPhoneVerified: true);
                if (userDto == null)
                {
                    return StatusCode(500, ApiResponse.CreateInternalServerError(
                        "Unable to process your login request. Please try again later.",
                        ErrorCodes.DATABASE_ERROR));
                }

                var isNewUser = userDto.IsNewUser;

                await _userService.UpdateUserLoginInfoAsync(userDto.Id);
                
                // Update device information using the new device info service
                if (!string.IsNullOrWhiteSpace(request.DeviceId) || 
                    !string.IsNullOrWhiteSpace(request.DeviceType) || 
                    !string.IsNullOrWhiteSpace(request.DeviceName) ||
                    !string.IsNullOrWhiteSpace(request.FcmToken))
                {
                    await _deviceInfoService.UpdateDeviceInfoFromOtpAsync(
                        userDto.Id, 
                        request.DeviceId, 
                        request.DeviceType, 
                        request.DeviceName, 
                        request.FcmToken);
                }

                // Refresh user data after login/device updates so profile fields stay in sync.
                userDto = await _userService.GetUserByIdAsync(userDto.Id);
                if (userDto == null)
                {
                    return StatusCode(500, ApiResponse.CreateInternalServerError(
                        "Unable to retrieve updated user data.",
                        ErrorCodes.DATABASE_ERROR));
                }
                userDto.IsNewUser = isNewUser;
                
                _otpService.RemoveOtp(fullPhoneNumber);

                var token = GenerateJwtToken(userDto.Id.ToString(), userDto.PhoneNumber);

                var responseData = new Dictionary<string, object>
                {
                    ["token"] = token,
                    ["userId"] = userDto.Id,
                    ["isNewUser"] = isNewUser,
                    ["isProfileComplete"] = !userDto.IsNewUser && 
                                           !string.IsNullOrWhiteSpace(userDto.Name) && 
                                           userDto.Name != "User" &&
                                           !string.IsNullOrWhiteSpace(userDto.Email) &&
                                           !string.IsNullOrWhiteSpace(userDto.Gender) &&
                                           userDto.DateOfBirth.HasValue &&
                                           userDto.StateId.HasValue &&
                                           userDto.LanguageId.HasValue &&
                                           userDto.QualificationId.HasValue &&
                                           userDto.CategoryId.HasValue &&
                                           userDto.StreamId.HasValue &&
                                           !string.IsNullOrWhiteSpace(userDto.ProfilePhoto) &&
                                           userDto.ExamId.HasValue &&
                                           userDto.InterestedInIntlExam,
                    ["phoneNumber"] = fullPhoneNumber,
                    ["isPhoneVerified"] = true,  // OTP verification means phone is verified
                    ["user"] = userDto  // Include user data with last login time
                };

                if (!string.IsNullOrWhiteSpace(request.FcmToken))
                    responseData["fcmToken"] = request.FcmToken;
                if (!string.IsNullOrWhiteSpace(request.DeviceId))
                    responseData["deviceId"] = request.DeviceId;
                if (!string.IsNullOrWhiteSpace(request.DeviceType))
                    responseData["deviceType"] = request.DeviceType;
                if (!string.IsNullOrWhiteSpace(request.DeviceName))
                    responseData["deviceName"] = request.DeviceName;

                // Add device info stored confirmation
                if (!string.IsNullOrWhiteSpace(request.DeviceId) || 
                    !string.IsNullOrWhiteSpace(request.DeviceType) || 
                    !string.IsNullOrWhiteSpace(request.DeviceName) ||
                    !string.IsNullOrWhiteSpace(request.FcmToken))
                {
                    responseData["deviceInfoStored"] = true;
                }

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
