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

        public async Task<ActionResult<AdminLoginResponse>> Login([FromBody] AdminLoginRequest request)

        {

            var response = await _adminService.LoginAsync(request);

            if (!response.Success)

                return Unauthorized(response);



            return Ok(response);

        }



        [HttpPost("verify-otp")]

        [AllowAnonymous]

        public async Task<ActionResult<AdminAuthResponse>> VerifyOtp([FromBody] AdminOtpVerificationRequest request)

        {

            var response = await _adminService.VerifyOtpAsync(request);

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



        [HttpPost("logout")]

        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Logout()

        {

            try

            {

                // Get admin info from token for logging

                var adminId = User.FindFirst("AdminId")?.Value;

                

                if (!string.IsNullOrEmpty(adminId))

                {

                    await _adminService.LogActivityAsync(int.Parse(adminId), "Logout", "Auth", null, "Admin logged out");

                }



                // For JWT tokens, client-side token removal is the primary logout mechanism

                // For enhanced security, we could implement token blacklisting here if needed

                

                return Ok(new { success = true, message = "Logout successful. Please discard the token from client side." });

            }

            catch (Exception ex)

            {

                return StatusCode(500, new { success = false, message = "An error occurred during logout. Please try again." });

            }

        }

    }

}

