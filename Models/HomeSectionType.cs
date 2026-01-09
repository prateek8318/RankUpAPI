using System.Text.Json.Serialization;

namespace RankUpAPI.Models
{
    /// <summary>
    /// Logical groupings for home screen content as per the app design.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum HomeSectionType
    {
        // Top cards
        MockTest = 1,
        TestSeries = 2,
        PracticeSets = 3,
        ContinuePractice = 4,

        // Middle sections
        DailyTargets = 10,
        RecommendedTestSeries = 11,
        ChoosePracticeMode = 12,
        ExamMode = 13,
        RapidFireTests = 14,
        FreeTests = 15
    }
}

