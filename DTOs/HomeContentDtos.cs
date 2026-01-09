using RankUpAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace RankUpAPI.DTOs
{
    public class HomeSectionItemCreateDto
    {
        [Required]
        public HomeSectionType SectionType { get; set; }

        public int? ExamId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Subtitle { get; set; }

        [MaxLength(50)]
        public string? ProgressText { get; set; }

        [MaxLength(50)]
        public string? Tag { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(200)]
        public string? ActionKey { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsVisible { get; set; } = true;
    }

    public class HomeSectionItemUpdateDto : HomeSectionItemCreateDto
    {
        [Required]
        public int Id { get; set; }
    }

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

    /// <summary>
    /// Grouped response that mobile app will consume on home screen.
    /// </summary>
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

