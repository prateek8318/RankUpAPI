using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;

namespace UserService.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(ApiResponse.CreateNotFound(
                        $"User profile with ID {userId} was not found",
                        ErrorCodes.USER_NOT_FOUND));
                }
                return Ok(ApiResponse.CreateSuccess(
                    "User profile retrieved successfully",
                    user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user profile");
                return StatusCode(500, ApiResponse.CreateInternalServerError(
                    "An error occurred while fetching your profile. Please try again later.",
                    ErrorCodes.INTERNAL_SERVER_ERROR));
            }
        }

        [HttpPatch("profile")]
        [Authorize]
        public async Task<ActionResult<UserDto>> PatchProfile([FromBody] PatchProfileRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
                var user = await _userService.PatchUserProfileAsync(userId, request);
                return Ok(ApiResponse.CreateSuccess(
                    "User profile updated successfully",
                    user));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse.CreateNotFound(
                    "User profile not found for update",
                    ErrorCodes.USER_NOT_FOUND));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                return StatusCode(500, ApiResponse.CreateInternalServerError(
                    "An error occurred while updating your profile. Please try again later.",
                    ErrorCodes.INTERNAL_SERVER_ERROR));
            }
        }

        [HttpPatch("profile-with-image")]
        [Authorize]
        [RequestFormLimits(MultipartBodyLengthLimit = 10485760)] // 10MB limit
        [RequestSizeLimit(10485760)] // 10MB limit
        public async Task<ActionResult<UserDto>> PatchProfileWithImage([FromForm] PatchProfileFormData formData)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
                var user = await _userService.PatchProfileWithImageAsync(userId, formData);
                return Ok(ApiResponse.CreateSuccess(
                    "User profile with photo updated successfully",
                    user));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse.CreateNotFound(
                    "User profile not found for update with photo",
                    ErrorCodes.USER_NOT_FOUND));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse.CreateBadRequest(
                    ex.Message,
                    ErrorCodes.INVALID_FILE_FORMAT));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile with image");
                return StatusCode(500, ApiResponse.CreateInternalServerError(
                    "An error occurred while updating your profile photo. Please try again later.",
                    ErrorCodes.FILE_UPLOAD_ERROR));
            }
        }
    }
}
