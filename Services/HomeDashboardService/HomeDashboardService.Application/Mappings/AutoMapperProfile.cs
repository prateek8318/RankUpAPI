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
            CreateMap<HomeBanner, HomeBannerDto>();
            CreateMap<OfferBanner, OfferBannerDto>();
            CreateMap<DailyVideo, DailyVideoDto>();
            CreateMap<Notification, NotificationDto>();

            // Home Page mappings
            CreateMap<PracticeMode, PracticeModeDto>();
            CreateMap<CreatePracticeModeDto, PracticeMode>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UpdatePracticeModeDto, PracticeMode>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<DailyTarget, DailyTargetDto>();

            CreateMap<RapidFireTest, RapidFireTestDto>();
            CreateMap<CreateRapidFireTestDto, RapidFireTest>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UpdateRapidFireTestDto, RapidFireTest>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Rapid Fire Test with Images mappings
            CreateMap<RapidFireTest, RapidFireTestImagesDto>();
            CreateMap<CreateRapidFireTestWithImagesDto, RapidFireTest>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UpdateRapidFireTestWithImagesDto, RapidFireTest>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<FreeTest, FreeTestDto>();
            CreateMap<CreateFreeTestDto, FreeTest>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UpdateFreeTestDto, FreeTest>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Note: MotivationMessageDto will be created inline in service
            CreateMap<CreateMotivationMessageDto, MotivationMessage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UpdateMotivationMessageDto, MotivationMessage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<SubscriptionBanner, SubscriptionBannerConfigDto>();
            CreateMap<CreateSubscriptionBannerDto, SubscriptionBanner>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UpdateSubscriptionBannerDto, SubscriptionBanner>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<ContinuePracticeItem, ContinuePracticeItemDto>();

            CreateMap<HomeBanner, HomeBannerDto>();
            CreateMap<CreateHomeBannerDto, HomeBanner>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UpdateHomeBannerDto, HomeBanner>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<OfferBanner, OfferBannerDto>();
            CreateMap<CreateOfferBannerDto, OfferBanner>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UpdateOfferBannerDto, OfferBanner>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Bulk Upload mappings
            CreateMap<BulkUploadLog, BulkUploadLogDto>();
            CreateMap<BulkUploadLog, BulkUploadResultDto>();
        }
    }
}
