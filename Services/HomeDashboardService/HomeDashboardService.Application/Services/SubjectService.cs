using AutoMapper;
using HomeDashboardService.Application.DTOs;
using HomeDashboardService.Application.Interfaces;
using HomeDashboardService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace HomeDashboardService.Application.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly IExamRepository _examRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SubjectService> _logger;

        public SubjectService(
            ISubjectRepository subjectRepository,
            IExamRepository examRepository,
            IMapper mapper,
            ILogger<SubjectService> logger)
        {
            _subjectRepository = subjectRepository;
            _examRepository = examRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto createSubjectDto)
        {
            try
            {
                var exam = await _examRepository.GetByIdAsync(createSubjectDto.ExamId);
                if (exam == null)
                    throw new KeyNotFoundException($"Exam with ID {createSubjectDto.ExamId} not found");

                var subject = _mapper.Map<Domain.Entities.Subject>(createSubjectDto);
                var created = await _subjectRepository.AddAsync(subject);
                await _subjectRepository.SaveChangesAsync();
                return _mapper.Map<SubjectDto>(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subject");
                throw;
            }
        }

        public async Task<SubjectDto?> UpdateSubjectAsync(int id, UpdateSubjectDto updateSubjectDto)
        {
            try
            {
                var subject = await _subjectRepository.GetByIdWithChaptersAsync(id);
                if (subject == null)
                    return null;

                _mapper.Map(updateSubjectDto, subject);
                subject.UpdatedAt = DateTime.UtcNow;
                await _subjectRepository.UpdateAsync(subject);
                await _subjectRepository.SaveChangesAsync();
                return _mapper.Map<SubjectDto>(subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subject: {SubjectId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteSubjectAsync(int id)
        {
            try
            {
                return await _subjectRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subject: {SubjectId}", id);
                throw;
            }
        }

        public async Task<bool> EnableDisableSubjectAsync(int id, bool isActive)
        {
            try
            {
                var subject = await _subjectRepository.GetByIdAsync(id);
                if (subject == null)
                    return false;

                subject.IsActive = isActive;
                subject.UpdatedAt = DateTime.UtcNow;
                await _subjectRepository.UpdateAsync(subject);
                await _subjectRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling/disabling subject: {SubjectId}", id);
                throw;
            }
        }

        public async Task<SubjectDto?> GetSubjectByIdAsync(int id)
        {
            try
            {
                var subject = await _subjectRepository.GetByIdWithChaptersAsync(id);
                return subject != null ? _mapper.Map<SubjectDto>(subject) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subject: {SubjectId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<SubjectDto>> GetSubjectsByExamIdAsync(int examId)
        {
            try
            {
                var subjects = await _subjectRepository.GetByExamIdAsync(examId);
                return _mapper.Map<IEnumerable<SubjectDto>>(subjects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subjects for exam: {ExamId}", examId);
                throw;
            }
        }
    }
}
