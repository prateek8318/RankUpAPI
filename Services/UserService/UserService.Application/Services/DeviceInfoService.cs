using AutoMapper;
using Microsoft.Extensions.Logging;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Application.Services
{
    /// <summary>
    /// Service for managing device information for users
    /// </summary>
    public class DeviceInfoService : IDeviceInfoService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DeviceInfoService> _logger;

        public DeviceInfoService(
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<DeviceInfoService> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task StoreDeviceInfoAsync(int userId, DeviceInfoRequest request)
        {
            var user = await _userRepository.GetUserEntityByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }

            bool hasUpdates = false;

            if (!string.IsNullOrWhiteSpace(request.DeviceId) && user.DeviceId != request.DeviceId)
            {
                user.DeviceId = request.DeviceId;
                hasUpdates = true;
            }

            if (!string.IsNullOrWhiteSpace(request.DeviceType) && user.DeviceType != request.DeviceType)
            {
                user.DeviceType = request.DeviceType;
                hasUpdates = true;
            }

            if (!string.IsNullOrWhiteSpace(request.DeviceName) && user.DeviceName != request.DeviceName)
            {
                user.DeviceName = request.DeviceName;
                hasUpdates = true;
            }

            if (hasUpdates)
            {
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);

                _logger.LogInformation("Device information updated for user {UserId}", userId);
            }
        }

        public async Task<DeviceInfoResponse> GetDeviceInfoAsync(int userId)
        {
            var user = await _userRepository.GetUserEntityByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }

            return new DeviceInfoResponse
            {
                UserId = user.Id,
                FcmToken = user.FcmToken,
                DeviceId = user.DeviceId,
                DeviceType = user.DeviceType,
                DeviceName = user.DeviceName,
                LastUpdated = user.UpdatedAt
            };
        }

        public async Task UpdateFcmTokenAsync(int userId, string fcmToken)
        {
            var user = await _userRepository.GetUserEntityByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }

            if (user.FcmToken != fcmToken)
            {
                user.FcmToken = fcmToken;
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);

                _logger.LogInformation("FCM token updated for user {UserId}", userId);
            }
        }

        public async Task UpdateDeviceInfoFromOtpAsync(int userId, string? deviceId = null, string? deviceType = null, string? deviceName = null, string? fcmToken = null)
        {
            _logger.LogInformation("Starting device info update for user {UserId}. DeviceId: {DeviceId}, DeviceType: {DeviceType}, DeviceName: {DeviceName}, FcmToken: {FcmToken}", 
                userId, deviceId, deviceType, deviceName, fcmToken);

            var user = await _userRepository.GetUserEntityByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError("User {UserId} not found for device info update", userId);
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }

            var hasUpdates = false;

            // Only update if values are provided and different from current values
            if (!string.IsNullOrWhiteSpace(deviceId) && user.DeviceId != deviceId)
            {
                _logger.LogInformation("Updating DeviceId from {OldDeviceId} to {NewDeviceId} for user {UserId}", user.DeviceId, deviceId, userId);
                user.DeviceId = deviceId;
                hasUpdates = true;
            }

            if (!string.IsNullOrWhiteSpace(deviceType) && user.DeviceType != deviceType)
            {
                _logger.LogInformation("Updating DeviceType from {OldDeviceType} to {NewDeviceType} for user {UserId}", user.DeviceType, deviceType, userId);
                user.DeviceType = deviceType;
                hasUpdates = true;
            }

            if (!string.IsNullOrWhiteSpace(deviceName) && user.DeviceName != deviceName)
            {
                _logger.LogInformation("Updating DeviceName from {OldDeviceName} to {NewDeviceName} for user {UserId}", user.DeviceName, deviceName, userId);
                user.DeviceName = deviceName;
                hasUpdates = true;
            }

            if (!string.IsNullOrWhiteSpace(fcmToken) && user.FcmToken != fcmToken)
            {
                _logger.LogInformation("Updating FcmToken for user {UserId}", userId);
                user.FcmToken = fcmToken;
                hasUpdates = true;
            }

            // Always update last device login tracking when device info is provided
            if (!string.IsNullOrWhiteSpace(deviceType) || !string.IsNullOrWhiteSpace(deviceName))
            {
                _logger.LogInformation("Updating last device login tracking for user {UserId} at {Time}", userId, DateTime.UtcNow);
                user.LastDeviceLoginAt = DateTime.UtcNow;
                user.LastDeviceType = deviceType;
                user.LastDeviceName = deviceName;
                hasUpdates = true;
            }

            if (hasUpdates)
            {
                user.UpdatedAt = DateTime.UtcNow;
                _logger.LogInformation("Calling UpdateAsync for user {UserId} with device changes", userId);
                await _userRepository.UpdateAsync(user);

                _logger.LogInformation("Device information successfully updated for user {UserId} with device {DeviceType} {DeviceName}", 
                    userId, deviceType, deviceName);
            }
            else
            {
                _logger.LogInformation("No device information updates needed for user {UserId}", userId);
            }
        }
    }
}
