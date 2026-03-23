using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Application.Services
{
    public class SocialLoginService : ISocialLoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly ISocialLoginRepository _socialLoginRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SocialLoginService> _logger;

        public SocialLoginService(
            IUserRepository userRepository,
            ISocialLoginRepository socialLoginRepository,
            IJwtTokenService jwtTokenService,
            IConfiguration configuration,
            ILogger<SocialLoginService> logger)
        {
            _userRepository = userRepository;
            _socialLoginRepository = socialLoginRepository;
            _jwtTokenService = jwtTokenService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<SocialLoginResponseDto> SocialLoginAsync(MinimalSocialLoginRequestDto request)
        {
            try
            {
                _logger.LogInformation($"SocialLogin request received: Email={request.Email}, Name={request.Name}, GoogleId={request.GoogleId}");

                // ✅ Early validation - DB tak null email pahunchne se rokho
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    _logger.LogWarning("SocialLogin failed: Email is missing in request");
                    return new SocialLoginResponseDto
                    {
                        Success = false,
                        Message = "Email is required for social login"
                    };
                }

                // GoogleId is optional - allow null/empty values
                if (!string.IsNullOrWhiteSpace(request.GoogleId) && request.GoogleId != "google")
                {
                    _logger.LogInformation($"Valid GoogleId provided: {request.GoogleId}");
                }

                // Use GoogleId as primary identifier if available, fallback to email lookup (skip placeholder values)
                UserSocialLogin? existingSocialLogin = null;
                if (!string.IsNullOrEmpty(request.GoogleId) && request.GoogleId != "google")
                {
                    existingSocialLogin = await _socialLoginRepository.GetByProviderAndGoogleIdAsync(
                        "Google", request.GoogleId ?? string.Empty);
                }

                if (existingSocialLogin != null)
                {
                    var user = await _userRepository.GetByIdAsync(existingSocialLogin.UserId);
                    if (user == null)
                    {
                        return new SocialLoginResponseDto
                        {
                            Success = false,
                            Message = "User not found"
                        };
                    }

                    // Update social login info
                    existingSocialLogin.AccessToken = request.AccessToken;
                    existingSocialLogin.RefreshToken = request.RefreshToken;
                    existingSocialLogin.ExpiresAt = request.ExpiresAt;
                    existingSocialLogin.UpdatedAt = DateTime.UtcNow;
                    await _socialLoginRepository.UpdateAsync(existingSocialLogin);
                    await _socialLoginRepository.SaveChangesAsync();

                    // Update last login info for existing user
                    user.LastLoginAt = DateTime.UtcNow;
                    user.UpdatedAt = DateTime.UtcNow;
                    
                    // Update device information if provided
                    if (!string.IsNullOrWhiteSpace(request.DeviceId))
                        user.DeviceId = request.DeviceId;
                    if (!string.IsNullOrWhiteSpace(request.DeviceType))
                        user.DeviceType = request.DeviceType;
                    if (!string.IsNullOrWhiteSpace(request.DeviceName))
                        user.DeviceName = request.DeviceName;
                    
                    await _userRepository.UpdateAsync(user);
                    await _userRepository.SaveChangesAsync();

                    var token = _jwtTokenService.GenerateToken(user);
                    var refreshToken = _jwtTokenService.GenerateRefreshToken();

                    return new SocialLoginResponseDto
                    {
                        Success = true,
                        Message = "Login successful",
                        Token = token,
                        RefreshToken = refreshToken,
                        ExpiresAt = DateTime.UtcNow.AddDays(30),
                        User = MapToUserDto(user),
                        IsNewUser = false
                    };
                }
                else
                {
                    var existingUser = await _userRepository.GetByEmailAsync(request.Email);

                    if (existingUser != null)
                    {
                        var socialLoginRequest = new SocialLoginRequestDto
                        {
                            Provider = "Google",
                            GoogleId = request.GoogleId,
                            Email = request.Email,
                            Name = request.Name,
                            AvatarUrl = request.AvatarUrl,
                            DeviceId = request.DeviceId,
                            DeviceType = request.DeviceType,
                            DeviceName = request.DeviceName,
                            FcmToken = request.FcmToken,
                            AccessToken = request.AccessToken,
                            RefreshToken = request.RefreshToken,
                            ExpiresAt = request.ExpiresAt
                        };

                        await LinkSocialAccountAsync(existingUser.Id, socialLoginRequest);

                        existingUser.LastLoginAt = DateTime.UtcNow;
                        existingUser.UpdatedAt = DateTime.UtcNow;
                        
                        // Update device information if provided
                        if (!string.IsNullOrWhiteSpace(request.DeviceId))
                            existingUser.DeviceId = request.DeviceId;
                        if (!string.IsNullOrWhiteSpace(request.DeviceType))
                            existingUser.DeviceType = request.DeviceType;
                        if (!string.IsNullOrWhiteSpace(request.DeviceName))
                            existingUser.DeviceName = request.DeviceName;
                        
                        await _userRepository.UpdateAsync(existingUser);
                        await _userRepository.SaveChangesAsync();

                        var token = _jwtTokenService.GenerateToken(existingUser);
                        var refreshToken = _jwtTokenService.GenerateRefreshToken();

                        return new SocialLoginResponseDto
                        {
                            Success = true,
                            Message = "Social account linked successfully",
                            Token = token,
                            RefreshToken = refreshToken,
                            ExpiresAt = DateTime.UtcNow.AddDays(30),
                            User = MapToUserDto(existingUser),
                            IsNewUser = false
                        };
                    }
                    else
                    {
                        var newUser = new User
                        {
                            Email = request.Email,
                            Name = request.Name,
                            ProfilePhoto = request.AvatarUrl,
                            PhoneNumber = null, // Allow null for social users, will be added later
                            IsPhoneVerified = false,
                            IsActive = true,
                            ProfileCompleted = false, // New social users have incomplete profiles
                            CreatedAt = DateTime.UtcNow,
                            DeviceId = request.DeviceId,
                            DeviceType = request.DeviceType,
                            DeviceName = request.DeviceName,
                        };

                        await _userRepository.AddAsync(newUser);
                        await _userRepository.SaveChangesAsync();

                        newUser.LastLoginAt = DateTime.UtcNow;
                        newUser.UpdatedAt = DateTime.UtcNow;
                        await _userRepository.UpdateAsync(newUser);
                        await _userRepository.SaveChangesAsync();

                        var socialLoginRequest = new SocialLoginRequestDto
                        {
                            Provider = "Google",
                            GoogleId = request.GoogleId,
                            Email = request.Email,
                            Name = request.Name,
                            AvatarUrl = request.AvatarUrl,
                            DeviceId = request.DeviceId,
                            DeviceType = request.DeviceType,
                            DeviceName = request.DeviceName,
                            FcmToken = request.FcmToken,
                            AccessToken = request.AccessToken,
                            RefreshToken = request.RefreshToken,
                            ExpiresAt = request.ExpiresAt
                        };

                        await LinkSocialAccountAsync(newUser.Id, socialLoginRequest);

                        var token = _jwtTokenService.GenerateToken(newUser);
                        var refreshToken = _jwtTokenService.GenerateRefreshToken();

                        return new SocialLoginResponseDto
                        {
                            Success = true,
                            Message = "Account created and logged in successfully",
                            Token = token,
                            RefreshToken = refreshToken,
                            ExpiresAt = DateTime.UtcNow.AddDays(30),
                            User = MapToUserDto(newUser),
                            IsNewUser = true
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during social login");
                return new SocialLoginResponseDto
                {
                    Success = false,
                    Message = "An error occurred during login"
                };
            }
        }

        public async Task<SocialLoginResponseDto> LinkSocialAccountAsync(int userId, SocialLoginRequestDto request)
        {
            try
            {
                var socialLogin = new UserSocialLogin
                {
                    UserId = userId,
                    Provider = "Google",
                    GoogleId = request.GoogleId == "google" ? null : request.GoogleId,
                    Email = request.Email,
                    Name = request.Name,
                    // AvatarUrl = request.AvatarUrl,
                    AccessToken = request.AccessToken,
                    RefreshToken = request.RefreshToken,
                    ExpiresAt = request.ExpiresAt,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _socialLoginRepository.AddAsync(socialLogin);
                await _socialLoginRepository.SaveChangesAsync();

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return new SocialLoginResponseDto
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }
                var token = _jwtTokenService.GenerateToken(user);
                var refreshToken = _jwtTokenService.GenerateRefreshToken();

                return new SocialLoginResponseDto
                {
                    Success = true,
                    Message = "Social account linked successfully",
                    Token = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddDays(30),
                    User = MapToUserDto(user),
                    IsNewUser = false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error linking social account");
                return new SocialLoginResponseDto
                {
                    Success = false,
                    Message = "Failed to link social account"
                };
            }
        }

        public async Task<bool> UnlinkSocialAccountAsync(int userId, string provider)
        {
            try
            {
                var socialLogin = await _socialLoginRepository.GetByUserIdAndProviderAsync(userId, provider);
                if (socialLogin != null)
                {
                    await _socialLoginRepository.DeleteAsync(socialLogin);
                    await _socialLoginRepository.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unlinking social account");
                return false;
            }
        }

        public async Task<bool> ValidateSocialTokenAsync(string provider, string accessToken)
        {
            await Task.Delay(100);
            return true;
        }

        private UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                CountryCode = user.CountryCode,
                ProfilePhoto = user.ProfilePhoto,
                ProfilePhotoUrl = user.ProfilePhoto,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                IsPhoneVerified = user.IsPhoneVerified,
                InterestedInIntlExam = user.InterestedInIntlExam,
                ProfileCompleted = user.ProfileCompleted
            };
        }
    }
}