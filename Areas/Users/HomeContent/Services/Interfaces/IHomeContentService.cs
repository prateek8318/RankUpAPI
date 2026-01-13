using RankUpAPI.Areas.Users.HomeContent.DTOs;

namespace RankUpAPI.Areas.Users.HomeContent.Services.Interfaces
{
    public interface IHomeContentService
    {
        Task<HomeContentResponseDto> GetHomeContentAsync(int? examId, CancellationToken cancellationToken = default);
    }
}
