using AdminService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Retry;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using UserService.Application.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Collections.Generic;

namespace AdminService.Application.Clients
{
    public class UserServiceClient : IUserServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserServiceClient> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly IConfiguration _configuration;

        public UserServiceClient(HttpClient httpClient, ILogger<UserServiceClient> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
            
            _retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        _logger.LogWarning($"Retry {retryCount} after {timespan.TotalSeconds}s");
                    });
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync($"/api/users/{userId}"));

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserDto>();
                }

                _logger.LogWarning($"Failed to get user {userId}: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling UserService for user {userId}");
                return null;
            }
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync($"/api/users/by-email/{email}"));

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserDto>();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling UserService for email {email}");
                return null;
            }
        }

        public async Task<IEnumerable<UserDto>?> GetAllUsersAsync(int page = 1, int pageSize = 50)
        {
            try
            {
                // Add service-to-service authentication
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", GetServiceToken());
                
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync($"/api/admin/users?page={page}&pageSize={pageSize}"));

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<IEnumerable<UserDto>>();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling UserService for all users");
                return null;
            }
        }

        public async Task<UserDto?> UpdateUserAsync(int id, object updateDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(updateDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.PutAsync($"/api/admin/users/{id}", content));

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserDto>();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling UserService to update user {id}");
                return null;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.DeleteAsync($"/api/admin/users/{id}"));

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling UserService to delete user {id}");
                return false;
            }
        }

        public async Task<bool> EnableDisableUserAsync(int id, bool isActive)
        {
            try
            {
                var json = JsonSerializer.Serialize(isActive);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.PatchAsync($"/api/admin/users/{id}/enable-disable", content));

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling UserService to enable/disable user {id}");
                return false;
            }
        }

        public async Task<int> GetTotalUsersCountAsync()
        {
            try
            {
                // Add service-to-service authentication
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", GetServiceToken());
                
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync("/api/admin/users/count"));

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
                    return result?.GetValueOrDefault("totalUsers", 0) ?? 0;
                }

                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling UserService for total users count");
                return 0;
            }
        }

        public async Task<int> GetDailyActiveUsersCountAsync()
        {
            try
            {
                // Add service-to-service authentication
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", GetServiceToken());
                
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync("/api/admin/users/daily-active-count"));

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
                    return result?.GetValueOrDefault("dailyActiveUsers", 0) ?? 0;
                }

                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting daily active users count");
                return 0;
            }
        }

        private string GetServiceToken()
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, "admin-service"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Service"),
                new Claim("ServiceName", "AdminService")
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
    }
}
