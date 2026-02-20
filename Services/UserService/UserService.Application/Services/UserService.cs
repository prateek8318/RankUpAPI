using AutoMapper;
using Microsoft.AspNetCore.Http;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public UserService(IUserRepository userRepository, IMapper mapper, IImageService imageService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _imageService = imageService;
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetOrCreateUserAsync(string phoneNumber, string? countryCode = null)
        {
            // Use full phone number for lookup and storage
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
                    CountryCode = countryCode ?? "+91", // Default to India if not provided
                    Name = $"User{fullPhoneNumber}",
                    Email = null,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();
            }
            else if (!string.IsNullOrWhiteSpace(countryCode) && user.CountryCode != countryCode)
            {
                // Update country code if it's different
                user.CountryCode = countryCode;
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();
            }

            var userDto = _mapper.Map<UserDto>(user);
            userDto.IsNewUser = isNewUser; // Set the flag
            return userDto;
        }

        public async Task UpdateUserLoginInfoAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.LastLoginAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();
            }
        }

        public async Task<UserDto> UpdateUserProfileAsync(int userId, ProfileUpdateRequest profileUpdate)
        {
            var user = await _userRepository.GetByIdAsync(userId);
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

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> PatchUserProfileAsync(int userId, PatchProfileRequest patchRequest)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            if (!string.IsNullOrWhiteSpace(patchRequest.FullName))
                user.Name = patchRequest.FullName;

            if (!string.IsNullOrWhiteSpace(patchRequest.Email))
                user.Email = patchRequest.Email;

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

            if (patchRequest.InterestedInIntlExam.HasValue)
                user.InterestedInIntlExam = patchRequest.InterestedInIntlExam.Value;

            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> PatchProfileWithImageAsync(int userId, PatchProfileFormData formData)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            // Handle profile photo upload
            if (formData.ProfilePhoto != null)
            {
                // Delete old profile photo if exists
                if (!string.IsNullOrWhiteSpace(user.ProfilePhoto))
                {
                    await _imageService.DeleteProfilePhotoAsync(user.ProfilePhoto);
                }

                // Upload new profile photo
                var newPhotoPath = await _imageService.UploadProfilePhotoAsync(formData.ProfilePhoto, userId);
                if (!string.IsNullOrWhiteSpace(newPhotoPath))
                {
                    user.ProfilePhoto = newPhotoPath;
                }
            }

            // Update other fields
            if (!string.IsNullOrWhiteSpace(formData.FullName))
                user.Name = formData.FullName;

            if (!string.IsNullOrWhiteSpace(formData.Email))
                user.Email = formData.Email;

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

            if (formData.InterestedInIntlExam.HasValue)
                user.InterestedInIntlExam = formData.InterestedInIntlExam.Value;

            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync(int page = 1, int pageSize = 50)
        {
            var users = await _userRepository.GetAllAsync(page, pageSize);
            return users.Select(u => _mapper.Map<UserDto>(u));
        }

        public async Task<int> GetTotalUsersCountAsync()
        {
            return await _userRepository.GetTotalUsersCountAsync();
        }

        public async Task<int> GetDailyActiveUsersCountAsync()
        {
            return await _userRepository.GetDailyActiveUsersCountAsync();
        }
    }
}
