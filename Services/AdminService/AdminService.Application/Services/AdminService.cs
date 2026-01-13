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
        }

        public async Task<AdminAuthResponse> LoginAsync(AdminLoginRequest request)
        {
            // Get user from UserService
            var user = await _userServiceClient.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return new AdminAuthResponse
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            // Get admin by userId
            var admin = await _adminRepository.GetByUserIdAsync(user.Id);
            if (admin == null)
            {
                return new AdminAuthResponse
                {
                    Success = false,
                    Message = "Admin account not found"
                };
            }

            // Simple password check (in production, use proper password hashing)
            // For now, check against config
            if (request.Email != _adminEmail || request.Password != _adminPassword)
            {
                return new AdminAuthResponse
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            // Get admin with roles and permissions
            var adminWithRoles = await _adminRepository.GetByIdWithRolesAsync(admin.Id);
            if (adminWithRoles == null)
            {
                return new AdminAuthResponse
                {
                    Success = false,
                    Message = "Admin account not found"
                };
            }

            var token = GenerateJwtToken(adminWithRoles, request.Email);
            var refreshToken = GenerateRefreshToken();

            // Create session
            var session = new AdminSession
            {
                AdminId = admin.Id,
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            admin.AdminSessions.Add(session);
            admin.LastLoginAt = DateTime.UtcNow;
            await _adminRepository.UpdateAsync(admin);
            await _adminRepository.SaveChangesAsync();

            // Get permissions
            var permissions = adminWithRoles.AdminRoles
                .SelectMany(ar => ar.Role.RolePermissions)
                .Select(rp => rp.Permission.Name)
                .Distinct()
                .ToList();

            var adminDto = new AdminDto
            {
                Id = admin.Id,
                UserId = admin.UserId,
                Role = admin.Role,
                Permissions = permissions,
                IsTwoFactorEnabled = admin.IsTwoFactorEnabled,
                LastLoginAt = admin.LastLoginAt,
                IsActive = admin.IsActive
            };

            await LogActivityAsync(admin.Id, "Login", "Auth", null, "Admin logged in");

            return new AdminAuthResponse
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                RefreshToken = refreshToken,
                Admin = adminDto
            };
        }

        public async Task<AdminDto?> GetAdminByIdAsync(int id)
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

        public async Task<IEnumerable<AdminDto>> GetAllAdminsAsync()
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

        public async Task LogActivityAsync(int adminId, string action, string? resource = null, int? resourceId = null, string? details = null)
        {
            var admin = await _adminRepository.GetByIdAsync(adminId);
            if (admin == null)
                return;

            var log = new AdminActivityLog
            {
                AdminId = adminId,
                Action = action,
                Resource = resource,
                ResourceId = resourceId,
                Details = details,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // Note: In a real implementation, you'd have an IAdminActivityLogRepository
            // For now, this is a placeholder
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
