using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RankUpAPI.Areas.Admin.Models.Auth;
using RankUpAPI.Areas.Admin.Services.Interfaces;

namespace RankUpAPI.Areas.Admin.Controllers
{
    [ApiController]
    [Area("Admin")]
    [Route("api/admin/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAdminAuthService _adminAuthService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAdminAuthService adminAuthService, ILogger<AuthController> logger)
        {
            _adminAuthService = adminAuthService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("login")] 
        public async Task<IActionResult> Login([FromBody] AdminLoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid request" });
                }

                var response = await _adminAuthService.LoginAsync(request);

                if (!response.Success)
                {
                    return Unauthorized(new { message = response.Message });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during admin login");
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.RefreshToken))
                {
                    return BadRequest(new { message = "Token and refresh token are required" });
                }

                var response = await _adminAuthService.RefreshTokenAsync(request.Token, request.RefreshToken);

                if (!response.Success)
                {
                    return BadRequest(new { message = response.Message });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return StatusCode(500, new { message = "An error occurred while refreshing token" });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("validate")]
        public IActionResult ValidateToken()
        {
            return Ok(new { message = "Token is valid", isAdmin = User.IsInRole("Admin") });
        }
    }

    public class RefreshTokenRequest
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
