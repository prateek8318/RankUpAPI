using AutoMapper;
using RankUpAPI.DTOs;
using RankUpAPI.Areas.Admin.Exam.DTOs;
using RankUpAPI.Areas.Admin.Qualification.DTOs;
using RankUpAPI.Areas.Admin.Subject.DTOs;
using RankUpAPI.Areas.Admin.Chapter.DTOs;
using RankUpAPI.Areas.Admin.TestSeries.DTOs;
using RankUpAPI.Areas.Admin.Question.DTOs;
using RankUpAPI.Models;

namespace RankUpAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Qualification Mappings (Admin)
            CreateMap<RankUpAPI.Areas.Admin.Qualification.DTOs.CreateQualificationDto, Qualification>();
            CreateMap<RankUpAPI.Areas.Admin.Qualification.DTOs.UpdateQualificationDto, Qualification>();
            CreateMap<Qualification, RankUpAPI.Areas.Admin.Qualification.DTOs.QualificationDto>();

            // Exam Mappings (using new namespace)
            CreateMap<RankUpAPI.Areas.Admin.Exam.DTOs.CreateExamDto, Exam>();
            CreateMap<RankUpAPI.Areas.Admin.Exam.DTOs.UpdateExamDto, Exam>();
            CreateMap<Exam, RankUpAPI.Areas.Admin.Exam.DTOs.ExamDto>()
                .ForMember(dest => dest.QualificationIds, 
                          opt => opt.MapFrom(src => src.ExamQualifications.Select(eq => eq.QualificationId).ToList()));
            
            // Legacy Exam Mappings (for backward compatibility)
            CreateMap<RankUpAPI.DTOs.CreateExamDto, Exam>();
            CreateMap<RankUpAPI.DTOs.UpdateExamDto, Exam>();
            CreateMap<Exam, RankUpAPI.DTOs.ExamDto>()
                .ForMember(dest => dest.QualificationIds, 
                          opt => opt.MapFrom(src => src.ExamQualifications.Select(eq => eq.QualificationId).ToList()));

            // Subject Mappings (Admin)
            CreateMap<RankUpAPI.Areas.Admin.Subject.DTOs.CreateSubjectDto, Subject>();
            CreateMap<RankUpAPI.Areas.Admin.Subject.DTOs.UpdateSubjectDto, Subject>();

            // Chapter Mappings (Admin)
            CreateMap<RankUpAPI.Areas.Admin.Chapter.DTOs.CreateChapterDto, Chapter>();
            CreateMap<RankUpAPI.Areas.Admin.Chapter.DTOs.UpdateChapterDto, Chapter>();

            // TestSeries Mappings (Admin)
            CreateMap<RankUpAPI.Areas.Admin.TestSeries.DTOs.CreateTestSeriesDto, TestSeries>();
            CreateMap<RankUpAPI.Areas.Admin.TestSeries.DTOs.UpdateTestSeriesDto, TestSeries>();

            // Question Mappings (Admin)
            CreateMap<RankUpAPI.Areas.Admin.Question.DTOs.CreateQuestionDto, Question>();
            CreateMap<RankUpAPI.Areas.Admin.Question.DTOs.UpdateQuestionDto, Question>();
        }
    }
}
