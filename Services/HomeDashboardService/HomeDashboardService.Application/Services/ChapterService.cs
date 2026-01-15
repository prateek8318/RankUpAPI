using AutoMapper;
using HomeDashboardService.Application.DTOs;
using HomeDashboardService.Application.Interfaces;
using HomeDashboardService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace HomeDashboardService.Application.Services
{
    public class ChapterService : IChapterService
    {
        private readonly IChapterRepository _chapterRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ChapterService> _logger;

        public ChapterService(
            IChapterRepository chapterRepository,
            ISubjectRepository subjectRepository,
            IMapper mapper,
            ILogger<ChapterService> logger)
        {
            _chapterRepository = chapterRepository;
            _subjectRepository = subjectRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ChapterDto> CreateChapterAsync(CreateChapterDto createChapterDto)
        {
            try
            {
                var subject = await _subjectRepository.GetByIdAsync(createChapterDto.SubjectId);
                if (subject == null)
                    throw new KeyNotFoundException($"Subject with ID {createChapterDto.SubjectId} not found");

                var chapter = _mapper.Map<Domain.Entities.Chapter>(createChapterDto);
                var created = await _chapterRepository.AddAsync(chapter);
                await _chapterRepository.SaveChangesAsync();
                return _mapper.Map<ChapterDto>(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating chapter");
                throw;
            }
        }

        public async Task<ChapterDto?> UpdateChapterAsync(int id, UpdateChapterDto updateChapterDto)
        {
            try
            {
                var chapter = await _chapterRepository.GetByIdWithQuizzesAsync(id);
                if (chapter == null)
                    return null;

                _mapper.Map(updateChapterDto, chapter);
                chapter.UpdatedAt = DateTime.UtcNow;
                await _chapterRepository.UpdateAsync(chapter);
                await _chapterRepository.SaveChangesAsync();
                return _mapper.Map<ChapterDto>(chapter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating chapter: {ChapterId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteChapterAsync(int id)
        {
            try
            {
                return await _chapterRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting chapter: {ChapterId}", id);
                throw;
            }
        }

        public async Task<bool> EnableDisableChapterAsync(int id, bool isActive)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling/disabling chapter: {ChapterId}", id);
                throw;
            }
        }

        public async Task<ChapterDto?> GetChapterByIdAsync(int id)
        {
            try
            {
                var chapter = await _chapterRepository.GetByIdWithQuizzesAsync(id);
                return chapter != null ? _mapper.Map<ChapterDto>(chapter) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting chapter: {ChapterId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ChapterDto>> GetChaptersBySubjectIdAsync(int subjectId)
        {
            try
            {
                var chapters = await _chapterRepository.GetBySubjectIdAsync(subjectId);
                return _mapper.Map<IEnumerable<ChapterDto>>(chapters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting chapters for subject: {SubjectId}", subjectId);
                throw;
            }
        }
    }
}
