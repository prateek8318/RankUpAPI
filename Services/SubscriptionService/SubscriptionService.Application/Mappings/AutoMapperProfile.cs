using AutoMapper;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Application.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // SubscriptionPlan mappings
            CreateMap<SubscriptionPlanTranslation, SubscriptionPlanTranslationDto>();
            CreateMap<SubscriptionPlan, SubscriptionPlanDto>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            CreateMap<SubscriptionPlan, SubscriptionPlanListDto>();

            CreateMap<CreateSubscriptionPlanDto, SubscriptionPlan>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Translations, opt => opt.Ignore())
                .ForMember(dest => dest.ValidityDays, opt => opt.Ignore()); // set in service from Duration + DurationType

            CreateMap<UpdateSubscriptionPlanDto, SubscriptionPlan>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Translations, opt => opt.Ignore());

            // UserSubscription mappings
            CreateMap<UserSubscription, UserSubscriptionDto>()
                .ForMember(dest => dest.DaysUntilExpiry, opt => opt.MapFrom(src => 
                    src.ValidTill > DateTime.UtcNow ? (int)(src.ValidTill - DateTime.UtcNow).TotalDays : 0))
                .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => 
                    src.ValidTill <= DateTime.UtcNow && src.Status != "Cancelled"));

            CreateMap<CreateUserSubscriptionDto, UserSubscription>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Active"))
                .ForMember(dest => dest.PurchasedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.ValidTill, opt => opt.MapFrom(src => DateTime.UtcNow.AddDays(30)))
                .ForMember(dest => dest.AutoRenewal, opt => opt.MapFrom(src => false));

            // PaymentTransaction mappings
            CreateMap<PaymentTransaction, PaymentTransactionDto>();

            // Invoice mappings
            CreateMap<Invoice, InvoiceDto>();

            // DemoAccessLog mappings
            CreateMap<DemoAccessLog, DemoAccessLogDto>();

            // Reverse mappings for DTOs to entities
            CreateMap<PaymentTransactionDto, PaymentTransaction>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<InvoiceDto, Invoice>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // PlanDurationOption mappings
            CreateMap<PlanDurationOption, PlanDurationOptionDto>();

            CreateMap<CreatePlanDurationOptionDto, PlanDurationOption>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<UpdatePlanDurationOptionDto, PlanDurationOption>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // PlanWithDurationOptions mappings
            CreateMap<SubscriptionPlan, PlanWithDurationOptionsDto>()
                .ForMember(dest => dest.LocalizedName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.LocalizedDescription, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.LocalizedFeatures, opt => opt.MapFrom(src => src.Features));

            CreateMap<CreateSubscriptionPlanWithDurationDto, SubscriptionPlan>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Translations, opt => opt.Ignore())
                .ForMember(dest => dest.DurationOptions, opt => opt.Ignore())
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.BasePrice))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => 1))
                .ForMember(dest => dest.DurationType, opt => opt.MapFrom(src => "Monthly"))
                .ForMember(dest => dest.ValidityDays, opt => opt.MapFrom(src => 30));
        }
    }

    // Additional DTO for DemoAccessLog
    public class DemoAccessLogDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ExamCategory { get; set; } = string.Empty;
        public DateTime AccessDate { get; set; }
        public int QuestionsAttempted { get; set; }
        public int TimeSpentMinutes { get; set; }
        public string? IPAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? DeviceType { get; set; }
        public bool IsCompleted { get; set; }
        public string? AccessDetails { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
