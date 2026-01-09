using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RankUpAPI.Data;
using RankUpAPI.DTOs;
using RankUpAPI.Models;
using RankUpAPI.Services.Interfaces;

namespace RankUpAPI.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public SubjectService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto createDto)
        {
            var subject = _mapper.Map<Subject>(createDto);
            subject.CreatedAt = DateTime.UtcNow;
            subject.IsActive = true;

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return await GetSubjectByIdAsync(subject.Id) ?? throw new Exception("Failed to retrieve created subject");
        }

        public async Task<bool> DeleteSubjectAsync(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
                return false;

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SubjectDto>> GetAllSubjectsAsync()
        {
            var subjects = await _context.Subjects
                .Include(s => s.Exam)
                .Include(s => s.Chapters)
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();

            return subjects.Select(s => new SubjectDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                ExamId = s.ExamId,
                ExamName = s.Exam.Name,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                ChapterCount = s.Chapters.Count
            });
        }

        public async Task<SubjectDto?> GetSubjectByIdAsync(int id)
        {
            var subject = await _context.Subjects
                .Include(s => s.Exam)
                .Include(s => s.Chapters)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subject == null)
                return null;

            return new SubjectDto
            {
                Id = subject.Id,
                Name = subject.Name,
                Description = subject.Description,
                ExamId = subject.ExamId,
                ExamName = subject.Exam.Name,
                IsActive = subject.IsActive,
                CreatedAt = subject.CreatedAt,
                UpdatedAt = subject.UpdatedAt,
                ChapterCount = subject.Chapters.Count
            };
        }

        public async Task<IEnumerable<SubjectDto>> GetSubjectsByExamIdAsync(int examId)
        {
            var subjects = await _context.Subjects
                .Include(s => s.Exam)
                .Include(s => s.Chapters)
                .Where(s => s.ExamId == examId && s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();

            return subjects.Select(s => new SubjectDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                ExamId = s.ExamId,
                ExamName = s.Exam.Name,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                ChapterCount = s.Chapters.Count
            });
        }

        public async Task<bool> ToggleSubjectStatusAsync(int id, bool isActive)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
                return false;

            subject.IsActive = isActive;
            subject.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<SubjectDto?> UpdateSubjectAsync(int id, UpdateSubjectDto updateDto)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
                return null;

            subject.Name = updateDto.Name;
            subject.Description = updateDto.Description;
            subject.IsActive = updateDto.IsActive;
            subject.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetSubjectByIdAsync(id);
        }
    }
}
