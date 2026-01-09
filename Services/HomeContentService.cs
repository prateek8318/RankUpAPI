using Microsoft.EntityFrameworkCore;
using RankUpAPI.Data;
using RankUpAPI.DTOs;
using RankUpAPI.Models;
using RankUpAPI.Services.Interfaces;

namespace RankUpAPI.Services
{
    public class HomeContentService : IHomeContentService
    {
        private readonly ApplicationDbContext _context;

        public HomeContentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<HomeSectionItemDto> CreateAsync(HomeSectionItemCreateDto dto, CancellationToken cancellationToken = default)
        {
            var entity = new HomeSectionItem
            {
                SectionType = dto.SectionType,
                ExamId = dto.ExamId,
                Title = dto.Title,
                Subtitle = dto.Subtitle,
                ProgressText = dto.ProgressText,
                Tag = dto.Tag,
                ImageUrl = dto.ImageUrl,
                ActionKey = dto.ActionKey,
                DisplayOrder = dto.DisplayOrder,
                IsVisible = dto.IsVisible
            };

            _context.HomeSectionItems.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return MapToDto(entity);
        }

        public async Task<HomeSectionItemDto?> UpdateAsync(HomeSectionItemUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var entity = await _context.HomeSectionItems
                .FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);

            if (entity == null)
            {
                return null;
            }

            entity.SectionType = dto.SectionType;
            entity.ExamId = dto.ExamId;
            entity.Title = dto.Title;
            entity.Subtitle = dto.Subtitle;
            entity.ProgressText = dto.ProgressText;
            entity.Tag = dto.Tag;
            entity.ImageUrl = dto.ImageUrl;
            entity.ActionKey = dto.ActionKey;
            entity.DisplayOrder = dto.DisplayOrder;
            entity.IsVisible = dto.IsVisible;

            await _context.SaveChangesAsync(cancellationToken);

            return MapToDto(entity);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.HomeSectionItems
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (entity == null)
            {
                return false;
            }

            _context.HomeSectionItems.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<IList<HomeSectionItemDto>> GetBySectionAsync(HomeSectionType sectionType, int? examId, CancellationToken cancellationToken = default)
        {
            var query = _context.HomeSectionItems
                .AsNoTracking()
                .Where(x => x.SectionType == sectionType && x.IsVisible);

            if (examId.HasValue)
            {
                query = query.Where(x => x.ExamId == examId || x.ExamId == null);
            }

            var items = await query
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync(cancellationToken);

            return items.Select(MapToDto).ToList();
        }

        public async Task<HomeContentResponseDto> GetHomeContentAsync(int? examId, CancellationToken cancellationToken = default)
        {
            var query = _context.HomeSectionItems
                .AsNoTracking()
                .Where(x => x.IsVisible);

            if (examId.HasValue)
            {
                query = query.Where(x => x.ExamId == examId || x.ExamId == null);
            }

            var items = await query
                .OrderBy(x => x.SectionType)
                .ThenBy(x => x.DisplayOrder)
                .ToListAsync(cancellationToken);

            var grouped = new HomeContentResponseDto
            {
                MockTests = Filter(items, HomeSectionType.MockTest),
                TestSeries = Filter(items, HomeSectionType.TestSeries),
                PracticeSets = Filter(items, HomeSectionType.PracticeSets),
                ContinuePractice = Filter(items, HomeSectionType.ContinuePractice),
                DailyTargets = Filter(items, HomeSectionType.DailyTargets),
                RecommendedTestSeries = Filter(items, HomeSectionType.RecommendedTestSeries),
                ChoosePracticeMode = Filter(items, HomeSectionType.ChoosePracticeMode),
                ExamMode = Filter(items, HomeSectionType.ExamMode),
                RapidFireTests = Filter(items, HomeSectionType.RapidFireTests),
                FreeTests = Filter(items, HomeSectionType.FreeTests)
            };

            return grouped;
        }

        private static IList<HomeSectionItemDto> Filter(IEnumerable<HomeSectionItem> items, HomeSectionType type)
        {
            return items
                .Where(x => x.SectionType == type)
                .OrderBy(x => x.DisplayOrder)
                .Select(MapToDto)
                .ToList();
        }

        private static HomeSectionItemDto MapToDto(HomeSectionItem entity)
        {
            return new HomeSectionItemDto
            {
                Id = entity.Id,
                SectionType = entity.SectionType,
                ExamId = entity.ExamId,
                Title = entity.Title,
                Subtitle = entity.Subtitle,
                ProgressText = entity.ProgressText,
                Tag = entity.Tag,
                ImageUrl = entity.ImageUrl,
                ActionKey = entity.ActionKey,
                DisplayOrder = entity.DisplayOrder,
                IsVisible = entity.IsVisible
            };
        }
    }
}

