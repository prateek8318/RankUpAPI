using AutoMapper;
using RankUpAPI.Areas.Admin.Qualification.DTOs;
using RankUpAPI.Areas.Admin.Qualification.Services.Interfaces;
using RankUpAPI.Models;
using RankUpAPI.Repositories.Interfaces;
using QualificationModel = RankUpAPI.Models.Qualification;

namespace RankUpAPI.Areas.Admin.Qualification.Services.Implementations
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
            var qualification = _mapper.Map<QualificationModel>(createDto);
            qualification.CreatedAt = DateTime.UtcNow;
            qualification.IsActive = true;
            qualification.UpdatedAt = null;

            await _qualificationRepository.AddAsync(qualification);
            await _qualificationRepository.SaveChangesAsync();

            return _mapper.Map<QualificationDto>(qualification);
        }

        public async Task<bool> DeleteQualificationAsync(int id)
        {
            var qualification = await _qualificationRepository.GetByIdAsync(id);
            if (qualification == null)
                return false;

            var hasExams = await _examQualificationRepository.HasExamsForQualificationAsync(id);
            if (hasExams)
                return false;

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
