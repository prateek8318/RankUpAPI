using AutoMapper;
using ExamService.Application.DTOs;
using ExamService.Application.Interfaces;
using ExamService.Domain.Entities;

namespace ExamService.Application.Services
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepository;
        private readonly IExamQualificationRepository _examQualificationRepository;
        private readonly IMapper _mapper;

        public ExamService(
            IExamRepository examRepository,
            IExamQualificationRepository examQualificationRepository,
            IMapper mapper)
        {
            _examRepository = examRepository;
            _examQualificationRepository = examQualificationRepository;
            _mapper = mapper;
        }

        public async Task<ExamDto> CreateExamAsync(CreateExamDto createDto)
        {
            var exam = _mapper.Map<Exam>(createDto);
            exam.CreatedAt = DateTime.UtcNow;
            exam.IsActive = true;

            await _examRepository.AddAsync(exam);
            await _examRepository.SaveChangesAsync();

            // Handle qualification relationships
            if (createDto.QualificationIds?.Any() == true)
            {
                var examQualifications = createDto.QualificationIds.Select(qId => new ExamQualification
                {
                    ExamId = exam.Id,
                    QualificationId = qId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }).ToList();

                await _examQualificationRepository.AddRangeAsync(examQualifications);
                await _examQualificationRepository.SaveChangesAsync();
            }

            var result = _mapper.Map<ExamDto>(exam);
            result.QualificationIds = createDto.QualificationIds ?? new List<int>();
            return result;
        }

        public async Task<ExamDto?> UpdateExamAsync(int id, UpdateExamDto updateDto)
        {
            var exam = await _examRepository.GetByIdWithQualificationsAsync(id);
            if (exam == null)
                return null;

            _mapper.Map(updateDto, exam);
            exam.UpdatedAt = DateTime.UtcNow;
            await _examRepository.UpdateAsync(exam);

            // Update qualification relationships
            if (updateDto.QualificationIds != null)
            {
                var existingQualificationIds = exam.ExamQualifications?.Select(eq => eq.QualificationId).ToList() ?? new List<int>();
                var qualificationsToRemove = exam.ExamQualifications?
                    .Where(eq => !updateDto.QualificationIds.Contains(eq.QualificationId))
                    .ToList() ?? new List<ExamQualification>();

                foreach (var eq in qualificationsToRemove)
                {
                    await _examQualificationRepository.DeleteAsync(eq);
                }

                foreach (var qualificationId in updateDto.QualificationIds.Except(existingQualificationIds))
                {
                    await _examQualificationRepository.AddAsync(new ExamQualification
                    {
                        ExamId = exam.Id,
                        QualificationId = qualificationId,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                }

                await _examQualificationRepository.SaveChangesAsync();
            }

            await _examRepository.SaveChangesAsync();
            var updatedExam = await _examRepository.GetByIdWithQualificationsAsync(id);
            var result = _mapper.Map<ExamDto>(updatedExam);
            result.QualificationIds = updatedExam?.ExamQualifications?.Select(eq => eq.QualificationId).ToList() ?? new List<int>();
            return result;
        }

        public async Task<bool> DeleteExamAsync(int id)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null)
                return false;

            await _examQualificationRepository.DeleteByExamIdAsync(id);
            await _examRepository.DeleteAsync(exam);
            await _examRepository.SaveChangesAsync();
            return true;
        }

        public async Task<ExamDto?> GetExamByIdAsync(int id)
        {
            var exam = await _examRepository.GetByIdWithQualificationsAsync(id);
            if (exam == null)
                return null;

            var examDto = _mapper.Map<ExamDto>(exam);
            examDto.QualificationIds = exam.ExamQualifications?.Select(eq => eq.QualificationId).ToList() ?? new List<int>();
            return examDto;
        }

        public async Task<IEnumerable<ExamDto>> GetAllExamsAsync()
        {
            var exams = await _examRepository.GetActiveAsync();
            var examDtos = new List<ExamDto>();

            foreach (var exam in exams.OrderBy(e => e.Name))
            {
                var examWithQuals = await _examRepository.GetByIdWithQualificationsAsync(exam.Id);
                var examDto = _mapper.Map<ExamDto>(examWithQuals ?? exam);
                examDto.QualificationIds = examWithQuals?.ExamQualifications?.Select(eq => eq.QualificationId).ToList() ?? new List<int>();
                examDtos.Add(examDto);
            }

            return examDtos;
        }

        public async Task<IEnumerable<ExamDto>> GetExamsByQualificationAsync(int qualificationId)
        {
            var exams = await _examRepository.GetByQualificationIdAsync(qualificationId);
            var examDtos = new List<ExamDto>();

            foreach (var exam in exams.OrderBy(e => e.Name))
            {
                var examWithQuals = await _examRepository.GetByIdWithQualificationsAsync(exam.Id);
                var examDto = _mapper.Map<ExamDto>(examWithQuals ?? exam);
                examDto.QualificationIds = examWithQuals?.ExamQualifications?.Select(eq => eq.QualificationId).ToList() ?? new List<int>();
                examDtos.Add(examDto);
            }

            return examDtos;
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
