using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Domain.Entities;
using UserService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace UserService.Application.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiryMinutes;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            _secretKey = _configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key not configured");
            _issuer = _configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer not configured");
            _audience = _configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience not configured");
            _expiryMinutes = int.Parse(_configuration["Jwt:ExpireInMinutes"] ?? "60");
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? ""),
                new Claim("is_phone_verified", user.IsPhoneVerified.ToString()),
                new Claim("is_active", user.IsActive.ToString()),
                new Claim(ClaimTypes.Role, "User"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_expiryMinutes),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = await Task.Run(() => tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken));
                return principal != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> RefreshTokenAsync(string refreshToken)
        {
            // This is a placeholder implementation
            // In a real scenario, you would validate the refresh token from a database
            // and generate a new JWT token
            await Task.Delay(1); // Placeholder async operation
            throw new NotImplementedException("Refresh token functionality not implemented yet");
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            // This is a placeholder implementation
            // In a real scenario, you would add the token to a blacklist or revoke it in the database
            await Task.Delay(1); // Placeholder async operation
            throw new NotImplementedException("Token revocation functionality not implemented yet");
        }
    }
}
