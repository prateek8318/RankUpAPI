using AutoMapper;
using QuizService.Application.DTOs;
using QuizService.Domain.Entities;

namespace QuizService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateTestSeriesDto, TestSeries>();
            CreateMap<UpdateTestSeriesDto, TestSeries>();
            CreateMap<TestSeries, TestSeriesDto>();

            CreateMap<CreateSubjectDto, Subject>();
            CreateMap<UpdateSubjectDto, Subject>();
            CreateMap<Subject, SubjectDto>();

            CreateMap<CreateChapterDto, Chapter>();
            CreateMap<UpdateChapterDto, Chapter>();
            CreateMap<Chapter, ChapterDto>();
        }
    }
}
