using AdminService.Application.DTOs;

namespace AdminService.Application.Interfaces
{
    public interface IDashboardAggregationService
    {
        Task<AdminDashboardMetricsDto> GetAggregatedDashboardMetricsAsync();
        Task<MetricTrendsDto> GetMetricTrendsAsync();
    }
}
