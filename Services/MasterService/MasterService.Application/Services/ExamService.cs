using AutoMapper;
using MasterService.Application.DTOs;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;

namespace MasterService.Application.Services
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepository;
        private readonly IMapper _mapper;

        public ExamService(IExamRepository examRepository, IMapper mapper)
        {
            _examRepository = examRepository;
            _mapper = mapper;
        }

        public async Task<ExamDto> CreateExamAsync(CreateExamDto createDto)
        {
            var exam = _mapper.Map<Exam>(createDto);
            exam.CreatedAt = DateTime.UtcNow;
            exam.IsActive = true;

            if (createDto.Names != null && createDto.Names.Any())
            {
                foreach (var langDto in createDto.Names)
                {
                    exam.ExamLanguages.Add(new ExamLanguage
                    {
                        LanguageId = langDto.LanguageId,
                        Name = langDto.Name,
                        Description = langDto.Description,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                }
            }

            if (createDto.QualificationIds != null && createDto.QualificationIds.Any())
            {
                var streamIds = createDto.StreamIds ?? new List<int?>();
                for (int i = 0; i < createDto.QualificationIds.Count; i++)
                {
                    var qualificationId = createDto.QualificationIds[i];
                    var streamId = i < streamIds.Count ? streamIds[i] : null;

                    exam.ExamQualifications.Add(new ExamQualification
                    {
                        QualificationId = qualificationId,
                        StreamId = streamId,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                }
            }

            await _examRepository.AddAsync(exam);
            await _examRepository.SaveChangesAsync();

            return (await GetExamByIdAsync(exam.Id))!;
        }

        public async Task<ExamDto?> UpdateExamAsync(int id, UpdateExamDto updateDto)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null)
                return null;

            exam.Name = updateDto.Name;
            exam.Description = updateDto.Description;
            exam.CountryCode = updateDto.CountryCode;
            exam.MinAge = updateDto.MinAge;
            exam.MaxAge = updateDto.MaxAge;
            exam.ImageUrl = updateDto.ImageUrl;
            exam.UpdatedAt = DateTime.UtcNow;

            if (updateDto.Names != null && updateDto.Names.Any())
            {
                var existingLanguages = exam.ExamLanguages.ToList();
                foreach (var existingLang in existingLanguages)
                {
                    exam.ExamLanguages.Remove(existingLang);
                }

                foreach (var langDto in updateDto.Names)
                {
                    exam.ExamLanguages.Add(new ExamLanguage
                    {
                        LanguageId = langDto.LanguageId,
                        Name = langDto.Name,
                        Description = langDto.Description,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                }
            }

            if (updateDto.QualificationIds != null)
            {
                var existingRelations = exam.ExamQualifications.ToList();
                foreach (var rel in existingRelations)
                {
                    exam.ExamQualifications.Remove(rel);
                }

                var streamIds = updateDto.StreamIds ?? new List<int?>();
                for (int i = 0; i < updateDto.QualificationIds.Count; i++)
                {
                    var qualificationId = updateDto.QualificationIds[i];
                    var streamId = i < streamIds.Count ? streamIds[i] : null;

                    exam.ExamQualifications.Add(new ExamQualification
                    {
                        QualificationId = qualificationId,
                        StreamId = streamId,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                }
            }

            await _examRepository.UpdateAsync(exam);
            await _examRepository.SaveChangesAsync();
            return await GetExamByIdAsync(exam.Id);
        }

        public async Task<bool> DeleteExamAsync(int id)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null)
                return false;

            await _examRepository.DeleteAsync(exam);
            await _examRepository.SaveChangesAsync();
            return true;
        }

        public async Task<ExamDto?> GetExamByIdAsync(int id, int? languageId = null)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null)
                return null;

            var dto = _mapper.Map<ExamDto>(exam);
            if (languageId.HasValue)
            {
                var langName = exam.ExamLanguages
                    .FirstOrDefault(el => el.LanguageId == languageId.Value && el.IsActive)?.Name;
                if (!string.IsNullOrEmpty(langName))
                    dto.Name = langName;

                var langDesc = exam.ExamLanguages
                    .FirstOrDefault(el => el.LanguageId == languageId.Value && el.IsActive)?.Description;
                if (langDesc != null)
                    dto.Description = langDesc;
            }

            return dto;
        }

        public async Task<IEnumerable<ExamDto>> GetAllExamsAsync(int? languageId = null)
        {
            var exams = await _examRepository.GetActiveAsync();
            var dtos = exams.OrderBy(e => e.Name)
                .Select(e => _mapper.Map<ExamDto>(e))
                .ToList();

            if (languageId.HasValue)
            {
                foreach (var dto in dtos)
                {
                    var exam = exams.FirstOrDefault(x => x.Id == dto.Id);
                    if (exam != null)
                    {
                        var langName = exam.ExamLanguages
                            .FirstOrDefault(el => el.LanguageId == languageId.Value && el.IsActive)?.Name;
                        if (!string.IsNullOrEmpty(langName))
                            dto.Name = langName;

                        var langDesc = exam.ExamLanguages
                            .FirstOrDefault(el => el.LanguageId == languageId.Value && el.IsActive)?.Description;
                        if (langDesc != null)
                            dto.Description = langDesc;
                    }
                }
            }

            return dtos;
        }

        public async Task<IEnumerable<ExamDto>> GetExamsByFilterAsync(string? countryCode, int? qualificationId, int? streamId, int? minAge, int? maxAge, int? languageId = null)
        {
            var exams = await _examRepository.GetByFilterAsync(countryCode, qualificationId, streamId, minAge, maxAge);
            var dtos = exams.OrderBy(e => e.Name)
                .Select(e => _mapper.Map<ExamDto>(e))
                .ToList();

            if (languageId.HasValue)
            {
                foreach (var dto in dtos)
                {
                    var exam = exams.FirstOrDefault(x => x.Id == dto.Id);
                    if (exam != null)
                    {
                        var langName = exam.ExamLanguages
                            .FirstOrDefault(el => el.LanguageId == languageId.Value && el.IsActive)?.Name;
                        if (!string.IsNullOrEmpty(langName))
                            dto.Name = langName;

                        var langDesc = exam.ExamLanguages
                            .FirstOrDefault(el => el.LanguageId == languageId.Value && el.IsActive)?.Description;
                        if (langDesc != null)
                            dto.Description = langDesc;
                    }
                }
            }

            return dtos;
        }

        public async Task<bool> ToggleExamStatusAsync(int id, bool isActive)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null)
                return false;

            exam.IsActive = isActive;
            exam.UpdatedAt = DateTime.UtcNow;

            await _examRepository.UpdateAsync(exam);
            await _examRepository.SaveChangesAsync();
            return true;
        }
    }
}

