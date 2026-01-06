using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RankUpAPI.Areas.Admin.Models.Auth;
using RankUpAPI.Areas.Admin.Services.Interfaces;

namespace RankUpAPI.Areas.Admin.Services.Implementations
{
    public class AdminAuthService : IAdminAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly string _adminEmail;
        private readonly string _adminPassword;
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;

        public AdminAuthService(IConfiguration configuration)
        {
            _configuration = configuration;
            
            // In production, these should be stored in a secure way (e.g., Azure Key Vault)
            _adminEmail = _configuration["AdminCredentials:Email"] ?? "admin@rankup.com";
            _adminPassword = _configuration["AdminCredentials:Password"] ?? "Admin@123";
            
            var jwtSection = _configuration.GetSection("Jwt");
            _jwtSecret = jwtSection["Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
            _jwtIssuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured");
            _jwtAudience = jwtSection["Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured");
        }

        public async Task<AdminAuthResponse> LoginAsync(AdminLoginRequest request)
        {
            // In a real application, you would validate against a database
            if (request.Email != _adminEmail || request.Password != _adminPassword)
            {
                return new AdminAuthResponse
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            var token = GenerateJwtToken(request.Email);
            var refreshToken = GenerateRefreshToken();

            return new AdminAuthResponse
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                RefreshToken = refreshToken,
                Expiration = DateTime.UtcNow.AddMinutes(60) // Token expires in 60 minutes
            };
        }

        public Task<AdminAuthResponse> RefreshTokenAsync(string token, string refreshToken)
        {
            // Implement token refresh logic here
            // For simplicity, we'll just return a new token
            var newToken = GenerateJwtToken(_adminEmail);
            var newRefreshToken = GenerateRefreshToken();

            return Task.FromResult(new AdminAuthResponse
            {
                Success = true,
                Message = "Token refreshed successfully",
                Token = newToken,
                RefreshToken = newRefreshToken,
                Expiration = DateTime.UtcNow.AddMinutes(60)
            });
        }

        private string GenerateJwtToken(string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.Email, email)
            };

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60), // Token expires in 60 minutes
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
