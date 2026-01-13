using AutoMapper;
using PaymentService.Application.DTOs;
using PaymentService.Domain.Entities;

namespace PaymentService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreatePaymentDto, Payment>();
            CreateMap<Payment, PaymentDto>();
        }
    }
}
