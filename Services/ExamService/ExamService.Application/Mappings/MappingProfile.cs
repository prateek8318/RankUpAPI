using AutoMapper;
using ExamService.Application.DTOs;
using ExamService.Domain.Entities;

namespace ExamService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateExamDto, Exam>();
            CreateMap<UpdateExamDto, Exam>();
            CreateMap<Exam, ExamDto>();
        }
    }
}
