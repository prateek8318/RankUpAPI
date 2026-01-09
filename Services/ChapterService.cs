using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RankUpAPI.Data;
using RankUpAPI.DTOs;
using RankUpAPI.Models;
using RankUpAPI.Services.Interfaces;

namespace RankUpAPI.Services
{
    public class ChapterService : IChapterService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ChapterService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ChapterDto> CreateChapterAsync(CreateChapterDto createDto)
        {
            var chapter = _mapper.Map<Chapter>(createDto);
            chapter.CreatedAt = DateTime.UtcNow;
            chapter.IsActive = true;

            _context.Chapters.Add(chapter);
            await _context.SaveChangesAsync();

            return await GetChapterByIdAsync(chapter.Id) ?? throw new Exception("Failed to retrieve created chapter");
        }

        public async Task<bool> DeleteChapterAsync(int id)
        {
            var chapter = await _context.Chapters.FindAsync(id);
            if (chapter == null)
                return false;

            _context.Chapters.Remove(chapter);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ChapterDto>> GetAllChaptersAsync()
        {
            var chapters = await _context.Chapters
                .Include(c => c.Subject)
                    .ThenInclude(s => s.Exam)
                .Include(c => c.Questions)
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return chapters.Select(c => new ChapterDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                SubjectId = c.SubjectId,
                SubjectName = c.Subject.Name,
                ExamId = c.Subject.ExamId,
                ExamName = c.Subject.Exam.Name,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                QuestionCount = c.Questions.Count
            });
        }

        public async Task<ChapterDto?> GetChapterByIdAsync(int id)
        {
            var chapter = await _context.Chapters
                .Include(c => c.Subject)
                    .ThenInclude(s => s.Exam)
                .Include(c => c.Questions)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chapter == null)
                return null;

            return new ChapterDto
            {
                Id = chapter.Id,
                Name = chapter.Name,
                Description = chapter.Description,
                SubjectId = chapter.SubjectId,
                SubjectName = chapter.Subject.Name,
                ExamId = chapter.Subject.ExamId,
                ExamName = chapter.Subject.Exam.Name,
                IsActive = chapter.IsActive,
                CreatedAt = chapter.CreatedAt,
                UpdatedAt = chapter.UpdatedAt,
                QuestionCount = chapter.Questions.Count
            };
        }

        public async Task<IEnumerable<ChapterDto>> GetChaptersBySubjectIdAsync(int subjectId)
        {
            var chapters = await _context.Chapters
                .Include(c => c.Subject)
                    .ThenInclude(s => s.Exam)
                .Include(c => c.Questions)
                .Where(c => c.SubjectId == subjectId && c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return chapters.Select(c => new ChapterDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                SubjectId = c.SubjectId,
                SubjectName = c.Subject.Name,
                ExamId = c.Subject.ExamId,
                ExamName = c.Subject.Exam.Name,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                QuestionCount = c.Questions.Count
            });
        }

        public async Task<bool> ToggleChapterStatusAsync(int id, bool isActive)
        {
            var chapter = await _context.Chapters.FindAsync(id);
            if (chapter == null)
                return false;

            chapter.IsActive = isActive;
            chapter.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ChapterDto?> UpdateChapterAsync(int id, UpdateChapterDto updateDto)
        {
            var chapter = await _context.Chapters.FindAsync(id);
            if (chapter == null)
                return null;

            chapter.Name = updateDto.Name;
            chapter.Description = updateDto.Description;
            chapter.IsActive = updateDto.IsActive;
            chapter.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetChapterByIdAsync(id);
        }
    }
}
