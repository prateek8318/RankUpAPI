using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using Common.DTOs;


namespace UserService.Application.Services
{
    public class UserService : BaseService, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public UserService(IUserRepository userRepository, IMapper mapper, IImageService imageService, ILogger<UserService> logger) : base(logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _imageService = imageService;
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetUserEntityByIdAsync(userId);
            if (user == null) return null;
            
            var userDto = MapToResponseDto(user);
            
            // Set IST times
            userDto.CreatedAtIST = user.CreatedAt.AddHours(5.5);
            if (user.LastLoginAt.HasValue && !userDto.LastLoginAtIST.HasValue)
            {
                userDto.LastLoginAtIST = user.LastLoginAt.Value.AddHours(5.5);
            }
            
            // Calculate profile completion - all required fields must be present
            userDto.IsProfileComplete = 
                !string.IsNullOrWhiteSpace(user.Name) &&
                !string.IsNullOrWhiteSpace(user.Email) &&
                !string.IsNullOrWhiteSpace(user.Gender) &&
                user.DateOfBirth.HasValue &&
                user.StateId.HasValue &&
                user.LanguageId.HasValue &&
                user.QualificationId.HasValue &&
                user.CategoryId.HasValue &&
                user.StreamId.HasValue &&
                !string.IsNullOrWhiteSpace(user.ProfilePhoto) &&
                user.ExamId.HasValue &&
                user.InterestedInIntlExam;
            
            return userDto;
        }



        public async Task<UserDto> GetOrCreateUserAsync(string phoneNumber, string? countryCode = null, bool markPhoneVerified = false)

        {


            var fullPhoneNumber = string.IsNullOrWhiteSpace(countryCode) 

                ? phoneNumber 

                : $"{countryCode}{phoneNumber}";

                

            var user = await _userRepository.GetByPhoneNumberAsync(fullPhoneNumber);

            var isNewUser = false;



            if (user == null)

            {

                isNewUser = true;

                user = new User
                {
                    PhoneNumber = fullPhoneNumber,
                    CountryCode = countryCode ?? "+91",
                    Name = "", // Empty name by default
                    Email = null,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    LoginType = "Mobile"
                };



                await _userRepository.AddAsync(user);

                await _userRepository.SaveChangesAsync();

            }

            else if (!string.IsNullOrWhiteSpace(countryCode) && user.CountryCode != countryCode)

            {

                user.CountryCode = countryCode;

                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);

                await _userRepository.SaveChangesAsync();

            }

            // Mark phone as verified if this is OTP verification
            if (markPhoneVerified && !user.IsPhoneVerified)
            {
                user.IsPhoneVerified = true;
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();
            }



            var userDto = MapToResponseDto(user);

            userDto.IsNewUser = isNewUser;

            return userDto;

        }



        public async Task UpdateUserLoginInfoAsync(int userId)

        {

            var user = await _userRepository.GetUserEntityByIdAsync(userId);

            if (user != null)

            {

                user.LastLoginAt = DateTime.UtcNow;

                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);

            }

        }
        
        public async Task UpdateUserDeviceInfoAsync(int userId, string? deviceId = null, string? deviceType = null, string? deviceName = null)
        {
            var user = await _userRepository.GetUserEntityByIdAsync(userId);
            if (user != null)
            {
                if (!string.IsNullOrWhiteSpace(deviceId))
                    user.DeviceId = deviceId;
                if (!string.IsNullOrWhiteSpace(deviceType))
                    user.DeviceType = deviceType;
                if (!string.IsNullOrWhiteSpace(deviceName))
                    user.DeviceName = deviceName;
                    
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
            }
        }



        public async Task<UserDto> UpdateUserProfileAsync(int userId, ProfileUpdateRequest profileUpdate)

        {

            var user = await _userRepository.GetUserEntityByIdAsync(userId);

            if (user == null)

                throw new KeyNotFoundException($"User with ID {userId} not found");



            if (!string.IsNullOrWhiteSpace(profileUpdate.Name))

                user.Name = profileUpdate.Name;



            if (!string.IsNullOrWhiteSpace(profileUpdate.Email))

                user.Email = profileUpdate.Email;



            if (!string.IsNullOrWhiteSpace(profileUpdate.Gender))

                user.Gender = profileUpdate.Gender;



            if (profileUpdate.DateOfBirth.HasValue)

                user.DateOfBirth = profileUpdate.DateOfBirth.Value;



            if (!string.IsNullOrWhiteSpace(profileUpdate.Qualification))

                user.Qualification = profileUpdate.Qualification;



            if (!string.IsNullOrWhiteSpace(profileUpdate.LanguagePreference))

                user.PreferredLanguage = profileUpdate.LanguagePreference;



            if (!string.IsNullOrWhiteSpace(profileUpdate.PreferredExam))

                user.PreferredExam = profileUpdate.PreferredExam;



            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            await _userRepository.SaveChangesAsync();



            return MapToResponseDto(user);

        }



        public async Task<UserDto> PatchUserProfileAsync(int userId, PatchProfileRequest patchRequest)

        {

            var user = await _userRepository.GetUserEntityByIdAsync(userId);

            if (user == null)

                throw new KeyNotFoundException($"User with ID {userId} not found");

            

            // Field restrictions based on login type
            if (user.LoginType == "Mobile" && !string.IsNullOrWhiteSpace(patchRequest.Email) && patchRequest.Email != user.Email)
            {
                // For mobile login users, email can be updated (no restriction)
            }
            
            if (user.LoginType == "Social" && !string.IsNullOrWhiteSpace(patchRequest.Email) && patchRequest.Email != user.Email)
            {
                throw new InvalidOperationException("Email cannot be changed for social login users");
            }

            if (!string.IsNullOrWhiteSpace(patchRequest.FullName))

                user.Name = patchRequest.FullName;



            if (!string.IsNullOrWhiteSpace(patchRequest.Email))

                user.Email = patchRequest.Email;

            
            if (!string.IsNullOrWhiteSpace(patchRequest.PhoneNumber))
                user.PhoneNumber = patchRequest.PhoneNumber;



            if (!string.IsNullOrWhiteSpace(patchRequest.Gender))

                user.Gender = patchRequest.Gender;



            if (patchRequest.Dob.HasValue)

                user.DateOfBirth = patchRequest.Dob.Value;



            if (patchRequest.StateId.HasValue)

                user.StateId = patchRequest.StateId.Value;



            if (patchRequest.LanguageId.HasValue)

                user.LanguageId = patchRequest.LanguageId.Value;



            if (patchRequest.QualificationId.HasValue)

                user.QualificationId = patchRequest.QualificationId.Value;



            if (patchRequest.ExamId.HasValue)

                user.ExamId = patchRequest.ExamId.Value;



            if (patchRequest.CategoryId.HasValue)

                user.CategoryId = patchRequest.CategoryId.Value;



            if (patchRequest.StreamId.HasValue)

                user.StreamId = patchRequest.StreamId.Value;



            if (patchRequest.InterestedInIntlExam.HasValue)

                user.InterestedInIntlExam = patchRequest.InterestedInIntlExam.Value;



            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            await _userRepository.SaveChangesAsync();



            return MapToResponseDto(user);

        }



        public async Task<UserDto> PatchProfileWithImageAsync(int userId, PatchProfileFormData formData)

        {

            var user = await _userRepository.GetUserEntityByIdAsync(userId);

            if (user == null)

                throw new KeyNotFoundException($"User with ID {userId} not found");

            

            // Field restrictions based on login type
            if (user.LoginType == "Social" && !string.IsNullOrWhiteSpace(formData.Email) && formData.Email != user.Email)
            {
                throw new InvalidOperationException("Email cannot be changed for social login users");
            }




            if (formData.ProfilePhoto != null)

            {


                if (!string.IsNullOrWhiteSpace(user.ProfilePhoto))

                {

                    await _imageService.DeleteProfilePhotoAsync(user.ProfilePhoto);

                }




                var newPhotoPath = await _imageService.UploadProfilePhotoAsync(formData.ProfilePhoto, userId);

                if (!string.IsNullOrWhiteSpace(newPhotoPath))

                {

                    user.ProfilePhoto = newPhotoPath;

                }

            }




            if (!string.IsNullOrWhiteSpace(formData.FullName))
                user.Name = formData.FullName;



            if (!string.IsNullOrWhiteSpace(formData.Email))
                user.Email = formData.Email;



            if (!string.IsNullOrWhiteSpace(formData.PhoneNumber))
                user.PhoneNumber = formData.PhoneNumber;



            if (!string.IsNullOrWhiteSpace(formData.Gender))
                user.Gender = formData.Gender;



            if (formData.Dob.HasValue)
                user.DateOfBirth = formData.Dob.Value;



            if (formData.StateId.HasValue)

                user.StateId = formData.StateId.Value;



            if (formData.LanguageId.HasValue)

                user.LanguageId = formData.LanguageId.Value;



            if (formData.QualificationId.HasValue)

                user.QualificationId = formData.QualificationId.Value;



            if (formData.ExamId.HasValue)

                user.ExamId = formData.ExamId.Value;



            if (formData.CategoryId.HasValue)

                user.CategoryId = formData.CategoryId.Value;



            if (formData.StreamId.HasValue)

                user.StreamId = formData.StreamId.Value;



            if (formData.InterestedInIntlExam.HasValue)

                user.InterestedInIntlExam = formData.InterestedInIntlExam.Value;



            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            await _userRepository.SaveChangesAsync();



            return MapToResponseDto(user);

        }



        public async Task<IEnumerable<UserDto>> GetAllUsersAsync(int page = 1, int pageSize = 50)

        {

            var users = await _userRepository.GetAllAsync(page, pageSize);

            return users.Select(MapToResponseDto);

        }



        public async Task<int> GetTotalUsersCountAsync()

        {

            return await _userRepository.GetTotalUsersCountAsync();

        }



        public async Task<int> GetDailyActiveUsersCountAsync()

        {

            return await _userRepository.GetDailyActiveUsersCountAsync();

        }

        // Pagination support methods
        public async Task<PaginatedResponse<UserDto>> GetAllUsersPaginatedAsync(PaginationRequest pagination)
        {
            try
            {
                var users = await _userRepository.GetAllAsync(pagination.PageNumber, pagination.PageSize);
                var totalCount = await _userRepository.GetTotalUsersCountAsync();
                var userDtos = users.Select(u =>
                {
                    var userDto = MapToResponseDto(u);
                    // Set IST times
                    userDto.CreatedAtIST = u.CreatedAt.AddHours(5.5);
                    if (u.LastLoginAt.HasValue)
                        userDto.LastLoginAtIST = u.LastLoginAt.Value.AddHours(5.5);
                    return userDto;
                });
                
                return new PaginatedResponse<UserDto>
                {
                    Data = userDtos,
                    TotalCount = totalCount,
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated users");
                throw;
            }
        }

        public async Task<PaginatedResponse<UserDto>> GetActiveUsersPaginatedAsync(PaginationRequest pagination)
        {
            try
            {
                var users = await _userRepository.GetActiveAsync(pagination.PageNumber, pagination.PageSize);
                var totalCount = await _userRepository.GetTotalUsersCountAsync();
                var userDtos = users.Select(u =>
                {
                    var userDto = MapToResponseDto(u);
                    // Set IST times
                    userDto.CreatedAtIST = u.CreatedAt.AddHours(5.5);
                    if (u.LastLoginAt.HasValue)
                        userDto.LastLoginAtIST = u.LastLoginAt.Value.AddHours(5.5);
                    return userDto;
                });
                
                return new PaginatedResponse<UserDto>
                {
                    Data = userDtos,
                    TotalCount = totalCount,
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated active users");
                throw;
            }
        }

        private UserDto MapToResponseDto(User user)
        {
            var userDto = _mapper.Map<UserDto>(user);
            userDto.PhoneNumber = GetLocalPhoneNumber(userDto.PhoneNumber, userDto.CountryCode);
            return userDto;
        }

        private static string GetLocalPhoneNumber(string? phoneNumber, string? countryCode)
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
