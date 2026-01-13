using AutoMapper;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateSubscriptionDto, Subscription>();
            CreateMap<Subscription, SubscriptionDto>();
        }
    }
}
