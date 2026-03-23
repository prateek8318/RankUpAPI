using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/users/device")]
    [Authorize]
    public class DeviceInfoController : ControllerBase
    {
        private readonly IDeviceInfoService _deviceInfoService;
        private readonly ILogger<DeviceInfoController> _logger;

        public DeviceInfoController(
            IDeviceInfoService deviceInfoService,
            ILogger<DeviceInfoController> logger)
        {
            _deviceInfoService = deviceInfoService;
            _logger = logger;
        }

        /// <summary>
        /// Store or update device information for authenticated user
        /// </summary>
        /// <remarks>
        /// **Usage:** Store device information when user logs in or updates device details
        /// **Authentication:** Requires valid JWT token
        /// 
        /// **Field Descriptions:**
        /// - FcmToken: Firebase Cloud Messaging token for push notifications
        /// - DeviceId: Unique device identifier for tracking
        /// - DeviceType: Device platform ("android", "ios", "web")
        /// - DeviceName: Device name or model (e.g., "Samsung", "iPhone 14", "Pixel 7")
        /// 
        /// **Examples:**
        /// ```json
        /// {
        ///   "fcmToken": "firebase_fcm_token_here",
        ///   "deviceId": "unique_device_id",
        ///   "deviceType": "android",
        ///   "deviceName": "Samsung Galaxy S21"
        /// }
        /// ```
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiResponse))]
        [HttpPost("store")]
        public async Task<IActionResult> StoreDeviceInfo([FromBody] DeviceInfoRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(ApiResponse.CreateUnauthorized(
                        "Invalid or missing user authentication token",
                        ErrorCodes.UNAUTHORIZED));
                }

                if (string.IsNullOrWhiteSpace(request.DeviceId) && 
                    string.IsNullOrWhiteSpace(request.DeviceType) && 
                    string.IsNullOrWhiteSpace(request.DeviceName) &&
                    string.IsNullOrWhiteSpace(request.FcmToken))
                {
                    return BadRequest(ApiResponse.CreateBadRequest(
                        "At least one device information field is required",
                        ErrorCodes.MISSING_REQUIRED_FIELDS));
                }

                await _deviceInfoService.StoreDeviceInfoAsync(userId, request);

                return Ok(ApiResponse.CreateSuccess(
                    "Device information stored successfully",
                    new { userId, deviceStored = true }));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse.CreateNotFound(
                    "User not found",
                    ErrorCodes.USER_NOT_FOUND));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error storing device info: {ex.Message}");
                return StatusCode(500, ApiResponse.CreateInternalServerError(
                    "Unable to store device information. Please try again later.",
                    ErrorCodes.INTERNAL_SERVER_ERROR));
            }
        }

        /// <summary>
        /// Get device information for authenticated user
        /// </summary>
        /// <remarks>
        /// **Usage:** Retrieve stored device information for the authenticated user
        /// **Authentication:** Requires valid JWT token
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [HttpGet("get")]
        public async Task<IActionResult> GetDeviceInfo()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(ApiResponse.CreateUnauthorized(
                        "Invalid or missing user authentication token",
                        ErrorCodes.UNAUTHORIZED));
                }

                var deviceInfo = await _deviceInfoService.GetDeviceInfoAsync(userId);
                
                return Ok(ApiResponse.CreateSuccess(
                    "Device information retrieved successfully",
                    deviceInfo));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse.CreateNotFound(
                    "User not found",
                    ErrorCodes.USER_NOT_FOUND));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving device info: {ex.Message}");
                return StatusCode(500, ApiResponse.CreateInternalServerError(
                    "Unable to retrieve device information. Please try again later.",
                    ErrorCodes.INTERNAL_SERVER_ERROR));
            }
        }

        /// <summary>
        /// Update FCM token for authenticated user
        /// </summary>
        /// <remarks>
        /// **Usage:** Update Firebase Cloud Messaging token for push notifications
        /// **Authentication:** Requires valid JWT token
        /// 
        /// **Examples:**
        /// ```json
        /// {
        ///   "fcmToken": "new_firebase_fcm_token_here"
        /// }
        /// ```
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiResponse))]
        [HttpPut("fcm-token")]
        public async Task<IActionResult> UpdateFcmToken([FromBody] UpdateFcmTokenRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(ApiResponse.CreateUnauthorized(
                        "Invalid or missing user authentication token",
                        ErrorCodes.UNAUTHORIZED));
                }

                if (string.IsNullOrWhiteSpace(request.FcmToken))
                {
                    return BadRequest(ApiResponse.CreateBadRequest(
                        "FCM token is required",
                        ErrorCodes.MISSING_REQUIRED_FIELDS));
                }

                await _deviceInfoService.UpdateFcmTokenAsync(userId, request.FcmToken);

                return Ok(ApiResponse.CreateSuccess(
                    "FCM token updated successfully",
                    new { userId, fcmTokenUpdated = true }));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse.CreateNotFound(
                    "User not found",
                    ErrorCodes.USER_NOT_FOUND));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating FCM token: {ex.Message}");
                return StatusCode(500, ApiResponse.CreateInternalServerError(
                    "Unable to update FCM token. Please try again later.",
                    ErrorCodes.INTERNAL_SERVER_ERROR));
            }
        }
    }
}
