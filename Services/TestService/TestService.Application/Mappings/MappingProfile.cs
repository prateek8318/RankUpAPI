using AutoMapper;
using TestService.Application.DTOs;
using TestService.Domain.Entities;

namespace TestService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Test mappings
            CreateMap<Test, TestDto>();
            CreateMap<CreateTestDto, Test>();
            CreateMap<UpdateTestDto, Test>();

            // TestSeries mappings
            CreateMap<TestSeries, TestSeriesDto>();
            CreateMap<CreateTestSeriesDto, TestSeries>();
            CreateMap<UpdateTestSeriesDto, TestSeries>();

            // PracticeMode mappings
            CreateMap<PracticeMode, PracticeModeDto>();

            // ExamMaster mappings
            CreateMap<ExamMaster, ExamDto>();
            CreateMap<SubjectMaster, SubjectDto>();

            // Question mappings
            CreateMap<Question, QuestionDto>();
            CreateMap<CreateQuestionDto, Question>();
            CreateMap<UpdateQuestionDto, Question>();

            // TestExecution mappings
            CreateMap<UserTestAttempt, UserTestAttemptDto>();
            CreateMap<CreateUserTestAttemptDto, UserTestAttempt>();
            CreateMap<UpdateUserTestAttemptDto, UserTestAttempt>();

            // TestQuestion mappings
            CreateMap<TestQuestion, TestQuestionDto>();
            CreateMap<CreateTestQuestionDto, TestQuestion>();
            CreateMap<UpdateTestQuestionDto, TestQuestion>();
        }
    }
}
