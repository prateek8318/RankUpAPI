using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RankUpAPI.Models;
using RankUpAPI.Services;
using System.Collections.Concurrent;
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
        
        // In-memory storage for OTPs (in production, use a distributed cache like Redis)
        private static readonly ConcurrentDictionary<string, string> OtpStore = new();
        private const string DefaultOtp = "1234"; // Default OTP
        private const int OtpExpirationMinutes = 5; // OTP expiration time in minutes

        public AuthController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
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

          
            var otp = DefaultOtp; // In production, generate a random OTP
            
            // Store OTP with expiration
            var otpKey = $"otp_{request.MobileNumber}";
            OtpStore[otpKey] = otp;
            
            // In a real app, you would send the OTP via SMS here
            Console.WriteLine($"OTP for {request.MobileNumber}: {otp}");

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

            var otpKey = $"otp_{request.MobileNumber}";
            
            if (!OtpStore.TryGetValue(otpKey, out var storedOtp))
            {
                return BadRequest(new AuthResponse 
                { 
                    Success = false, 
                    Message = "OTP not found or expired. Please request a new OTP." 
                });
            }

            if (storedOtp != request.Otp)
            {
                return BadRequest(new AuthResponse 
                { 
                    Success = false, 
                    Message = "Invalid OTP" 
                });
            }

            // OTP is valid, remove it from store
            OtpStore.TryRemove(otpKey, out _);

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
    }
}
