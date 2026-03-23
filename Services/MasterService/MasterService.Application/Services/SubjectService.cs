using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MasterService.Application.DTOs;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;

namespace MasterService.Application.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly ISubjectLanguageRepository _subjectLanguageRepository;
        private readonly ILogger<SubjectService> _logger;

        public SubjectService(
            ISubjectRepository subjectRepository,
            ISubjectLanguageRepository subjectLanguageRepository,
            ILogger<SubjectService> logger)
        {
            _subjectRepository = subjectRepository;
            _subjectLanguageRepository = subjectLanguageRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<SubjectDto>> GetAllSubjectsAsync()
        {
            try
            {
                var subjects = await _subjectRepository.GetAllAsync();
                return subjects.Select(MapToSubjectDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all subjects");
                throw;
            }
        }

        public async Task<SubjectDto> GetSubjectByIdAsync(int id)
        {
            try
            {
                var subject = await _subjectRepository.GetByIdAsync(id);
                return subject == null ? null : MapToSubjectDto(subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subject by id: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<SubjectListDto>> GetActiveSubjectsAsync()
        {
            try
            {
                var subjects = await _subjectRepository.GetActiveSubjectsAsync();
                return subjects.Select(MapToSubjectListDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active subjects");
                throw;
            }
        }

        public async Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto createSubjectDto)
        {
            try
            {
                var subject = new Subject
                {
                    Name = createSubjectDto.Name,
                    Description = createSubjectDto.Description,
                    IsActive = createSubjectDto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdSubject = await _subjectRepository.AddAsync(subject);

                // Add subject languages if provided
                if (createSubjectDto.SubjectLanguages != null && createSubjectDto.SubjectLanguages.Any())
                {
                    foreach (var langDto in createSubjectDto.SubjectLanguages)
                    {
                        var subjectLanguage = new SubjectLanguage
                        {
                            SubjectId = createdSubject.Id,
                            LanguageId = langDto.LanguageId,
                            Name = langDto.Name,
                            Description = langDto.Description,
                            IsActive = langDto.IsActive,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        await _subjectLanguageRepository.AddAsync(subjectLanguage);
                    }
                }

                return MapToSubjectDto(createdSubject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subject");
                throw;
            }
        }

        public async Task<SubjectDto> UpdateSubjectAsync(int id, UpdateSubjectDto updateSubjectDto)
        {
            try
            {
                var existingSubject = await _subjectRepository.GetByIdAsync(id);
                if (existingSubject == null)
                {
                    throw new ArgumentException($"Subject with id {id} not found");
                }

                // Update properties
                if (!string.IsNullOrEmpty(updateSubjectDto.Name))
                    existingSubject.Name = updateSubjectDto.Name;
                
                if (updateSubjectDto.Description != null)
                    existingSubject.Description = updateSubjectDto.Description;
                
                if (updateSubjectDto.IsActive.HasValue)
                    existingSubject.IsActive = updateSubjectDto.IsActive.Value;

                existingSubject.UpdatedAt = DateTime.UtcNow;

                var updatedSubject = await _subjectRepository.UpdateAsync(existingSubject);

                // Update subject languages if provided
                if (updateSubjectDto.SubjectLanguages != null && updateSubjectDto.SubjectLanguages.Any())
                {
                    var existingLanguages = await _subjectLanguageRepository.GetBySubjectIdAsync(id);
                    
                    foreach (var langDto in updateSubjectDto.SubjectLanguages)
                    {
                        var existingLang = existingLanguages.FirstOrDefault(l => l.LanguageId == langDto.LanguageId);
                        
                        if (existingLang != null)
                        {
                            // Update existing
                            if (!string.IsNullOrEmpty(langDto.Name))
                                existingLang.Name = langDto.Name;
                            
                            if (langDto.Description != null)
                                existingLang.Description = langDto.Description;
                            
                            if (langDto.IsActive.HasValue)
                                existingLang.IsActive = langDto.IsActive.Value;
                            
                            existingLang.UpdatedAt = DateTime.UtcNow;
                            await _subjectLanguageRepository.UpdateAsync(existingLang);
                        }
                    }
                }

                return MapToSubjectDto(updatedSubject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subject with id: {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteSubjectAsync(int id)
        {
            try
            {
                var subject = await _subjectRepository.GetByIdAsync(id);
                if (subject == null)
                {
                    return false;
                }

                await _subjectRepository.DeleteAsync(subject);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subject with id: {Id}", id);
                throw;
            }
        }

        public async Task<bool> SubjectExistsAsync(int id)
        {
            try
            {
                return await _subjectRepository.ExistsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if subject exists with id: {Id}", id);
                throw;
            }
        }

        private SubjectDto MapToSubjectDto(Subject subject)
        {
            return new SubjectDto
            {
                Id = subject.Id,
                Name = subject.Name,
                Description = subject.Description,
                IsActive = subject.IsActive,
                CreatedAt = subject.CreatedAt,
                UpdatedAt = subject.UpdatedAt,
                SubjectLanguages = subject.SubjectLanguages?.Select(MapToSubjectLanguageDto).ToList() ?? new List<SubjectLanguageDto>()
            };
        }

        private SubjectListDto MapToSubjectListDto(Subject subject)
        {
            return new SubjectListDto
            {
                Id = subject.Id,
                Name = subject.Name,
                Description = subject.Description,
                IsActive = subject.IsActive,
                CreatedAt = subject.CreatedAt
            };
        }

        private SubjectLanguageDto MapToSubjectLanguageDto(SubjectLanguage subjectLanguage)
        {
            return new SubjectLanguageDto
            {
                Id = subjectLanguage.Id,
                SubjectId = subjectLanguage.SubjectId,
                LanguageId = subjectLanguage.LanguageId,
                Name = subjectLanguage.Name,
                Description = subjectLanguage.Description,
                IsActive = subjectLanguage.IsActive,
                CreatedAt = subjectLanguage.CreatedAt,
                UpdatedAt = subjectLanguage.UpdatedAt,
                Language = subjectLanguage.Language != null ? new LanguageDto
                {
                    Id = subjectLanguage.Language.Id,
                    Name = subjectLanguage.Language.Name,
                    Code = subjectLanguage.Language.Code
                } : null
            };
        }
    }
}
