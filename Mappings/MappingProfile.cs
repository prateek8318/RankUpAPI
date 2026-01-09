using AutoMapper;
using RankUpAPI.DTOs;
using RankUpAPI.Models;

namespace RankUpAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Qualification Mappings
            CreateMap<CreateQualificationDto, Qualification>();
            CreateMap<UpdateQualificationDto, Qualification>();
            CreateMap<Qualification, QualificationDto>();

            // Exam Mappings
            CreateMap<CreateExamDto, Exam>();
            CreateMap<UpdateExamDto, Exam>();
            CreateMap<Exam, ExamDto>()
                .ForMember(dest => dest.QualificationIds, 
                          opt => opt.MapFrom(src => src.ExamQualifications.Select(eq => eq.QualificationId).ToList()));

            // Subject Mappings
            CreateMap<CreateSubjectDto, Subject>();
            CreateMap<UpdateSubjectDto, Subject>();

            // Chapter Mappings
            CreateMap<CreateChapterDto, Chapter>();
            CreateMap<UpdateChapterDto, Chapter>();

            // TestSeries Mappings
            CreateMap<CreateTestSeriesDto, TestSeries>();
            CreateMap<UpdateTestSeriesDto, TestSeries>();

            // Question Mappings
            CreateMap<CreateQuestionDto, Question>();
            CreateMap<UpdateQuestionDto, Question>();
        }
    }
}
