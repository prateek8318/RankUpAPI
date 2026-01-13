using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RankUpAPI.Areas.Users.Auth.Models;
using RankUpAPI.Core.Services.Interfaces;
using RankUpAPI.Services;
using RankUpAPI.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace RankUpAPI.Areas.Users.Auth.Controllers
{
    [ApiController]
    [Area("Users")]
    [Route("api/[area]/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IOtpService _otpService;
        private readonly ILogger<AuthController> _logger;
        private readonly ApplicationDbContext _context;

        public AuthController(
            IConfiguration configuration,
            IUserService userService,
            IOtpService otpService,
            ILogger<AuthController> logger,
            ApplicationDbContext context)
        {
            _configuration = configuration;
            _userService = userService;
            _otpService = otpService;
            _logger = logger;
            _context = context;

            try
            {
                _logger.LogInformation("Checking database connection...");
                var canConnect = _context.Database.CanConnect();
                _logger.LogInformation($"Database connection status: {(canConnect ? "Connected successfully" : "Failed to connect")}");

                if (canConnect)
                {
                    _logger.LogInformation("Database provider: " + _context.Database.ProviderName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to database");
            }
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

            var otp = _otpService.GenerateOtp();
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
                _logger.LogInformation($"[VerifyOtp] Starting verification for: {request?.MobileNumber}");

                if (string.IsNullOrWhiteSpace(request?.MobileNumber) || string.IsNullOrWhiteSpace(request?.Otp))
                {
                    _logger.LogWarning("[VerifyOtp] Missing mobile number or OTP");
                    return BadRequest(new
                    {
                        success = false,
                        message = "Mobile number and OTP are required"
                    });
                }

                if (!_otpService.ValidateOtp(request.MobileNumber, request.Otp))
                {
                    _logger.LogWarning($"[VerifyOtp] Invalid OTP for {request.MobileNumber}");
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid or expired OTP"
                    });
                }

                var user = await _userService.GetOrCreateUserAsync(request.MobileNumber);
                if (user == null)
                {
                    _logger.LogError($"[VerifyOtp] Failed to create user: {request.MobileNumber}");
                    return StatusCode(500, new
                    {
                        success = false,
                        message = "Failed to process your request"
                    });
                }

                try
                {
                    user.LastLoginAt = DateTime.UtcNow;
                    user.IsPhoneVerified = true;
                    await _userService.UpdateUserLoginInfoAsync(user);
                    _otpService.RemoveOtp(request.MobileNumber);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[VerifyOtp] Error saving user info for: {user.Id}");
                    return StatusCode(500, new { success = false, message = "Failed to save verification state" });
                }

                var token = GenerateJwtToken(user.Id.ToString(), user.PhoneNumber);

                _logger.LogInformation($"[VerifyOtp] Success for user: {user.Id}");
                return Ok(new
                {
                    success = true,
                    message = "Login successful",
                    token,
                    userId = user.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[VerifyOtp] Error: {ex.Message}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred"
                });
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
