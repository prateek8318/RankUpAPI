using RankUpAPI.Models;

namespace RankUpAPI.Areas.Users.HomeContent.DTOs
{
    public class HomeSectionItemDto
    {
        public int Id { get; set; }
        public HomeSectionType SectionType { get; set; }
        public int? ExamId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string? ProgressText { get; set; }
        public string? Tag { get; set; }
        public string? ImageUrl { get; set; }
        public string? ActionKey { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsVisible { get; set; }
    }

    public class HomeContentResponseDto
    {
        public IList<HomeSectionItemDto> MockTests { get; set; } = new List<HomeSectionItemDto>();
        public IList<HomeSectionItemDto> TestSeries { get; set; } = new List<HomeSectionItemDto>();
        public IList<HomeSectionItemDto> PracticeSets { get; set; } = new List<HomeSectionItemDto>();
        public IList<HomeSectionItemDto> ContinuePractice { get; set; } = new List<HomeSectionItemDto>();
        public IList<HomeSectionItemDto> DailyTargets { get; set; } = new List<HomeSectionItemDto>();
        public IList<HomeSectionItemDto> RecommendedTestSeries { get; set; } = new List<HomeSectionItemDto>();
        public IList<HomeSectionItemDto> ChoosePracticeMode { get; set; } = new List<HomeSectionItemDto>();
        public IList<HomeSectionItemDto> ExamMode { get; set; } = new List<HomeSectionItemDto>();
        public IList<HomeSectionItemDto> RapidFireTests { get; set; } = new List<HomeSectionItemDto>();
        public IList<HomeSectionItemDto> FreeTests { get; set; } = new List<HomeSectionItemDto>();
    }
}
