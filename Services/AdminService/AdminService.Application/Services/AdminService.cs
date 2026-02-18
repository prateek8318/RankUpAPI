using AdminService.Application.DTOs;
using AdminService.Application.Interfaces;
using AdminService.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AdminService.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IUserServiceClient _userServiceClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdminService> _logger;
        private readonly string _adminEmail;
        private readonly string _adminPassword;
        private readonly string _adminMobileNumber;
        private readonly string _fixedOtp = "123456";

        public AdminService(
            IAdminRepository adminRepository,
            IUserServiceClient userServiceClient,
            IConfiguration configuration,
            ILogger<AdminService> logger)
        {
            _adminRepository = adminRepository;
            _userServiceClient = userServiceClient;
            _configuration = configuration;
            _logger = logger;
            _adminEmail = _configuration["AdminCredentials:Email"] ?? "admin@rankup.com";
            _adminPassword = _configuration["AdminCredentials:Password"] ?? "Admin@123";
            _adminMobileNumber = _configuration["AdminCredentials:MobileNumber"] ?? "+919876543210";
        }

        public async Task<AdminLoginResponse> LoginAsync(AdminLoginRequest request)
        {
            // Simple password check (in production, use proper password hashing)
            if (request.Email != _adminEmail || request.Password != _adminPassword)
            {
                return new AdminLoginResponse
                {
                    Success = false,
                    Message = "Invalid email or password",
                    RequiresTwoFactor = false
                };
            }

            // Always require 2-step verification
            if (!string.IsNullOrEmpty(_adminMobileNumber))
            {
                // In a real implementation, send OTP to mobile number here
                _logger.LogInformation($"OTP {_fixedOtp} sent to {_adminMobileNumber}");
                
                return new AdminLoginResponse
                {
                    Success = true,
                    Message = "Password verified. Please enter OTP sent to your mobile.",
                    RequiresTwoFactor = true,
                    MobileNumber = MaskMobileNumber(_adminMobileNumber)
                };
            }

            // If no mobile number configured, return error
            return new AdminLoginResponse
            {
                Success = false,
                Message = "Mobile number not configured for admin. 2-step verification required.",
                RequiresTwoFactor = false
            };
        }

        public async Task<AdminAuthResponse> VerifyOtpAsync(AdminOtpVerificationRequest request)
        {
            if (request.Email != _adminEmail)
            {
                return new AdminAuthResponse
                {
                    Success = false,
                    Message = "Invalid email"
                };
            }

            if (request.Otp != _fixedOtp)
            {
                return new AdminAuthResponse
                {
                    Success = false,
                    Message = "Invalid OTP"
                };
            }

            return await CompleteLoginAsync();
        }

        private async Task<AdminAuthResponse> CompleteLoginAsync()
        {
            // Create a simple admin response for testing
            var adminDto = new AdminDto
            {
                Id = 1,
                UserId = 1,
                Role = "SuperAdmin",
                Permissions = new List<string> { "all" },
                IsTwoFactorEnabled = !string.IsNullOrEmpty(_adminMobileNumber),
                LastLoginAt = DateTime.UtcNow,
                IsActive = true
            };

            var token = GenerateJwtTokenForTesting(_adminEmail);
            var refreshToken = GenerateRefreshToken();

            await LogActivityAsync(1, "Login", "Auth", null, "Admin logged in");

            return new AdminAuthResponse
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                RefreshToken = refreshToken,
                Admin = adminDto
            };
        }

        private string MaskMobileNumber(string mobileNumber)
        {
            if (string.IsNullOrEmpty(mobileNumber) || mobileNumber.Length < 4)
                return mobileNumber;
            
            return mobileNumber.Substring(0, 3) + "*****" + mobileNumber.Substring(mobileNumber.Length - 2);
        }

        public async Task<AdminDto?> GetAdminByIdAsync(int id)
        {
            try
            {
                var admin = await _adminRepository.GetByIdWithRolesAsync(id);
                if (admin == null)
                    return null;

                var permissions = admin.AdminRoles
                    .SelectMany(ar => ar.Role.RolePermissions)
                    .Select(rp => rp.Permission.Name)
                    .Distinct()
                    .ToList();

                return new AdminDto
                {
                    Id = admin.Id,
                    UserId = admin.UserId,
                    Role = admin.Role,
                    Permissions = permissions,
                    IsTwoFactorEnabled = admin.IsTwoFactorEnabled,
                    LastLoginAt = admin.LastLoginAt,
                    IsActive = admin.IsActive
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin by ID: {AdminId}", id);
                return null;
            }
        }

        public async Task<IEnumerable<AdminDto>> GetAllAdminsAsync()
        {
            try
            {
                var admins = await _adminRepository.GetAllAsync();
                var adminDtos = new List<AdminDto>();

                foreach (var admin in admins)
                {
                    var adminWithRoles = await _adminRepository.GetByIdWithRolesAsync(admin.Id);
                    if (adminWithRoles != null)
                    {
                        var permissions = adminWithRoles.AdminRoles
                            .SelectMany(ar => ar.Role.RolePermissions)
                            .Select(rp => rp.Permission.Name)
                            .Distinct()
                            .ToList();

                        adminDtos.Add(new AdminDto
                        {
                            Id = admin.Id,
                            UserId = admin.UserId,
                            Role = admin.Role,
                            Permissions = permissions,
                            IsTwoFactorEnabled = admin.IsTwoFactorEnabled,
                            LastLoginAt = admin.LastLoginAt,
                            IsActive = admin.IsActive
                        });
                    }
                }

                return adminDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all admins");
                return new List<AdminDto>();
            }
        }

        public async Task LogActivityAsync(int adminId, string action, string? resource = null, int? resourceId = null, string? details = null)
        {
            // Simplified logging - just log to console for now
            _logger.LogInformation($"Admin {adminId}: {action} - {resource} {resourceId} - {details}");
            await Task.CompletedTask;
        }

        private string GenerateJwtTokenForTesting(string email)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.Email, email),
                new Claim("AdminId", "1")
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateJwtToken(Admin admin, string email)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.Email, email),
                new Claim("AdminId", admin.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
