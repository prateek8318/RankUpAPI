using AutoMapper;
using MasterService.Application.DTOs;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;

namespace MasterService.Application.Services
{
    public class QualificationService : IQualificationService
    {
        private readonly IQualificationRepository _qualificationRepository;
        private readonly IMapper _mapper;

        public QualificationService(IQualificationRepository qualificationRepository, IMapper mapper)
        {
            _qualificationRepository = qualificationRepository;
            _mapper = mapper;
        }

        public async Task<QualificationDto> CreateQualificationAsync(CreateQualificationDto createDto)
        {
            var qualification = _mapper.Map<Qualification>(createDto);
            qualification.CreatedAt = DateTime.UtcNow;
            qualification.IsActive = true;

            if (createDto.Names != null && createDto.Names.Any())
            {
                foreach (var langDto in createDto.Names)
                {
                    qualification.QualificationLanguages.Add(new QualificationLanguage
                    {
                        LanguageId = langDto.LanguageId,
                        Name = langDto.Name,
                        Description = langDto.Description,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                }
            }

            await _qualificationRepository.AddAsync(qualification);
            await _qualificationRepository.SaveChangesAsync();
            return (await GetQualificationByIdAsync(qualification.Id))!;
        }

        public async Task<QualificationDto?> UpdateQualificationAsync(int id, UpdateQualificationDto updateDto)
        {
            var qualification = await _qualificationRepository.GetByIdAsync(id);
            if (qualification == null)
                return null;

            qualification.Name = updateDto.Name;
            qualification.Description = updateDto.Description;
            qualification.CountryCode = updateDto.CountryCode;
            qualification.UpdatedAt = DateTime.UtcNow;

            if (updateDto.Names != null && updateDto.Names.Any())
            {
                var existingLanguages = qualification.QualificationLanguages.ToList();
                foreach (var existingLang in existingLanguages)
                {
                    qualification.QualificationLanguages.Remove(existingLang);
                }
                foreach (var langDto in updateDto.Names)
                {
                    qualification.QualificationLanguages.Add(new QualificationLanguage
                    {
                        LanguageId = langDto.LanguageId,
                        Name = langDto.Name,
                        Description = langDto.Description,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                }
            }

            await _qualificationRepository.UpdateAsync(qualification);
            await _qualificationRepository.SaveChangesAsync();
            return await GetQualificationByIdAsync(qualification.Id);
        }

        public async Task<bool> DeleteQualificationAsync(int id)
        {
            var qualification = await _qualificationRepository.GetByIdAsync(id);
            if (qualification == null)
                return false;
            await _qualificationRepository.DeleteAsync(qualification);
            await _qualificationRepository.SaveChangesAsync();
            return true;
        }

        public async Task<QualificationDto?> GetQualificationByIdAsync(int id, int? languageId = null)
        {
            var qualification = await _qualificationRepository.GetByIdAsync(id);
            if (qualification == null)
                return null;

            var dto = _mapper.Map<QualificationDto>(qualification);
            if (languageId.HasValue)
            {
                var langName = qualification.QualificationLanguages
                    .FirstOrDefault(ql => ql.LanguageId == languageId.Value && ql.IsActive)?.Name;
                if (!string.IsNullOrEmpty(langName))
                    dto.Name = langName;
                var langDesc = qualification.QualificationLanguages
                    .FirstOrDefault(ql => ql.LanguageId == languageId.Value && ql.IsActive)?.Description;
                if (langDesc != null)
                    dto.Description = langDesc;
            }
            return dto;
        }

        public async Task<IEnumerable<QualificationDto>> GetAllQualificationsAsync(int? languageId = null)
        {
            var qualifications = await _qualificationRepository.GetActiveAsync();
            var dtos = qualifications.OrderBy(q => q.Name).Select(q => _mapper.Map<QualificationDto>(q)).ToList();
            if (languageId.HasValue)
            {
                foreach (var dto in dtos)
                {
                    var q = qualifications.FirstOrDefault(x => x.Id == dto.Id);
                    if (q != null)
                    {
                        var langName = q.QualificationLanguages.FirstOrDefault(ql => ql.LanguageId == languageId.Value && ql.IsActive)?.Name;
                        if (!string.IsNullOrEmpty(langName))
                            dto.Name = langName;
                        var langDesc = q.QualificationLanguages.FirstOrDefault(ql => ql.LanguageId == languageId.Value && ql.IsActive)?.Description;
                        if (langDesc != null)
                            dto.Description = langDesc;
                    }
                }
            }
            return dtos;
        }

        public async Task<IEnumerable<QualificationDto>> GetQualificationsByCountryCodeAsync(string countryCode, int? languageId = null)
        {
            var qualifications = await _qualificationRepository.GetActiveByCountryCodeAsync(countryCode);
            var dtos = qualifications.OrderBy(q => q.Name).Select(q => _mapper.Map<QualificationDto>(q)).ToList();
            if (languageId.HasValue)
            {
                foreach (var dto in dtos)
                {
                    var q = qualifications.FirstOrDefault(x => x.Id == dto.Id);
                    if (q != null)
                    {
                        var langName = q.QualificationLanguages.FirstOrDefault(ql => ql.LanguageId == languageId.Value && ql.IsActive)?.Name;
                        if (!string.IsNullOrEmpty(langName))
                            dto.Name = langName;
                        var langDesc = q.QualificationLanguages.FirstOrDefault(ql => ql.LanguageId == languageId.Value && ql.IsActive)?.Description;
                        if (langDesc != null)
                            dto.Description = langDesc;
                    }
                }
            }
            return dtos;
        }

        public async Task<bool> ToggleQualificationStatusAsync(int id, bool isActive)
        {
            var qualification = await _qualificationRepository.GetByIdAsync(id);
            if (qualification == null)
                return false;
            qualification.IsActive = isActive;
            qualification.UpdatedAt = DateTime.UtcNow;
            await _qualificationRepository.UpdateAsync(qualification);
            await _qualificationRepository.SaveChangesAsync();
            return true;
        }
    }
}
