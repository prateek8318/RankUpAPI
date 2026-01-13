using AutoMapper;
using QuestionService.Application.DTOs;
using QuestionService.Domain.Entities;

namespace QuestionService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateQuestionDto, Question>();
            CreateMap<UpdateQuestionDto, Question>();
            CreateMap<Question, QuestionDto>();
        }
    }
}
