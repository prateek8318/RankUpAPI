using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RankUpAPI.Data;
using RankUpAPI.DTOs;
using RankUpAPI.Models;
using RankUpAPI.Repositories.Interfaces;
using RankUpAPI.Services.Interfaces;

namespace RankUpAPI.Services
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepository;
        private readonly IQualificationRepository _qualificationRepository;
        private readonly IExamQualificationRepository _examQualificationRepository;
        private readonly IMapper _mapper;

        public ExamService(IExamRepository examRepository, IQualificationRepository qualificationRepository, IExamQualificationRepository examQualificationRepository, IMapper mapper)
        {
            _examRepository = examRepository;
            _qualificationRepository = qualificationRepository;
            _examQualificationRepository = examQualificationRepository;
            _mapper = mapper;
        }

        public async Task<ExamDto> CreateExamAsync(CreateExamDto createDto)
        {
            var exam = _mapper.Map<Exam>(createDto);
            exam.CreatedAt = DateTime.UtcNow;
            exam.IsActive = true;

            // Add the exam first to get the ID
            await _examRepository.AddAsync(exam);
            await _examRepository.SaveChangesAsync();

            // Now handle the many-to-many relationships
            if (createDto.QualificationIds?.Any() == true)
            {
                var examQualifications = new List<ExamQualification>();
                foreach (var qualificationId in createDto.QualificationIds)
                {
                    var qualification = await _qualificationRepository.GetByIdAsync(qualificationId);
                    if (qualification != null)
                    {
                        examQualifications.Add(new ExamQualification
                        {
                            ExamId = exam.Id,
                            QualificationId = qualificationId
                        });
                    }
                }
                if (examQualifications.Any())
                {
                    await _examQualificationRepository.AddRangeAsync(examQualifications);
                    await _examQualificationRepository.SaveChangesAsync();
                }
            }

            var result = _mapper.Map<ExamDto>(exam);
            result.QualificationIds = createDto.QualificationIds ?? new List<int>();
            return result;
        }

        public async Task<bool> DeleteExamAsync(int id)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null)
                return false;

            // Remove related exam qualifications
            await _examQualificationRepository.DeleteByExamIdAsync(id);

            await _examRepository.DeleteAsync(exam);
            await _examRepository.SaveChangesAsync();
            return true;
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

        public async Task<ExamDto?> GetExamByIdAsync(int id)
        {
            var exam = await _examRepository.GetByIdWithQualificationsAsync(id);
                
            if (exam == null)
                return null;
                
            var examDto = _mapper.Map<ExamDto>(exam);
            examDto.QualificationIds = exam.ExamQualifications?.Select(eq => eq.QualificationId).ToList() ?? new List<int>();
            return examDto;
        }

        public async Task<bool> ToggleExamStatusAsync(int id, bool isActive)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null)
                return false;

            exam.IsActive = isActive;
            await _examRepository.UpdateAsync(exam);
            await _examRepository.SaveChangesAsync();
            return true;
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

        public async Task<ExamDto?> UpdateExamAsync(int id, UpdateExamDto updateDto)
        {
            var exam = await _examRepository.GetByIdWithQualificationsAsync(id);
            if (exam == null)
                return null;
            // Update exam properties
            _mapper.Map(updateDto, exam);
            // Update exam properties
            await _examRepository.UpdateAsync(exam);
            
            // Update many-to-many relationships
            if (updateDto.QualificationIds != null)
            {
                // Remove existing relationships not in the new list
                var existingQualificationIds = exam.ExamQualifications?.Select(eq => eq.QualificationId).ToList() ?? new List<int>();
                var qualificationsToRemove = exam.ExamQualifications?
                    .Where(eq => !updateDto.QualificationIds.Contains(eq.QualificationId))
                    .ToList() ?? new List<ExamQualification>();
                foreach (var eq in qualificationsToRemove)
                {
                    await _examQualificationRepository.DeleteAsync(eq);
                }
                // Add new relationships
                foreach (var qualificationId in updateDto.QualificationIds.Except(existingQualificationIds))
                {
                    var qualification = await _qualificationRepository.GetByIdAsync(qualificationId);
                    if (qualification != null)
                    {
                        await _examQualificationRepository.AddAsync(new ExamQualification
                        {
                            ExamId = exam.Id,
                            QualificationId = qualificationId
                        });
                    }
                }
                // Save changes for ExamQualification updates
                await _examQualificationRepository.SaveChangesAsync();
            }
            await _examRepository.SaveChangesAsync();
            var result = _mapper.Map<ExamDto>(exam);
            var updatedExam = await _examRepository.GetByIdWithQualificationsAsync(id);
            result.QualificationIds = updatedExam?.ExamQualifications?.Select(eq => eq.QualificationId).ToList() ?? new List<int>();
            return result;
        }
    }
}
