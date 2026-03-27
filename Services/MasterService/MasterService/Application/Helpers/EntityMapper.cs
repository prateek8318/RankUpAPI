using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                SubjectLanguages = subject.SubjectLanguages?.Select(lang => new SubjectLanguageDto
                {
                    Id = lang.Id,
                    SubjectId = lang.SubjectId,
                    LanguageId = lang.LanguageId,
                    Name = lang.Name,
                    Description = lang.Description,
                    IsActive = lang.IsActive
                }).ToList() ?? new List<SubjectLanguageDto>()
            };
        }

        public static SubjectListDto MapToSubjectListDto(Subject subject)
        {
            return new SubjectListDto
            {
                Id = subject.Id,
                Name = subject.Name,
                IsActive = subject.IsActive,
                SubjectLanguages = subject.SubjectLanguages?.Select(lang => new SubjectLanguageListDto
                {
                    LanguageId = lang.LanguageId,
                    Name = lang.Name,
                    IsActive = lang.IsActive
                }).ToList() ?? new List<SubjectLanguageListDto>()
            };
        }

        public static StreamDto MapToStreamDto(StreamEntity stream)
        {
            return new StreamDto
            {
                Id = stream.Id,
                Name = stream.Name,
                Description = stream.Description,
                QualificationId = stream.QualificationId,
                IsActive = stream.IsActive,
                CreatedAt = stream.CreatedAt,
                UpdatedAt = stream.UpdatedAt,
                StreamLanguages = stream.StreamLanguages?.Select(lang => new StreamLanguageDto
                {
                    Id = lang.Id,
                    StreamId = lang.StreamId,
                    LanguageId = lang.LanguageId,
                    Name = lang.Name,
                    Description = lang.Description,
                    IsActive = lang.IsActive
                }).ToList() ?? new List<StreamLanguageDto>()
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
                QualificationLanguages = qualification.QualificationLanguages?.Select(lang => new QualificationLanguageDto
                {
                    Id = lang.Id,
                    QualificationId = lang.QualificationId,
                    LanguageId = lang.LanguageId,
                    Name = lang.Name,
                    Description = lang.Description,
                    IsActive = lang.IsActive
                }).ToList() ?? new List<QualificationLanguageDto>()
            };
        }

        public static ExamDto MapToExamDto(Exam exam)
        {
            return new ExamDto
            {
                Id = exam.Id,
                Name = exam.Name,
                Description = exam.Description,
                IsActive = exam.IsActive,
                CreatedAt = exam.CreatedAt,
                UpdatedAt = exam.UpdatedAt,
                ExamLanguages = exam.ExamLanguages?.Select(lang => new ExamLanguageDto
                {
                    Id = lang.Id,
                    ExamId = lang.ExamId,
                    LanguageId = lang.LanguageId,
                    Name = lang.Name,
                    Description = lang.Description,
                    IsActive = lang.IsActive
                }).ToList() ?? new List<ExamLanguageDto>(),
                ExamQualifications = exam.ExamQualifications?.Select(req => new ExamQualificationDto
                {
                    Id = req.Id,
                    ExamId = req.ExamId,
                    QualificationId = req.QualificationId,
                    StreamId = req.StreamId
                }).ToList() ?? new List<ExamQualificationDto>()
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
                StateLanguages = state.StateLanguages?.Select(lang => new StateLanguageDto
                {
                    Id = lang.Id,
                    StateId = lang.StateId,
                    LanguageId = lang.LanguageId,
                    Name = lang.Name,
                    IsActive = lang.IsActive
                }).ToList() ?? new List<StateLanguageDto>()
            };
        }
    }
}
