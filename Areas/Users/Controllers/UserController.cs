using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RankUpAPI.Models;
using RankUpAPI.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;

namespace RankUpAPI.Areas.Users.Controllers
{
    [ApiController]
    [Route("api/User")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            _logger.LogInformation("=== GetProfile Started ===");
            
            try
            {
                // Log all request headers for debugging
                _logger.LogInformation("Request Headers:");
                foreach (var header in Request.Headers)
                {
                    _logger.LogInformation($"{header.Key}: {header.Value}");
                }

                // Check authentication status
                _logger.LogInformation($"User Authenticated: {User.Identity?.IsAuthenticated}");
                _logger.LogInformation($"Authentication Type: {User.Identity?.AuthenticationType}");

                // Get the token from the Authorization header
                var authHeader = Request.Headers["Authorization"].ToString();
                _logger.LogInformation($"Auth Header: {authHeader}");

                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    _logger.LogWarning("No Bearer token found in Authorization header");
                    return Unauthorized(new { 
                        Success = false, 
                        Message = "No authentication token provided",
                        Solution = "Make sure to include a valid JWT token in the Authorization header as: Bearer YOUR_TOKEN"
                    });
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();
                
                try
                {
                    var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                    _logger.LogInformation($"Token Valid: {jsonToken != null}");
                    if (jsonToken != null)
                    {
                        _logger.LogInformation($"Token Issuer: {jsonToken.Issuer}");
                        _logger.LogInformation($"Token Valid To: {jsonToken.ValidTo}");
                        _logger.LogInformation($"Token Claims:");
                        foreach (var claim in jsonToken.Claims)
                        {
                            _logger.LogInformation($"{claim.Type} = {claim.Value}");
                        }
                    }
                }
                catch (Exception tokenEx)
                {
                    _logger.LogError(tokenEx, "Error reading JWT token");
                    return Unauthorized(new { 
                        Success = false, 
                        Message = "Invalid token format",
                        Error = tokenEx.Message
                    });
                }

                // Get user ID from claims
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Fallback to 'sub' or 'nameid' if NameIdentifier is missing
                if (string.IsNullOrEmpty(userId))
                {
                    userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                             ?? User.FindFirst("nameid")?.Value;
                }

                _logger.LogInformation($"User ID from token: {userId ?? "NULL"}");

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("No NameIdentifier claim found in token");
                    return Unauthorized(new { 
                        Success = false, 
                        Message = "Invalid user identity in token",
                        Solution = "Make sure your token contains a valid NameIdentifier claim"
                    });
                }

                _logger.LogInformation("Fetching user from database...");
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

                if (user == null)
                {
                    _logger.LogWarning($"User with ID {userId} not found in database");
                    return NotFound(new { 
                        Success = false, 
                        Message = "User not found",
                        UserId = userId
                    });
                }

                _logger.LogInformation("User found, returning profile data");
                return Ok(new 
                { 
                    Success = true, 
                    Data = new 
                    {
                        user.Id,
                        user.Name,
                        user.Email,
                        user.PhoneNumber,
                        user.Gender,
                        user.ProfilePhoto,
                        user.PreferredExam,
                        user.IsPhoneVerified
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetProfile");
                return StatusCode(500, new { 
                    Success = false, 
                    Message = "An error occurred while processing your request",
                    Error = ex.Message,
                    StackTrace = ex.StackTrace
                });
            }
            finally
            {
                _logger.LogInformation("=== GetProfile Completed ===");
            }
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest model)
        {
            _logger.LogInformation("=== UpdateProfile Started ===");
            
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation($"User ID from token: {userId ?? "NULL"}");

                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int id))
                {
                    _logger.LogWarning("Invalid or missing user ID in token");
                    return Unauthorized(new { 
                        Success = false, 
                        Message = "Invalid user identity",
                        Solution = "Make sure you're logged in with a valid token"
                    });
                }

                _logger.LogInformation($"Looking for user with ID: {id}");
                var user = await _context.Users.FindAsync(id);
                
                if (user == null)
                {
                    _logger.LogWarning($"User with ID {id} not found in database");
                    return NotFound(new { 
                        Success = false, 
                        Message = "User not found",
                        UserId = id
                    });
                }

                // Log the update
                _logger.LogInformation($"Updating user {id}. Changes: " +
                    $"Name: {(!string.IsNullOrEmpty(model.Name) ? "Updating" : "No change")}, " +
                    $"Gender: {(!string.IsNullOrEmpty(model.Gender) ? "Updating" : "No change")}");

                // Update fields if provided
                if (!string.IsNullOrEmpty(model.Name)) 
                    user.Name = model.Name;
                if (!string.IsNullOrEmpty(model.Gender)) 
                    user.Gender = model.Gender;
                if (!string.IsNullOrEmpty(model.ProfilePhoto)) 
                    user.ProfilePhoto = model.ProfilePhoto;
                if (!string.IsNullOrEmpty(model.PreferredExam)) 
                    user.PreferredExam = model.PreferredExam;

                // Update email from the request if provided (do not auto-generate)
                if (!string.IsNullOrEmpty(model.Email))
                {
                    user.Email = model.Email;
                    _logger.LogInformation($"Updating email for user {id} to {model.Email}");
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Profile updated successfully");
                
                return Ok(new { 
                    Success = true, 
                    Message = "Profile updated",
                    UpdatedFields = new {
                        Name = !string.IsNullOrEmpty(model.Name),
                        Gender = !string.IsNullOrEmpty(model.Gender),
                        ProfilePhoto = !string.IsNullOrEmpty(model.ProfilePhoto),
                        PreferredExam = !string.IsNullOrEmpty(model.PreferredExam),
                        Email = !string.IsNullOrEmpty(model.Email)
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile");
                return StatusCode(500, new { 
                    Success = false, 
                    Message = "Error updating profile",
                    Error = ex.Message,
                    StackTrace = ex.StackTrace
                });
            }
            finally
            {
                _logger.LogInformation("=== UpdateProfile Completed ===");
            }
        }
    }

    public class UpdateProfileRequest
    {
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public string? ProfilePhoto { get; set; }
        public string? PreferredExam { get; set; }
        // Accept email in update request so GetProfile shows the updated email
        public string? Email { get; set; }
    }
}