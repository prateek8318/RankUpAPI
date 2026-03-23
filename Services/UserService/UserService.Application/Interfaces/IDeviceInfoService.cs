using UserService.Application.DTOs;

namespace UserService.Application.Interfaces
{
    /// <summary>
    /// Interface for device information management service
    /// </summary>
    public interface IDeviceInfoService
    {
        /// <summary>
        /// Store device information for a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="request">Device information request</param>
        /// <returns>Task</returns>
        Task StoreDeviceInfoAsync(int userId, DeviceInfoRequest request);

        /// <summary>
        /// Get device information for a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Device information response</returns>
        Task<DeviceInfoResponse> GetDeviceInfoAsync(int userId);

        /// <summary>
        /// Update FCM token for a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="fcmToken">FCM token</param>
        /// <returns>Task</returns>
        Task UpdateFcmTokenAsync(int userId, string fcmToken);

        /// <summary>
        /// Update device information for a user (called during OTP verification)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="deviceId">Device ID</param>
        /// <param name="deviceType">Device type</param>
        /// <param name="deviceName">Device name</param>
        /// <param name="fcmToken">FCM token</param>
        /// <returns>Task</returns>
        Task UpdateDeviceInfoFromOtpAsync(int userId, string? deviceId = null, string? deviceType = null, string? deviceName = null, string? fcmToken = null);
    }
}
