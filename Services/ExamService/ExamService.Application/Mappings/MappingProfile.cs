using AutoMapper;
using ExamService.Application.DTOs;
using ExamService.Domain.Entities;

namespace ExamService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateExamDto, Exam>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore());
            CreateMap<UpdateExamDto, Exam>();
            CreateMap<Exam, ExamDto>();
        }
    }
}
