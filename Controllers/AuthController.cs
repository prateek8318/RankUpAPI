using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RankUpAPI.Models;
using RankUpAPI.Services;
using RankUpAPI.Core.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace RankUpAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpPost("send-otp")]
        public IActionResult SendOtp([FromBody] OtpRequest request)
        {
            // Basic mobile number validation
            if (string.IsNullOrWhiteSpace(request.MobileNumber) || !IsValidMobileNumber(request.MobileNumber))
            {
                return BadRequest(new AuthResponse 
                { 
                    Success = false, 
                    Message = "Please provide a valid 10-digit mobile number" 
                });
            }

            // Generate OTP using service (reads from configuration)
            var otp = _otpService.GenerateOtp();
            
            // Store OTP with expiration
            _otpService.StoreOtp(request.MobileNumber, otp);
            
            // In a real app, you would send the OTP via SMS here
            _logger.LogInformation($"OTP for {request.MobileNumber}: {otp}");

            return Ok(new AuthResponse 
            { 
                Success = true, 
                Message = $"OTP sent to {request.MobileNumber}" 
            });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerificationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.MobileNumber) || string.IsNullOrWhiteSpace(request.Otp))
            {
                return BadRequest(new AuthResponse 
                { 
                    Success = false, 
                    Message = "Mobile number and OTP are required" 
                });
            }

            // Validate OTP using service
            if (!_otpService.ValidateOtp(request.MobileNumber, request.Otp))
            {
                return BadRequest(new AuthResponse 
                { 
                    Success = false, 
                    Message = "Invalid or expired OTP. Please request a new OTP." 
                });
            }

            // OTP is valid, remove it from store
            _otpService.RemoveOtp(request.MobileNumber);

            // Get or create user in database
            var user = await _userService.GetOrCreateUserAsync(request.MobileNumber);
            
            // Update user's last login time
            await _userService.UpdateUserLoginInfoAsync(user);

            // Generate JWT token
            var token = GenerateJwtToken(user.Id.ToString(), user.PhoneNumber);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                UserId = user.Id,
                PhoneNumber = user.PhoneNumber,
                Name = user.Name
            });
        }

        private static bool IsValidMobileNumber(string mobileNumber)
        {
            // Simple validation for 10-digit mobile number
            return !string.IsNullOrWhiteSpace(mobileNumber) && 
                   Regex.IsMatch(mobileNumber, "^[0-9]{10}$");
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
                    new Claim(ClaimTypes.MobilePhone, mobileNumber),
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
        
        [Authorize]
        [HttpGet("validate-token")]
        public IActionResult ValidateToken()
        {
            // If we get here, the token is valid
            var mobileNumber = User.FindFirst(ClaimTypes.MobilePhone)?.Value;
            return Ok(new AuthResponse 
            { 
                Success = true, 
                Message = $"Token is valid for user: {mobileNumber}" 
            });
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdateRequest profileUpdate)
        {
            try
            {
                // Get the user ID from the token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new AuthResponse 
                    { 
                        Success = false, 
                        Message = "Invalid user token" 
                    });
                }

                // Update the user profile
                var updatedUser = await _userService.UpdateUserProfileAsync(userId, profileUpdate);

                return Ok(new 
                { 
                    Success = true, 
                    Message = "Profile updated successfully",
                    User = new 
                    {
                        updatedUser.Id,
                        updatedUser.Name,
                        updatedUser.Email,
                        updatedUser.Gender,
                        DateOfBirth = updatedUser.DateOfBirth?.ToString("yyyy-MM-dd"),
                        updatedUser.Qualification,
                        updatedUser.LanguagePreference,
                        updatedUser.PreferredExam
                    }
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new AuthResponse 
                { 
                    Success = false, 
                    Message = ex.Message 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile");
                return StatusCode(500, new AuthResponse 
                { 
                    Success = false, 
                    Message = "An error occurred while updating the profile" 
                });
            }
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                // Get the user ID from the token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new AuthResponse 
                    { 
                        Success = false, 
                        Message = "Invalid user token" 
                    });
                }

                // Get the user profile
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new AuthResponse 
                    { 
                        Success = false, 
                        Message = "User not found" 
                    });
                }

                return Ok(new 
                { 
                    Success = true, 
                    Data = new 
                    {
                        user.Id,
                        user.Name,
                        user.Email,
                        user.Gender,
                        DateOfBirth = user.DateOfBirth?.ToString("yyyy-MM-dd"),
                        user.Qualification,
                        user.LanguagePreference,
                        user.PreferredExam,
                        user.PhoneNumber
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting profile");
                return StatusCode(500, new AuthResponse 
                { 
                    Success = false, 
                    Message = "An error occurred while retrieving the profile" 
                });
            }
        }
    }
}
