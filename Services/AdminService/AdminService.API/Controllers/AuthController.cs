using AdminService.Application.DTOs;
using AdminService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminService.API.Controllers
{
    [Route("api/admin/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AuthController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AdminAuthResponse>> Login([FromBody] AdminLoginRequest request)
        {
            var response = await _adminService.LoginAsync(request);
            if (!response.Success)
                return Unauthorized(response);

            return Ok(response);
        }

        [HttpGet("profile/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AdminDto>> GetProfile(int id)
        {
            var admin = await _adminService.GetAdminByIdAsync(id);
            if (admin == null)
                return NotFound();

            return Ok(admin);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<AdminDto>>> GetAllAdmins()
        {
            var admins = await _adminService.GetAllAdminsAsync();
            return Ok(admins);
        }
    }
}
