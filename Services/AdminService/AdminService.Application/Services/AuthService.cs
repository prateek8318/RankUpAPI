using AdminService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AdminService.Application.Services
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserServiceClient _userServiceClient;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IConfiguration configuration,
            IUserServiceClient userServiceClient,
            ILogger<AuthService> logger)
        {
            _configuration = configuration;
            _userServiceClient = userServiceClient;
            _logger = logger;
        }

        // User token validate करने के लिए
        public async Task<bool> ValidateUserTokenAsync(string token)
        {
            try
            {
                // UserService से user data validate करें
                var user = await _userServiceClient.GetUserByTokenAsync(token);
                return user != null && user.IsActive;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating user token");
                return false;
            }
        }

        // Service-to-service token generate करने के लिए
        public string GenerateServiceToken(string serviceName)
        {
            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("service_name", serviceName),
                    new Claim("service_type", "internal"),
                    new Claim("role", "service")
                }),
                Expires = DateTime.UtcNow.AddHours(24), // 24 hours valid
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // Internal service token validate करने के लिए
        public bool ValidateServiceToken(string token, string expectedService)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var serviceName = jwtToken.Claims.FirstOrDefault(x => x.Type == "service_name")?.Value;

                return serviceName == expectedService;
            }
            catch
            {
                return false;
            }
        }

        // User role check करने के लिए
        public async Task<bool> IsUserAdminAsync(int userId)
        {
            try
            {
                var user = await _userServiceClient.GetUserByIdAsync(userId);
                return user?.Role == "Admin" || user?.Role == "SuperAdmin";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking user admin role");
                return false;
            }
        }
    }
}
