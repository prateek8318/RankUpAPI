using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;

namespace UserService.API.Controllers
{
    /// <summary>
    /// User Profile Management Controller - Handles user profile operations, language data, and photo uploads
    /// </summary>
    /// <remarks>
    /// **Features:**
    /// - Get current user profile
    /// - Partial profile updates (PATCH)
    /// - Profile photo upload
    /// - Multi-language data retrieval (states, qualifications, categories, streams)
    /// - International exam preferences
    /// 
    /// **Authentication:** All endpoints require JWT token except language data endpoints
    /// **File Upload:** Profile photos limited to 10MB, supports JPG, PNG, GIF formats
    /// </remarks>
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

        /// <summary>
        /// Get current user's profile information
        /// </summary>
        /// <returns>Complete user profile data</returns>
        /// <remarks>
        /// **Authentication:** Requires JWT token
        /// **Access:** Returns profile of the authenticated user only
        /// 
        /// **Response Includes:**
        /// - Basic Info: Name, email, phone, country code
        /// - Personal Details: Gender, date of birth, qualification
        /// - Preferences: Language, exam category, international exam interest
        /// - IDs: State, language, qualification, exam, category, stream IDs
        /// - Metadata: Last login, verification status, account status
        /// - Profile Photo: URL if uploaded
        /// 
        /// **Example Response:**
        /// {
        ///   "success": true,
        ///   "message": "User profile retrieved successfully",
        ///   "data": {
        ///     "id": 123,
        ///     "name": "John Doe",
        ///     "email": "john@example.com",
        ///     "phoneNumber": "9876543210",
        ///     "countryCode": "+91",
        ///     "gender": "Male",
        ///     "dateOfBirth": "1990-01-01",
        ///     "qualification": "Bachelor's Degree",
        ///     "languagePreference": "English",
        ///     "profilePhotoUrl": "https://example.com/photos/user123.jpg",
        ///     "preferredExam": "Engineering",
        ///     "stateId": 1,
        ///     "languageId": 1,
        ///     "qualificationId": 1,
        ///     "examId": 1,
        ///     "lastLoginAt": "2024-01-15T10:30:00Z",
        ///     "isPhoneVerified": true,
        ///     "isActive": true,
        ///     "createdAt": "2024-01-01T00:00:00Z",
        ///     "isNewUser": false,
        ///     "interestedInIntlExam": true
        ///   }
        /// }
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UserDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse))]
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
