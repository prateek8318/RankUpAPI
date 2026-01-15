using AutoMapper;
using HomeDashboardService.Application.DTOs;
using HomeDashboardService.Application.Interfaces;
using HomeDashboardService.Domain.Entities;
using HomeDashboardService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace HomeDashboardService.Application.Services
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ExamService> _logger;

        public ExamService(
            IExamRepository examRepository,
            IMapper mapper,
            ILogger<ExamService> logger)
        {
            _examRepository = examRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ExamDto> CreateExamAsync(CreateExamDto createExamDto)
        {
            try
            {
                var exam = _mapper.Map<Exam>(createExamDto);
                var created = await _examRepository.AddAsync(exam);
                await _examRepository.SaveChangesAsync();
                return _mapper.Map<ExamDto>(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating exam");
                throw;
            }
        }

        public async Task<ExamDto?> UpdateExamAsync(int id, UpdateExamDto updateExamDto)
        {
            try
            {
                var exam = await _examRepository.GetByIdAsync(id);
                if (exam == null)
                    return null;

                _mapper.Map(updateExamDto, exam);
                exam.UpdatedAt = DateTime.UtcNow;
                await _examRepository.UpdateAsync(exam);
                await _examRepository.SaveChangesAsync();
                return _mapper.Map<ExamDto>(exam);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating exam: {ExamId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteExamAsync(int id)
        {
            try
            {
                return await _examRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting exam: {ExamId}", id);
                throw;
            }
        }

        public async Task<bool> EnableDisableExamAsync(int id, bool isActive)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling/disabling exam: {ExamId}", id);
                throw;
            }
        }

        public async Task<ExamDto?> GetExamByIdAsync(int id)
        {
            try
            {
                var exam = await _examRepository.GetByIdWithSubjectsAsync(id);
                return exam != null ? _mapper.Map<ExamDto>(exam) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting exam: {ExamId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ExamDto>> GetAllExamsAsync()
        {
            try
            {
                var exams = await _examRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<ExamDto>>(exams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all exams");
                throw;
            }
        }

        public async Task<IEnumerable<ExamDto>> GetActiveExamsAsync()
        {
            try
            {
                var exams = await _examRepository.GetActiveExamsAsync();
                return _mapper.Map<IEnumerable<ExamDto>>(exams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active exams");
                throw;
            }
        }
    }
}
