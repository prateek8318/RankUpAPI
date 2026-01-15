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
            CreateMap<SubscriptionPlan, SubscriptionPlanDto>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            CreateMap<SubscriptionPlan, SubscriptionPlanListDto>();

            CreateMap<CreateSubscriptionPlanDto, SubscriptionPlan>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<UpdateSubscriptionPlanDto, SubscriptionPlan>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // UserSubscription mappings
            CreateMap<UserSubscription, UserSubscriptionDto>()
                .ForMember(dest => dest.DaysUntilExpiry, opt => opt.MapFrom(src => 
                    src.EndDate > DateTime.UtcNow ? (int)(src.EndDate - DateTime.UtcNow).TotalDays : 0))
                .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => 
                    src.EndDate <= DateTime.UtcNow && src.Status != SubscriptionStatus.Cancelled));

            CreateMap<CreateUserSubscriptionDto, UserSubscription>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => SubscriptionStatus.Pending))
                .ForMember(dest => dest.StartDate, opt => opt.Ignore())
                .ForMember(dest => dest.EndDate, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

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
