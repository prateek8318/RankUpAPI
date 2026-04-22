using AutoMapper;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User subscription mappings
            CreateMap<CreateSubscriptionDto, Subscription>();
            CreateMap<Subscription, SubscriptionDto>();
            
            // Subscription plan mappings
            CreateMap<SubscriptionPlan, SubscriptionPlanDto>()
                .ForMember(dest => dest.ExamType, opt => opt.MapFrom(src => src.ExamCategory));
            
            CreateMap<CreateSubscriptionPlanDto, SubscriptionPlan>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            
            CreateMap<UpdateSubscriptionPlanDto, SubscriptionPlan>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            
            // Subscription plan with duration options mappings
            CreateMap<SubscriptionPlan, PlanWithDurationOptionsDto>();
            
            CreateMap<PlanDurationOption, PlanDurationOptionDto>();
            
            CreateMap<CreatePlanDurationOptionDto, PlanDurationOption>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            
            CreateMap<UpdatePlanDurationOptionDto, PlanDurationOption>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            
            // Translation mappings
            CreateMap<SubscriptionPlanTranslation, SubscriptionPlanTranslationDto>();
            CreateMap<SubscriptionPlanTranslationDto, SubscriptionPlanTranslation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            
            // User subscription mappings
            CreateMap<UserSubscription, UserSubscriptionDto>()
                .ForMember(dest => dest.SubscriptionPlan, opt => opt.MapFrom(src => src.SubscriptionPlan));
            
            CreateMap<CreateUserSubscriptionDto, UserSubscription>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Pending"));
            
            // Payment transaction mappings
            CreateMap<PaymentTransaction, PaymentTransactionDto>();
            
            // Invoice mappings
            CreateMap<Invoice, InvoiceDto>();
        }
    }
}
