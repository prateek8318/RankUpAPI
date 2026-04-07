using System;
using System.Linq;
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
                var sanitizedGoogleId = NormalizeGoogleId(request.GoogleId);
                var normalizedPhoneNumber = NormalizePhoneNumber(request.MobileNumber);
                var normalizedFullPhoneNumber = BuildFullPhoneNumber(normalizedPhoneNumber);

                _logger.LogInformation($"SocialLogin request received: Email={request.Email}, Name={request.Name}, GoogleId={sanitizedGoogleId}");

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
                if (!string.IsNullOrWhiteSpace(sanitizedGoogleId))
                {
                    _logger.LogInformation($"Valid GoogleId provided: {sanitizedGoogleId}");
                }

                // Use GoogleId as primary identifier if available, fallback to email lookup (skip placeholder values)
                UserSocialLogin? existingSocialLogin = null;
                if (!string.IsNullOrEmpty(sanitizedGoogleId))
                {
                    existingSocialLogin = await _socialLoginRepository.GetByProviderAndGoogleIdAsync(
                        "Google", sanitizedGoogleId);
                }

                if (existingSocialLogin != null)
                {
                    var user = await _userRepository.GetUserEntityByIdAsync(existingSocialLogin.UserId);
                    if (user == null)
                    {
                        return new SocialLoginResponseDto
                        {
                            Success = false,
                            Message = "User not found"
                        };
                    }

                    // Check if email matches - if not, treat as new user scenario
                    if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
                    {
                        // Email changed, treat as new user - go to email lookup flow
                        _logger.LogInformation($"Email mismatch for GoogleId {sanitizedGoogleId}. Existing: {user.Email}, Requested: {request.Email}. Treating as new user.");
                        
                        // Update the existing social login with new email and proceed with new user flow
                        existingSocialLogin.Email = request.Email;
                        existingSocialLogin.Name = request.Name;
                        existingSocialLogin.AccessToken = request.AccessToken;
                        existingSocialLogin.RefreshToken = request.RefreshToken;
                        existingSocialLogin.ExpiresAt = request.ExpiresAt;
                        existingSocialLogin.UpdatedAt = DateTime.UtcNow;
                        await _socialLoginRepository.UpdateAsync(existingSocialLogin);
                        await _socialLoginRepository.SaveChangesAsync();
                        
                        // Update user info
                        user.Email = request.Email;
                        user.Name = request.Name;
                        
                        // Update phone number if provided
                        if (!string.IsNullOrWhiteSpace(normalizedFullPhoneNumber))
                        {
                            user.PhoneNumber = normalizedFullPhoneNumber;
                            user.CountryCode = DefaultCountryCode;
                        }
                        
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
                            Message = "Login successful with updated email",
                            Token = token,
                            RefreshToken = refreshToken,
                            ExpiresAt = DateTime.UtcNow.AddDays(30),
                            User = MapToUserDto(user),
                            IsNewUser = false
                        };
                    }
                    else
                    {
                        // Email matches, proceed with existing user login

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
                    
                    // Update phone number if provided
                    if (!string.IsNullOrWhiteSpace(normalizedFullPhoneNumber))
                    {
                        user.PhoneNumber = normalizedFullPhoneNumber;
                        user.CountryCode = DefaultCountryCode;
                    }
                    
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
                }
                else
                {
                    var existingUser = await FindExistingUserForSocialLoginAsync(request.Email, normalizedPhoneNumber, normalizedFullPhoneNumber);

                    if (existingUser != null)
                    {
                        var socialLoginRequest = new SocialLoginRequestDto
                        {
                            Provider = "Google",
                            GoogleId = sanitizedGoogleId,
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

                        // Set LoginType to Social for existing users who link social accounts
                        if (string.IsNullOrEmpty(existingUser.LoginType))
                        {
                            existingUser.LoginType = "Social";
                        }
                        
                        existingUser.LastLoginAt = DateTime.UtcNow;
                        existingUser.UpdatedAt = DateTime.UtcNow;
                        
                        // Update phone number if provided
                        if (!string.IsNullOrWhiteSpace(normalizedFullPhoneNumber))
                        {
                            existingUser.PhoneNumber = normalizedFullPhoneNumber;
                            existingUser.CountryCode = DefaultCountryCode;
                        }
                        
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
                            PhoneNumber = normalizedFullPhoneNumber,
                            CountryCode = DefaultCountryCode,
                            IsPhoneVerified = false,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            DeviceId = request.DeviceId,
                            DeviceType = request.DeviceType,
                            DeviceName = request.DeviceName,
                            LoginType = "Social"
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
                            GoogleId = sanitizedGoogleId,
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
                _logger.LogError(ex, "Error during social login. Email: {Email}, GoogleId: {GoogleId}, Error: {Error}", request?.Email, request?.GoogleId, ex.Message);
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
                    GoogleId = NormalizeGoogleId(request.GoogleId),
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

                var user = await _userRepository.GetUserEntityByIdAsync(userId);
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

        private const string DefaultCountryCode = "+91";

        private async Task<User?> FindExistingUserForSocialLoginAsync(string? email, string? normalizedPhoneNumber, string? normalizedFullPhoneNumber)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                var existingByEmail = await _userRepository.GetByEmailAsync(email);
                if (existingByEmail != null)
                    return existingByEmail;
            }

            if (!string.IsNullOrWhiteSpace(normalizedFullPhoneNumber))
            {
                var existingByFullPhone = await _userRepository.GetByPhoneNumberAsync(normalizedFullPhoneNumber);
                if (existingByFullPhone != null)
                    return existingByFullPhone;
            }

            if (!string.IsNullOrWhiteSpace(normalizedPhoneNumber))
            {
                var existingByLocalPhone = await _userRepository.GetByPhoneNumberAsync(normalizedPhoneNumber);
                if (existingByLocalPhone != null)
                    return existingByLocalPhone;
            }

            return null;
        }

        private static string? NormalizeGoogleId(string? googleId)
        {
            if (string.IsNullOrWhiteSpace(googleId))
                return null;

            var normalized = googleId.Trim();
            if (normalized.Equals("google", StringComparison.OrdinalIgnoreCase) ||
                normalized.Equals("null", StringComparison.OrdinalIgnoreCase) ||
                normalized.Equals("undefined", StringComparison.OrdinalIgnoreCase) ||
                normalized.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return normalized;
        }

        private static string? NormalizePhoneNumber(string? mobileNumber)
        {
            if (string.IsNullOrWhiteSpace(mobileNumber))
                return null;

            var digitsOnly = new string(mobileNumber.Where(char.IsDigit).ToArray());
            if (string.IsNullOrWhiteSpace(digitsOnly))
                return null;

            if (digitsOnly.Length > 10)
                return digitsOnly[^10..];

            return digitsOnly;
        }

        private static string? BuildFullPhoneNumber(string? normalizedPhoneNumber)
        {
            if (string.IsNullOrWhiteSpace(normalizedPhoneNumber))
                return null;

            return $"{DefaultCountryCode}{normalizedPhoneNumber}";
        }

        private UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                PhoneNumber = GetDisplayPhoneNumber(user.PhoneNumber, user.CountryCode),
                CountryCode = user.CountryCode,
                ProfilePhoto = user.ProfilePhoto,
                ProfilePhotoUrl = user.ProfilePhoto,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                IsPhoneVerified = user.IsPhoneVerified,
                InterestedInIntlExam = user.InterestedInIntlExam,
                CreatedAtIST = user.CreatedAtIST,
                LastLoginAtIST = user.LastLoginAtIST
            };
        }

        private static string GetDisplayPhoneNumber(string? phoneNumber, string? countryCode)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return string.Empty;

            var normalizedPhoneNumber = phoneNumber.Trim();
            var normalizedCountryCode = countryCode?.Trim();

            if (!string.IsNullOrWhiteSpace(normalizedCountryCode) &&
                normalizedPhoneNumber.StartsWith(normalizedCountryCode, StringComparison.Ordinal))
            {
                return normalizedPhoneNumber[normalizedCountryCode.Length..];
            }

            return normalizedPhoneNumber;
        }
    }
}
