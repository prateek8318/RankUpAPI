using AutoMapper;
using MasterService.Application.DTOs;
using MasterService.Domain.Entities;
using StreamEntity = MasterService.Domain.Entities.Stream;

namespace MasterService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateLanguageDto, Language>();
            CreateMap<UpdateLanguageDto, Language>();
            CreateMap<Language, LanguageDto>();

            CreateMap<CreateStateDto, State>();
            CreateMap<UpdateStateDto, State>();
            CreateMap<State, StateDto>()
                .ForMember(dest => dest.Names, opt => opt.MapFrom(src => src.StateLanguages));

            CreateMap<StateLanguage, StateLanguageDto>()
                .ForMember(dest => dest.LanguageCode, opt => opt.MapFrom(src => src.Language.Code))
                .ForMember(dest => dest.LanguageName, opt => opt.MapFrom(src => src.Language.Name));

            CreateMap<CreateQualificationDto, Qualification>();
            CreateMap<UpdateQualificationDto, Qualification>();
            CreateMap<Qualification, QualificationDto>()
                .ForMember(dest => dest.Names, opt => opt.MapFrom(src => src.QualificationLanguages));
            CreateMap<QualificationLanguage, QualificationLanguageDto>()
                .ForMember(dest => dest.LanguageCode, opt => opt.MapFrom(src => src.Language.Code))
                .ForMember(dest => dest.LanguageName, opt => opt.MapFrom(src => src.Language.Name));

            CreateMap<CreateExamDto, Exam>();
            CreateMap<UpdateExamDto, Exam>();
            CreateMap<Exam, ExamDto>()
                .ForMember(dest => dest.Names, opt => opt.MapFrom(src => src.ExamLanguages))
                .ForMember(dest => dest.QualificationIds,
                    opt => opt.MapFrom(src => src.ExamQualifications.Select(eq => eq.QualificationId)))
                .ForMember(dest => dest.StreamIds,
                    opt => opt.MapFrom(src => src.ExamQualifications.Select(eq => eq.StreamId)));
            CreateMap<ExamLanguage, ExamLanguageDto>()
                .ForMember(dest => dest.LanguageCode, opt => opt.MapFrom(src => src.Language.Code))
                .ForMember(dest => dest.LanguageName, opt => opt.MapFrom(src => src.Language.Name));

            CreateMap<CreateStreamDto, StreamEntity>();
            CreateMap<UpdateStreamDto, StreamEntity>();
            CreateMap<StreamEntity, StreamDto>()
                .ForMember(dest => dest.QualificationName, opt => opt.MapFrom(src => src.Qualification != null ? src.Qualification.Name : null))
                .ForMember(dest => dest.Names, opt => opt.MapFrom(src => src.StreamLanguages));
            CreateMap<StreamLanguage, StreamLanguageDto>()
                .ForMember(dest => dest.LanguageCode, opt => opt.MapFrom(src => src.Language.Code))
                .ForMember(dest => dest.LanguageName, opt => opt.MapFrom(src => src.Language.Name));
        }
    }
}
