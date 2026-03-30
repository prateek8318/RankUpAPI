using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MasterService.Application.DTOs;
using MasterService.Application.Helpers;
using MasterService.Application.Interfaces;
using MasterService.Domain.Entities;
using Common.DTOs;

namespace MasterService.Application.Services
{
    public class SubjectService : BaseService, ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;

        public SubjectService(
            ISubjectRepository subjectRepository,
            ILogger<SubjectService> logger) : base(logger)
        {
            _subjectRepository = subjectRepository;
        }

        public async Task<IEnumerable<SubjectDto>> GetAllSubjectsAsync(int? languageId = null)
        {
            try
            {
                var subjects = await _subjectRepository.GetAllAsync(languageId);
                return subjects.Select(MapToSubjectDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all subjects");
                throw;
            }
        }

        public async Task<SubjectDto?> GetSubjectByIdAsync(int id, int? languageId = null)
        {
            try
            {
                var subject = await _subjectRepository.GetByIdAsync(id, languageId);
                return subject == null ? null : MapToSubjectDto(subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subject by id: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<SubjectListDto>> GetActiveSubjectsAsync(int? languageId = null)
        {
            try
            {
                var subjects = await _subjectRepository.GetActiveSubjectsAsync(languageId);
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
            return await ExecuteInTransactionAsync<SubjectDto>(
                _subjectRepository,
                async () =>
                {
                    var subject = new Subject
                    {
                        Name = createSubjectDto.Name,
                        Description = createSubjectDto.Description,
                        IsActive = createSubjectDto.IsActive,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    var namesJson = LanguagePayloadSerializer.SerializeNames(
                        createSubjectDto.SubjectLanguages,
                        lang => new
                        {
                            lang.LanguageId,
                            lang.Name,
                            lang.Description,
                            IsActive = lang.IsActive
                        });

                    var createdSubject = await _subjectRepository.AddAsync(subject, namesJson);
                    await _subjectRepository.SaveChangesAsync();
                    return (await GetSubjectByIdAsync(createdSubject.Id))!;
                },
                "CreateSubject");
        }

        public async Task<SubjectDto?> UpdateSubjectAsync(int id, UpdateSubjectDto updateSubjectDto)
        {
            return await ExecuteInTransactionAsync<SubjectDto?>(
                _subjectRepository,
                async () =>
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

                    var namesJson = updateSubjectDto.SubjectLanguages == null
                        ? null
                        : LanguagePayloadSerializer.SerializeNames(
                            updateSubjectDto.SubjectLanguages,
                            lang => new
                            {
                                lang.LanguageId,
                                Name = lang.Name ?? string.Empty,
                                lang.Description,
                                IsActive = lang.IsActive ?? true
                            });

                    await _subjectRepository.UpdateAsync(existingSubject, namesJson);
                    await _subjectRepository.SaveChangesAsync();
                    return await GetSubjectByIdAsync(id);
                },
                "UpdateSubject");
        }

        public async Task<bool> DeleteSubjectAsync(int id)
        {
            return await ExecuteInTransactionAsync<bool>(
                _subjectRepository,
                async () =>
                {
                    var subject = await _subjectRepository.GetByIdAsync(id);
                    if (subject == null)
                    {
                        return false;
                    }

                    await _subjectRepository.DeleteAsync(subject);
                    await _subjectRepository.SaveChangesAsync();

                    return true;
                },
                "DeleteSubject");
        }

        public async Task<bool> ToggleSubjectStatusAsync(int id, bool isActive)
        {
            try
            {
                var subject = await _subjectRepository.GetByIdAsync(id);
                if (subject == null)
                {
                    return false;
                }

                return await _subjectRepository.ToggleSubjectStatusAsync(id, isActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling subject status with id: {Id}", id);
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

        // Pagination support methods
        public async Task<PaginatedResponse<SubjectDto>> GetAllSubjectsPaginatedAsync(PaginationRequest pagination)
        {
            try
            {
                var paginatedSubjects = await _subjectRepository.GetAllAsync(pagination);
                var subjectDtos = paginatedSubjects.Data.Select(MapToSubjectDto);
                
                return new PaginatedResponse<SubjectDto>
                {
                    Data = subjectDtos,
                    TotalCount = paginatedSubjects.TotalCount,
                    PageNumber = paginatedSubjects.PageNumber,
                    PageSize = paginatedSubjects.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated subjects");
                throw;
            }
        }

        public async Task<PaginatedResponse<SubjectListDto>> GetActiveSubjectsPaginatedAsync(PaginationRequest pagination)
        {
            try
            {
                var paginatedSubjects = await _subjectRepository.GetActiveSubjectsAsync(pagination);
                var subjectDtos = paginatedSubjects.Data.Select(MapToSubjectListDto);
                
                return new PaginatedResponse<SubjectListDto>
                {
                    Data = subjectDtos,
                    TotalCount = paginatedSubjects.TotalCount,
                    PageNumber = paginatedSubjects.PageNumber,
                    PageSize = paginatedSubjects.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated active subjects");
                throw;
            }
        }
    }
}
