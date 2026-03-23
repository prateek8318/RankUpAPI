using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using UserService.API.Binders;

namespace UserService.API.Controllers
{
    [Route("api/users/auth")]
    public class SocialLoginController : ControllerBase
    {
        private readonly ISocialLoginService _socialLoginService;
        private readonly ILogger<SocialLoginController> _logger;

        public SocialLoginController(
            ISocialLoginService socialLoginService,
            ILogger<SocialLoginController> logger)
        {
            _socialLoginService = socialLoginService;
            _logger = logger;
        }

        [HttpPost("social-login")]
        public async Task<IActionResult> SocialLogin([ModelBinder(BinderType = typeof(NoValidationModelBinder))] MinimalSocialLoginRequestDto request)
        {
            try
            {
                var result = await _socialLoginService.SocialLoginAsync(request);

                if (result.Success)
                {
                    var user = result.User;

                    var data = new Dictionary<string, object?>
                    {
                        ["token"] = result.Token,
                        ["refreshToken"] = result.RefreshToken,
                        ["expiresAt"] = result.ExpiresAt,
                        ["userId"] = user.Id,
                        ["userName"] = user.Name,
                        ["isNewUser"] = result.IsNewUser,
                        ["isProfileComplete"] = user.ProfileCompleted,
                        ["phoneNumber"] = user.PhoneNumber,
                        ["isPhoneVerified"] = user.IsPhoneVerified
                    };

                    return Ok(ApiResponse.CreateSuccess(
                        result.Message,
                        data));
                }

                return BadRequest(ApiResponse.CreateBadRequest(result.Message));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error during social login");
                return StatusCode(500, ApiResponse.CreateInternalServerError("Internal server error"));
            }
        }

        [HttpPost("link")]
        [Authorize]
        public async Task<IActionResult> LinkSocialAccount([FromBody] SocialLoginRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.CreateBadRequest("Invalid request data"));
                }

                var userId = int.Parse(User.FindFirst("sub")?.Value);
                var result = await _socialLoginService.LinkSocialAccountAsync(userId, request);

                if (result.Success)
                {
                    var user = result.User;

                    var data = new Dictionary<string, object?>
                    {
                        ["token"] = result.Token,
                        ["refreshToken"] = result.RefreshToken,
                        ["expiresAt"] = result.ExpiresAt,
                        ["userId"] = user.Id,
                        ["userName"] = user.Name,
                        ["isNewUser"] = result.IsNewUser,
                        ["isProfileComplete"] = user.ProfileCompleted,
                        ["phoneNumber"] = user.PhoneNumber,
                        ["isPhoneVerified"] = user.IsPhoneVerified
                    };

                    return Ok(ApiResponse.CreateSuccess(
                        result.Message,
                        data));
                }

                return BadRequest(ApiResponse.CreateBadRequest(result.Message));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error linking social account");
                return StatusCode(500, ApiResponse.CreateInternalServerError("Internal server error"));
            }
        }

        [HttpDelete("unlink/{provider}")]
        [Authorize]
        public async Task<IActionResult> UnlinkSocialAccount(string provider)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("sub")?.Value);
                var result = await _socialLoginService.UnlinkSocialAccountAsync(userId, provider);

                if (result)
                {
                    return Ok(ApiResponse.CreateSuccess("Social account unlinked successfully"));
                }

                return BadRequest(ApiResponse.CreateBadRequest("Failed to unlink social account"));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error unlinking social account");
                return StatusCode(500, ApiResponse.CreateInternalServerError("Internal server error"));
            }
        }

        [HttpPost("validate-token")]
        public async Task<IActionResult> ValidateSocialToken([FromBody] ValidateTokenRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.CreateBadRequest("Invalid request data"));
                }

                var isValid = await _socialLoginService.ValidateSocialTokenAsync(request.Provider, request.AccessToken);

                return Ok(ApiResponse.CreateSuccess("Token validation result", new { isValid }));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error validating social token");
                return StatusCode(500, ApiResponse.CreateInternalServerError("Internal server error"));
            }
        }
    }

    public class ValidateTokenRequestDto
    {
        public string Provider { get; set; }
        public string AccessToken { get; set; }
    }
}
