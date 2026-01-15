using AutoMapper;
using HomeDashboardService.Application.DTOs;
using HomeDashboardService.Domain.Entities;

namespace HomeDashboardService.Application.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Exam mappings
            CreateMap<Exam, ExamDto>();
            CreateMap<CreateExamDto, Exam>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Subjects, opt => opt.Ignore());
            CreateMap<UpdateExamDto, Exam>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.Subjects, opt => opt.Ignore());

            // Subject mappings
            CreateMap<Subject, SubjectDto>()
                .ForMember(dest => dest.ExamName, opt => opt.MapFrom(src => src.Exam != null ? src.Exam.Name : string.Empty));
            CreateMap<CreateSubjectDto, Subject>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Exam, opt => opt.Ignore())
                .ForMember(dest => dest.Chapters, opt => opt.Ignore());
            CreateMap<UpdateSubjectDto, Subject>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.ExamId, opt => opt.Ignore())
                .ForMember(dest => dest.Exam, opt => opt.Ignore())
                .ForMember(dest => dest.Chapters, opt => opt.Ignore());

            // Chapter mappings
            CreateMap<Chapter, ChapterDto>()
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject != null ? src.Subject.Name : string.Empty));
            CreateMap<CreateChapterDto, Chapter>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Subject, opt => opt.Ignore())
                .ForMember(dest => dest.Quizzes, opt => opt.Ignore());
            CreateMap<UpdateChapterDto, Chapter>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.SubjectId, opt => opt.Ignore())
                .ForMember(dest => dest.Subject, opt => opt.Ignore())
                .ForMember(dest => dest.Quizzes, opt => opt.Ignore());

            // Quiz mappings
            CreateMap<Quiz, QuizDto>()
                .ForMember(dest => dest.ChapterName, opt => opt.MapFrom(src => src.Chapter != null ? src.Chapter.Name : string.Empty));
            CreateMap<CreateQuizDto, Quiz>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Chapter, opt => opt.Ignore())
                .ForMember(dest => dest.Questions, opt => opt.Ignore())
                .ForMember(dest => dest.QuizAttempts, opt => opt.Ignore());
            CreateMap<UpdateQuizDto, Quiz>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.ChapterId, opt => opt.Ignore())
                .ForMember(dest => dest.Chapter, opt => opt.Ignore())
                .ForMember(dest => dest.Questions, opt => opt.Ignore())
                .ForMember(dest => dest.QuizAttempts, opt => opt.Ignore());

            // Question mappings
            CreateMap<Question, QuestionDto>();
            CreateMap<QuestionOption, QuestionOptionDto>();
            CreateMap<CreateQuestionOptionDto, QuestionOption>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.QuestionId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Question, opt => opt.Ignore());

            // Dashboard mappings
            CreateMap<DashboardBanner, DashboardBannerDto>();
            CreateMap<OfferBanner, OfferBannerDto>();
            CreateMap<DailyVideo, DailyVideoDto>();
            CreateMap<Notification, NotificationDto>();

            // Bulk Upload mappings
            CreateMap<BulkUploadLog, BulkUploadLogDto>();
            CreateMap<BulkUploadLog, BulkUploadResultDto>();
        }
    }
}
