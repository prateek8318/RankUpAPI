using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [AllowAnonymous]
        [HttpPost("send-otp")]
        public IActionResult SendOtp([FromBody] OtpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.MobileNumber) || !IsValidMobileNumber(request.MobileNumber))
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Please provide a valid 10-digit mobile number"
                });
            }

            // Use default OTP from configuration
            var useRandomOtp = _configuration.GetValue<bool>("OtpSettings:UseRandomOtp", false);
            var defaultOtp = _configuration.GetValue<string>("OtpSettings:DefaultOtp", "1234");
            var otp = useRandomOtp ? _otpService.GenerateOtp() : defaultOtp;
            
            _otpService.StoreOtp(request.MobileNumber, otp);
            _logger.LogInformation($"OTP for {request.MobileNumber}: {otp}");

            return Ok(new AuthResponse
            {
                Success = true,
                Message = $"OTP sent to {request.MobileNumber}"
            });
        }

        [AllowAnonymous]
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerificationRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.MobileNumber) || string.IsNullOrWhiteSpace(request?.Otp))
                {
                    return BadRequest(new { success = false, message = "Mobile number and OTP are required" });
                }

                if (!_otpService.ValidateOtp(request.MobileNumber, request.Otp))
                {
                    return BadRequest(new { success = false, message = "Invalid or expired OTP" });
                }

                var userDto = await _userService.GetOrCreateUserAsync(request.MobileNumber);
                if (userDto == null)
                {
                    return StatusCode(500, new { success = false, message = "Failed to process your request" });
                }

                await _userService.UpdateUserLoginInfoAsync(userDto.Id);
                _otpService.RemoveOtp(request.MobileNumber);

                var token = GenerateJwtToken(userDto.Id.ToString(), userDto.PhoneNumber);

                return Ok(new
                {
                    success = true,
                    message = "Login successful",
                    token,
                    userId = userDto.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in VerifyOtp: {ex.Message}");
                return StatusCode(500, new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet("profile/{userId}")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetProfile(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPut("profile/{userId}")]
        [Authorize]
        public async Task<ActionResult<UserDto>> UpdateProfile(int userId, [FromBody] ProfileUpdateRequest request)
        {
            try
            {
                var user = await _userService.UpdateUserProfileAsync(userId, request);
                return Ok(user);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        private static bool IsValidMobileNumber(string mobileNumber)
        {
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
                    new Claim(JwtRegisteredClaimNames.Sub, userId),
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
    }
}
