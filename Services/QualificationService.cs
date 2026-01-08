using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RankUpAPI.Data;
using RankUpAPI.DTOs;
using RankUpAPI.Models;
using RankUpAPI.Services.Interfaces;

namespace RankUpAPI.Services
{
    public class QualificationService : IQualificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public QualificationService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<QualificationDto> CreateQualificationAsync(CreateQualificationDto createDto)
        {
            var qualification = _mapper.Map<Qualification>(createDto);
            qualification.CreatedAt = DateTime.UtcNow;
            qualification.IsActive = true;
            qualification.UpdatedAt = null; // Explicitly set to null for new records

            _context.Qualifications.Add(qualification);
            await _context.SaveChangesAsync();

            return _mapper.Map<QualificationDto>(qualification);
        }

        public async Task<bool> DeleteQualificationAsync(int id)
        {
            var qualification = await _context.Qualifications.FindAsync(id);
            if (qualification == null)
                return false;

            // Check if there are any exams associated with this qualification
            var hasExams = await _context.ExamQualifications
                .AnyAsync(eq => eq.QualificationId == id);

            if (hasExams)
                return false; // Or handle this case differently (e.g., soft delete)

            _context.Qualifications.Remove(qualification);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<QualificationDto>> GetAllQualificationsAsync()
        {
            var qualifications = await _context.Qualifications
                .Where(q => q.IsActive)
                .OrderBy(q => q.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<QualificationDto>>(qualifications);
        }

        public async Task<QualificationDto?> GetQualificationByIdAsync(int id)
{
    var qualification = await _context.Qualifications.FindAsync(id);
    return qualification == null ? null : _mapper.Map<QualificationDto>(qualification);
}

        public async Task<bool> ToggleQualificationStatusAsync(int id, bool isActive)
        {
            var qualification = await _context.Qualifications.FindAsync(id);
            if (qualification == null)
                return false;

            qualification.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<QualificationDto?> UpdateQualificationAsync(int id, UpdateQualificationDto updateDto)
{
    var qualification = await _context.Qualifications.FindAsync(id);
    if (qualification == null)
        return null;
    _mapper.Map(updateDto, qualification);
    await _context.SaveChangesAsync();
    return _mapper.Map<QualificationDto>(qualification);
}
    }
}
