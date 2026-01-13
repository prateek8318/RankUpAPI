using RankUpAPI.Areas.Admin.HomeContent.DTOs;
using RankUpAPI.Areas.Admin.HomeContent.Services.Interfaces;
using RankUpAPI.Models;
using RankUpAPI.Repositories.Interfaces;

namespace RankUpAPI.Areas.Admin.HomeContent.Services.Implementations
{
    public class HomeContentService : IHomeContentService
    {
        private readonly IHomeSectionItemRepository _homeSectionItemRepository;

        public HomeContentService(IHomeSectionItemRepository homeSectionItemRepository)
        {
            _homeSectionItemRepository = homeSectionItemRepository;
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

            await _homeSectionItemRepository.AddAsync(entity);
            await _homeSectionItemRepository.SaveChangesAsync();

            return MapToDto(entity);
        }

        public async Task<HomeSectionItemDto?> UpdateAsync(HomeSectionItemUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var entity = await _homeSectionItemRepository.GetByIdAsync(dto.Id);

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

            await _homeSectionItemRepository.UpdateAsync(entity);
            await _homeSectionItemRepository.SaveChangesAsync();

            return MapToDto(entity);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _homeSectionItemRepository.GetByIdAsync(id);

            if (entity == null)
            {
                return false;
            }

            await _homeSectionItemRepository.DeleteAsync(entity);
            await _homeSectionItemRepository.SaveChangesAsync();

            return true;
        }

        public async Task<IList<HomeSectionItemDto>> GetBySectionAsync(HomeSectionType sectionType, int? examId = null, CancellationToken cancellationToken = default)
        {
            var items = await _homeSectionItemRepository.GetBySectionTypeAsync(sectionType, examId);

            return items.Select(MapToDto).ToList();
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
