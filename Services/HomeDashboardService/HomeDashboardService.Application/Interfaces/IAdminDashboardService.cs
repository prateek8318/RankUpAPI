using HomeDashboardService.Application.DTOs;

namespace HomeDashboardService.Application.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardMetricsDto> GetAdminDashboardMetricsAsync();
        Task<MetricTrendsDto> GetMetricTrendsAsync();
        
        // Banner management
        Task<DashboardBannerDto> CreateBannerAsync(CreateBannerDto banner);
        Task<DashboardBannerDto?> UpdateBannerAsync(int id, UpdateBannerDto banner);
        
        // Free test management
        Task<FreeTestDto> CreateFreeTestAsync(CreateFreeTestDto test);
        Task<FreeTestDto?> UpdateFreeTestAsync(int id, UpdateFreeTestDto test);
        
        // Daily target management
        Task<DailyTargetDto> CreateDailyTargetAsync(CreateDailyTargetDto target);
        Task<DailyTargetDto?> UpdateDailyTargetAsync(int id, UpdateDailyTargetDto target);
        
        // Trending test management
        Task<TrendingTestDto> CreateTrendingTestAsync(CreateTrendingTestDto test);
        Task<TrendingTestDto?> UpdateTrendingTestAsync(int id, UpdateTrendingTestDto test);
    }
}
