using HomeDashboardService.Application.DTOs;

namespace HomeDashboardService.Application.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardMetricsDto> GetAdminDashboardMetricsAsync();
        Task<MetricTrendsDto> GetMetricTrendsAsync();
    }
}
