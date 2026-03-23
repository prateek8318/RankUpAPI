using AutoMapper;
using UserService.Application.DTOs;
using UserService.Domain.Entities;

namespace UserService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.ProfilePhotoUrl, opt => opt.MapFrom(src => 
                    string.IsNullOrEmpty(src.ProfilePhoto) ? null : $"http://localhost:5002/{src.ProfilePhoto}"))
                .ForMember(dest => dest.LastDeviceLoginAt, opt => opt.MapFrom(src => src.LastDeviceLoginAt))
                .ForMember(dest => dest.LastDeviceType, opt => opt.MapFrom(src => src.LastDeviceType))
                .ForMember(dest => dest.LastDeviceName, opt => opt.MapFrom(src => src.LastDeviceName))
                .ForMember(dest => dest.FcmToken, opt => opt.MapFrom(src => src.FcmToken));
        }
    }
}
