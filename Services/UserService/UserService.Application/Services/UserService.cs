using AutoMapper;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetOrCreateUserAsync(string phoneNumber)
        {
            var user = await _userRepository.GetByPhoneNumberAsync(phoneNumber);

            if (user == null)
            {
                user = new User
                {
                    PhoneNumber = phoneNumber,
                    Name = $"User{phoneNumber}",
                    Email = null,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();
            }

            return _mapper.Map<UserDto>(user);
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
                user.LanguagePreference = profileUpdate.LanguagePreference;

            if (!string.IsNullOrWhiteSpace(profileUpdate.PreferredExam))
                user.PreferredExam = profileUpdate.PreferredExam;

            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }
    }
}
