using System.Collections.Generic;
using MasterService.Application.DTOs;
using MasterService.Domain.Entities;

namespace MasterService.Application.Helpers
{
    public static class EntityMapper
    {
        public static SubjectDto MapToSubjectDto(Subject subject)
        {
            return new SubjectDto
            {
                Id = subject.Id,
                Name = subject.Name,
                Description = subject.Description,
                IsActive = subject.IsActive,
                CreatedAt = subject.CreatedAt,
                UpdatedAt = subject.UpdatedAt,
                SubjectLanguages = new List<SubjectLanguageDto>()
            };
        }

        public static QualificationDto MapToQualificationDto(Qualification qualification)
        {
            return new QualificationDto
            {
                Id = qualification.Id,
                Name = qualification.Name,
                Description = qualification.Description,
                IsActive = qualification.IsActive,
                CreatedAt = qualification.CreatedAt,
                UpdatedAt = qualification.UpdatedAt,
                Names = new List<QualificationLanguageDto>()
            };
        }

        public static StateDto MapToStateDto(State state)
        {
            return new StateDto
            {
                Id = state.Id,
                Name = state.Name,
                IsActive = state.IsActive,
                CreatedAt = state.CreatedAt,
                UpdatedAt = state.UpdatedAt,
                Names = new List<StateLanguageDto>()
            };
        }
    }
}
