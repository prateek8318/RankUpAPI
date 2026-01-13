using AutoMapper;
using UserService.Application.DTOs;
using UserService.Domain.Entities;

namespace UserService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
        }
    }
}
