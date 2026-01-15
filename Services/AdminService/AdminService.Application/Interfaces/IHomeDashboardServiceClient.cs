namespace AdminService.Application.Interfaces
{
    public interface IHomeDashboardServiceClient
    {
        Task<object?> GetDailyVideoAsync();
        Task<object?> CreateDailyVideoAsync(object createDto);
        Task<object?> UpdateDailyVideoAsync(int id, object updateDto);
        Task<bool> DeleteDailyVideoAsync(int id);
        Task<object?> GetAllDailyVideosAsync();
        Task<object?> GetBannersAsync();
        Task<object?> CreateBannerAsync(object createDto);
        Task<object?> UpdateBannerAsync(int id, object updateDto);
        Task<bool> DeleteBannerAsync(int id);
    }
}
