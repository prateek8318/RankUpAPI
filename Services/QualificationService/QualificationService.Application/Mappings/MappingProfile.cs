using AutoMapper;
using QualificationService.Application.DTOs;
using QualificationService.Domain.Entities;

namespace QualificationService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Qualification mappings
            CreateMap<CreateQualificationDto, Qualification>();
            CreateMap<UpdateQualificationDto, Qualification>();
            CreateMap<Qualification, QualificationDto>()
                .ForMember(dest => dest.StreamName, opt => opt.MapFrom(src => src.Stream != null ? src.Stream.Name : null));
            
            // Stream mappings
            CreateMap<CreateStreamDto, Domain.Entities.Stream>();
            CreateMap<UpdateStreamDto, Domain.Entities.Stream>();
            CreateMap<Domain.Entities.Stream, StreamDto>();
        }
    }
}
