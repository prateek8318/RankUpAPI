using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RankUpAPI.Data;
using RankUpAPI.DTOs;
using RankUpAPI.Models;
using RankUpAPI.Services.Interfaces;

namespace RankUpAPI.Services
{
    public class ExamService : IExamService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ExamService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ExamDto> CreateExamAsync(CreateExamDto createDto)
        {
            var exam = _mapper.Map<Exam>(createDto);
            exam.CreatedAt = DateTime.UtcNow;
            exam.IsActive = true;

            // Add the exam first to get the ID
            _context.Exams.Add(exam);
            await _context.SaveChangesAsync();

            // Now handle the many-to-many relationships
            if (createDto.QualificationIds?.Any() == true)
            {
                foreach (var qualificationId in createDto.QualificationIds)
                {
                    var qualification = await _context.Qualifications.FindAsync(qualificationId);
                    if (qualification != null)
                    {
                        exam.ExamQualifications.Add(new ExamQualification
                        {
                            ExamId = exam.Id,
                            QualificationId = qualificationId
                        });
                    }
                }
                await _context.SaveChangesAsync();
            }

            var result = _mapper.Map<ExamDto>(exam);
            result.QualificationIds = createDto.QualificationIds ?? new List<int>();
            return result;
        }

        public async Task<bool> DeleteExamAsync(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam == null)
                return false;

            // Remove related exam qualifications
            var examQualifications = _context.ExamQualifications.Where(eq => eq.ExamId == id);
            _context.ExamQualifications.RemoveRange(examQualifications);

            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ExamDto>> GetAllExamsAsync()
        {
            var exams = await _context.Exams
                .Include(e => e.ExamQualifications)
                .Where(e => e.IsActive)
                .OrderBy(e => e.Name)
                .ToListAsync();

            var examDtos = new List<ExamDto>();
            foreach (var exam in exams)
            {
                var examDto = _mapper.Map<ExamDto>(exam);
                examDto.QualificationIds = exam.ExamQualifications.Select(eq => eq.QualificationId).ToList();
                examDtos.Add(examDto);
            }

            return examDtos;
        }

        public async Task<ExamDto?> GetExamByIdAsync(int id)
        {
            var exam = await _context.Exams
                .Include(e => e.ExamQualifications)
                .FirstOrDefaultAsync(e => e.Id == id);
                
            if (exam == null)
                return null;
                
            var examDto = _mapper.Map<ExamDto>(exam);
            examDto.QualificationIds = exam.ExamQualifications?.Select(eq => eq.QualificationId).ToList() ?? new List<int>();
            return examDto;
        }

        public async Task<bool> ToggleExamStatusAsync(int id, bool isActive)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam == null)
                return false;

            exam.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ExamDto>> GetExamsByQualificationAsync(int qualificationId)
        {
            var examIds = await _context.ExamQualifications
                .Where(eq => eq.QualificationId == qualificationId)
                .Select(eq => eq.ExamId)
                .ToListAsync();

            var exams = await _context.Exams
                .Where(e => examIds.Contains(e.Id) && e.IsActive)
                .OrderBy(e => e.Name)
                .ToListAsync();

            var examDtos = new List<ExamDto>();
            foreach (var exam in exams)
            {
                var examDto = _mapper.Map<ExamDto>(exam);
                examDto.QualificationIds = exam.ExamQualifications?.Select(eq => eq.QualificationId).ToList() ?? new List<int>();
                examDtos.Add(examDto);
            }

            return examDtos;
        }

        public async Task<ExamDto?> UpdateExamAsync(int id, UpdateExamDto updateDto)
{
    var exam = await _context.Exams
        .Include(e => e.ExamQualifications)
        .FirstOrDefaultAsync(e => e.Id == id);
    if (exam == null)
        return null;
    // Update exam properties
    _mapper.Map(updateDto, exam);
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
            exam.ExamQualifications?.Remove(eq);
        }
        // Add new relationships
        foreach (var qualificationId in updateDto.QualificationIds.Except(existingQualificationIds))
        {
            var qualification = await _context.Qualifications.FindAsync(qualificationId);
            if (qualification != null)
            {
                exam.ExamQualifications ??= new List<ExamQualification>();
                exam.ExamQualifications.Add(new ExamQualification
                {
                    ExamId = exam.Id,
                    QualificationId = qualificationId
                });
            }
        }
    }
    await _context.SaveChangesAsync();
    var result = _mapper.Map<ExamDto>(exam);
    result.QualificationIds = exam.ExamQualifications?.Select(eq => eq.QualificationId).ToList() ?? new List<int>();
    return result;
}
    }
}
