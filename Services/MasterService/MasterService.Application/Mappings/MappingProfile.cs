using AutoMapper;
using MasterService.Application.DTOs;
using MasterService.Domain.Entities;

namespace MasterService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateLanguageDto, Language>();
            CreateMap<UpdateLanguageDto, Language>();
            CreateMap<Language, LanguageDto>();

            CreateMap<CreateStateDto, State>();
            CreateMap<UpdateStateDto, State>();
            CreateMap<State, StateDto>();
        }
    }
}
