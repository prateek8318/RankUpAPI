using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RankUpAPI.Data;
using RankUpAPI.DTOs;
using RankUpAPI.Models;
using RankUpAPI.Repositories.Interfaces;
using RankUpAPI.Services.Interfaces;

namespace RankUpAPI.Services
{
    public class QualificationService : IQualificationService
    {
        private readonly IQualificationRepository _qualificationRepository;
        private readonly IExamQualificationRepository _examQualificationRepository;
        private readonly IMapper _mapper;

        public QualificationService(IQualificationRepository qualificationRepository, IExamQualificationRepository examQualificationRepository, IMapper mapper)
        {
            _qualificationRepository = qualificationRepository;
            _examQualificationRepository = examQualificationRepository;
            _mapper = mapper;
        }

        public async Task<QualificationDto> CreateQualificationAsync(CreateQualificationDto createDto)
        {
            var qualification = _mapper.Map<Qualification>(createDto);
            qualification.CreatedAt = DateTime.UtcNow;
            qualification.IsActive = true;
            qualification.UpdatedAt = null; // Explicitly set to null for new records

            await _qualificationRepository.AddAsync(qualification);
            await _qualificationRepository.SaveChangesAsync();

            return _mapper.Map<QualificationDto>(qualification);
        }

        public async Task<bool> DeleteQualificationAsync(int id)
        {
            var qualification = await _qualificationRepository.GetByIdAsync(id);
            if (qualification == null)
                return false;

            // Check if there are any exams associated with this qualification
            var hasExams = await _examQualificationRepository.HasExamsForQualificationAsync(id);

            if (hasExams)
                return false; // Or handle this case differently (e.g., soft delete)

            await _qualificationRepository.DeleteAsync(qualification);
            await _qualificationRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<QualificationDto>> GetAllQualificationsAsync()
        {
            var qualifications = await _qualificationRepository.GetActiveAsync();

            return _mapper.Map<IEnumerable<QualificationDto>>(qualifications.OrderBy(q => q.Name));
        }

        public async Task<QualificationDto?> GetQualificationByIdAsync(int id)
        {
            var qualification = await _qualificationRepository.GetByIdAsync(id);
            return qualification == null ? null : _mapper.Map<QualificationDto>(qualification);
        }

        public async Task<bool> ToggleQualificationStatusAsync(int id, bool isActive)
        {
            var qualification = await _qualificationRepository.GetByIdAsync(id);
            if (qualification == null)
                return false;

            qualification.IsActive = isActive;
            await _qualificationRepository.UpdateAsync(qualification);
            await _qualificationRepository.SaveChangesAsync();
            return true;
        }

        public async Task<QualificationDto?> UpdateQualificationAsync(int id, UpdateQualificationDto updateDto)
        {
            var qualification = await _qualificationRepository.GetByIdAsync(id);
            if (qualification == null)
                return null;
            _mapper.Map(updateDto, qualification);
            await _qualificationRepository.UpdateAsync(qualification);
            await _qualificationRepository.SaveChangesAsync();
            return _mapper.Map<QualificationDto>(qualification);
        }
    }
}
