using AutoMapper;
using QualificationService.Application.DTOs;
using QualificationService.Domain.Entities;

namespace QualificationService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateQualificationDto, Qualification>();
            CreateMap<UpdateQualificationDto, Qualification>();
            CreateMap<Qualification, QualificationDto>();
        }
    }
}
