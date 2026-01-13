using RankUpAPI.Areas.Users.HomeContent.DTOs;
using RankUpAPI.Areas.Users.HomeContent.Services.Interfaces;
using RankUpAPI.Models;
using RankUpAPI.Repositories.Interfaces;

namespace RankUpAPI.Areas.Users.HomeContent.Services.Implementations
{
    public class HomeContentService : IHomeContentService
    {
        private readonly IHomeSectionItemRepository _homeSectionItemRepository;

        public HomeContentService(IHomeSectionItemRepository homeSectionItemRepository)
        {
            _homeSectionItemRepository = homeSectionItemRepository;
        }

        public async Task<HomeContentResponseDto> GetHomeContentAsync(int? examId, CancellationToken cancellationToken = default)
        {
            var items = await _homeSectionItemRepository.GetVisibleAsync(examId);

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
