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
        private const string InvalidUserTokenMessage = "Invalid user token.";
        private const string InvalidProfileUpdateRequestMessage = "Invalid profile update request.";

        private readonly IUserService _userService;

        private readonly ILogger<UserController> _logger;



        public UserController(IUserService userService, ILogger<UserController> logger)

        {

            _userService = userService;

            _logger = logger;

        }




        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse))]

        [HttpGet("profile")]

        [Authorize]

        public async Task<ActionResult<UserDto>> GetProfile()

        {

            try

            {

                if (!TryGetCurrentUserId(out var userId))
                {
                    return Unauthorized(ApiResponse.CreateUnauthorized(
                        InvalidUserTokenMessage,
                        ErrorCodes.UNAUTHORIZED));
                }

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

                if (!TryGetCurrentUserId(out var userId))
                {
                    return Unauthorized(ApiResponse.CreateUnauthorized(
                        InvalidUserTokenMessage,
                        ErrorCodes.UNAUTHORIZED));
                }

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
        [Consumes("multipart/form-data")]

        [RequestFormLimits(MultipartBodyLengthLimit = 10485760)]
        [RequestSizeLimit(10485760)]

        public async Task<ActionResult<UserDto>> PatchProfileWithImage([FromForm] PatchProfileFormData formData)

        {

            try

            {

                if (!TryGetCurrentUserId(out var userId))
                {
                    return Unauthorized(ApiResponse.CreateUnauthorized(
                        InvalidUserTokenMessage,
                        ErrorCodes.UNAUTHORIZED));
                }

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
                _logger.LogWarning(ex, "Invalid profile-with-image payload");
                return BadRequest(ApiResponse.CreateBadRequest(

                    InvalidProfileUpdateRequestMessage,

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

        [HttpPatch("profile-with-image")]
        [Authorize]
        [Consumes("application/json")]
        public async Task<ActionResult<UserDto>> PatchProfileWithImageJson([FromBody] PatchProfileRequest patchRequest)
        {
            try
            {
                if (!TryGetCurrentUserId(out var userId))
                {
                    return Unauthorized(ApiResponse.CreateUnauthorized(
                        InvalidUserTokenMessage,
                        ErrorCodes.UNAUTHORIZED));
                }

                var formData = new PatchProfileFormData
                {
                    FullName = patchRequest.FullName,
                    Email = patchRequest.Email,
                    PhoneNumber = patchRequest.PhoneNumber,
                    Gender = patchRequest.Gender,
                    Dob = patchRequest.Dob,
                    StateId = patchRequest.StateId,
                    LanguageId = patchRequest.LanguageId,
                    QualificationId = patchRequest.QualificationId,
                    ExamId = patchRequest.ExamId,
                    CategoryId = patchRequest.CategoryId,
                    StreamId = patchRequest.StreamId,
                    InterestedInIntlExam = patchRequest.InterestedInIntlExam
                };

                var user = await _userService.PatchProfileWithImageAsync(userId, formData);

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
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid JSON patch profile payload");
                return BadRequest(ApiResponse.CreateBadRequest(
                    InvalidProfileUpdateRequestMessage,
                    ErrorCodes.MISSING_REQUIRED_FIELDS));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile via JSON patch");
                return StatusCode(500, ApiResponse.CreateInternalServerError(
                    "An error occurred while updating your profile. Please try again later.",
                    ErrorCodes.INTERNAL_SERVER_ERROR));
            }
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            userId = 0;
            var claimValue = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claimValue, out userId) && userId > 0;
        }

    }

}

