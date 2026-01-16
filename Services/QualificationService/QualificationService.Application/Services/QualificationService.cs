using AutoMapper;
using QualificationService.Application.DTOs;
using QualificationService.Application.Interfaces;
using QualificationService.Domain.Entities;

namespace QualificationService.Application.Services
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

            await _qualificationRepository.AddAsync(qualification);
            await _qualificationRepository.SaveChangesAsync();

            return _mapper.Map<QualificationDto>(qualification);
        }

        public async Task<QualificationDto?> UpdateQualificationAsync(int id, UpdateQualificationDto updateDto)
        {
            var qualification = await _qualificationRepository.GetByIdAsync(id);
            if (qualification == null)
                return null;

            _mapper.Map(updateDto, qualification);
            qualification.UpdatedAt = DateTime.UtcNow;
            await _qualificationRepository.UpdateAsync(qualification);
            await _qualificationRepository.SaveChangesAsync();

            return _mapper.Map<QualificationDto>(qualification);
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

        public async Task<QualificationDto?> GetQualificationByIdAsync(int id)
        {
            var qualification = await _qualificationRepository.GetByIdAsync(id);
            return qualification == null ? null : _mapper.Map<QualificationDto>(qualification);
        }

        public async Task<IEnumerable<QualificationDto>> GetAllQualificationsAsync()
        {
            var qualifications = await _qualificationRepository.GetActiveAsync();
            return qualifications.OrderBy(q => q.Name).Select(q => _mapper.Map<QualificationDto>(q));
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
