using AutoMapper;
using RankUpAPI.Areas.Admin.Chapter.DTOs;
using RankUpAPI.Areas.Admin.Chapter.Services.Interfaces;
using RankUpAPI.Models;
using RankUpAPI.Repositories.Interfaces;
using ChapterModel = RankUpAPI.Models.Chapter;

namespace RankUpAPI.Areas.Admin.Chapter.Services.Implementations
{
    public class ChapterService : IChapterService
    {
        private readonly IChapterRepository _chapterRepository;
        private readonly IMapper _mapper;

        public ChapterService(IChapterRepository chapterRepository, IMapper mapper)
        {
            _chapterRepository = chapterRepository;
            _mapper = mapper;
        }

        public async Task<ChapterDto> CreateChapterAsync(CreateChapterDto createDto)
        {
            var chapter = _mapper.Map<ChapterModel>(createDto);
            chapter.CreatedAt = DateTime.UtcNow;
            chapter.IsActive = true;

            await _chapterRepository.AddAsync(chapter);
            await _chapterRepository.SaveChangesAsync();

            return await GetChapterByIdAsync(chapter.Id) ?? throw new Exception("Failed to retrieve created chapter");
        }

        public async Task<bool> DeleteChapterAsync(int id)
        {
            var chapter = await _chapterRepository.GetByIdAsync(id);
            if (chapter == null)
                return false;

            await _chapterRepository.DeleteAsync(chapter);
            await _chapterRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ChapterDto>> GetAllChaptersAsync()
        {
            var chapters = await _chapterRepository.GetActiveAsync();

            return chapters.OrderBy(c => c.Name).Select(c => new ChapterDto
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
            var chapter = await _chapterRepository.GetByIdWithDetailsAsync(id);

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
            var chapters = await _chapterRepository.GetBySubjectIdAsync(subjectId);

            return chapters.OrderBy(c => c.Name).Select(c => new ChapterDto
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
            var chapter = await _chapterRepository.GetByIdAsync(id);
            if (chapter == null)
                return false;

            chapter.IsActive = isActive;
            chapter.UpdatedAt = DateTime.UtcNow;
            await _chapterRepository.UpdateAsync(chapter);
            await _chapterRepository.SaveChangesAsync();
            return true;
        }

        public async Task<ChapterDto?> UpdateChapterAsync(int id, UpdateChapterDto updateDto)
        {
            var chapter = await _chapterRepository.GetByIdAsync(id);
            if (chapter == null)
                return null;

            chapter.Name = updateDto.Name;
            chapter.Description = updateDto.Description;
            chapter.IsActive = updateDto.IsActive;
            chapter.UpdatedAt = DateTime.UtcNow;

            await _chapterRepository.UpdateAsync(chapter);
            await _chapterRepository.SaveChangesAsync();
            return await GetChapterByIdAsync(id);
        }
    }
}
