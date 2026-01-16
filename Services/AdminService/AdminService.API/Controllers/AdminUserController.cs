using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdminService.Application.Interfaces;
using AdminService.Application.DTOs;

namespace AdminService.API.Controllers
{
    /// <summary>
    /// Admin User Management Controller - FR-ADM-08
    /// Orchestrates calls to UserService
    /// </summary>
    [Route("api/admin/users")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminUserController : ControllerBase
    {
        private readonly IUserServiceClient _userServiceClient;
        private readonly ILogger<AdminUserController> _logger;

        public AdminUserController(
            IUserServiceClient userServiceClient,
            ILogger<AdminUserController> logger)
        {
            _userServiceClient = userServiceClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponseDto<object>>> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                var users = await _userServiceClient.GetAllUsersAsync(page, pageSize);
                var totalCount = await _userServiceClient.GetTotalUsersCountAsync();
                
                return Ok(new PaginatedResponseDto<object>
                {
                    Items = users?.Cast<object>().ToList() ?? new List<object>(),
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<object>>> GetUserById(int id)
        {
            try
            {
                var user = await _userServiceClient.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound(new ApiResponseDto<object> { Success = false, ErrorMessage = "User not found" });

                return Ok(new ApiResponseDto<object> { Success = true, Data = user });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user {id}");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<object>>> UpdateUser(int id, [FromBody] object updateDto)
        {
            try
            {
                var user = await _userServiceClient.UpdateUserAsync(id, updateDto);
                if (user == null)
                    return NotFound(new ApiResponseDto<object> { Success = false, ErrorMessage = "User not found" });

                return Ok(new ApiResponseDto<object> { Success = true, Data = user });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user {id}");
                return StatusCode(500, new ApiResponseDto<object> { Success = false, ErrorMessage = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userServiceClient.DeleteUserAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("{id}/enable-disable")]
        public async Task<ActionResult> EnableDisableUser(int id, [FromBody] bool isActive)
        {
            try
            {
                var result = await _userServiceClient.EnableDisableUserAsync(id, isActive);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error enabling/disabling user {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
