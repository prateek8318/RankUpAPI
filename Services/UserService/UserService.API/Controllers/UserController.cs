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

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}/profile")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetProfile(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound();
                return Ok(user);
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while fetching profile" });
            }
        }

        [HttpPatch("{id}/profile")]
        [Authorize]
        public async Task<ActionResult<UserDto>> PatchProfile(int id, [FromBody] PatchProfileRequest request)
        {
            try
            {
                var user = await _userService.PatchUserProfileAsync(id, request);
                return Ok(user);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPatch("{id}/profile-with-image")]
        [Authorize]
        [RequestFormLimits(MultipartBodyLengthLimit = 10485760)] // 10MB limit
        [RequestSizeLimit(10485760)] // 10MB limit
        public async Task<ActionResult<UserDto>> PatchProfileWithImage(int id, [FromForm] PatchProfileFormData formData)
        {
            try
            {
                var user = await _userService.PatchProfileWithImageAsync(id, formData);
                return Ok(user);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while updating profile" });
            }
        }
    }
}
